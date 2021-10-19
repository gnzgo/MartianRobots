using MartianRobotsLib;
using System;
using Xunit;
using static MartianRobotsLib.MarsState;

namespace MartianRobotsTests
{
	public class Tests
	{
		// Mars can't be zero-sized
		// Mars can't be less-than-zero sized
		// Mars can't be sized over 50
		//
		// Original spec data leads to expected results
		// Moving over a cliff results in the robot being lost
		// Moving over a cliff results in the cell having a scent
		// Trying to move over a cliff from a scented cell does nothing
		// 
		// Providing invalid movement commands fails
		// Placing a robot outside of Mars fails
		// 
		// Moving west, east, north and south works as expected


		/// <summary>
		/// Tests that Mars can't be initialized with a zero-size surface.
		/// </summary>
		[Fact]
		public void MarsIsNotZeroSized()
		{
			MarsState Mars = new MarsState();

			Assert.Throws<ArgumentException>(() => Mars.CreateSurface("0", "0"));
		}

		/// <summary>
		/// Tests that Mars can't be initialized with a less-than-zero sided surface.
		/// </summary>
		[Theory]
		[InlineData("1", "-1")]
		[InlineData("-1", "1")]
		[InlineData("-1", "-1")]
		public void MarsIsNotLessThanZeroSized(string x, string y)
		{
			MarsState Mars = new MarsState();

			Assert.Throws<ArgumentException>(() => Mars.CreateSurface(x, y));
		}

		/// <summary>
		/// Tests that Mars can't be initialized with a surface bigger than the maximum allowed.
		/// </summary>
		[Theory]
		[InlineData(MartianRobotsLib.MarsState.MaxCoordinateValue + 1,	MartianRobotsLib.MarsState.MaxCoordinateValue + 1)]
		[InlineData(MartianRobotsLib.MarsState.MaxCoordinateValue + 1,	MartianRobotsLib.MarsState.MaxCoordinateValue)]
		[InlineData(MartianRobotsLib.MarsState.MaxCoordinateValue,		MartianRobotsLib.MarsState.MaxCoordinateValue + 1)]
		public void MarsIsNotOverMaximumSize(int x, int y)
		{
			MarsState Mars = new MarsState();

			Assert.Throws<ArgumentException>(() => Mars.CreateSurface(x.ToString(), y.ToString()));
		}

		[Theory]
		[InlineData(3, 3, "5 5 N")]
		[InlineData(20, 20, "25 5 E")]
		[InlineData(10, 8, "3 9 W")]
		[InlineData(41, 23, "40 24 S")]
		public void PlacingRobotOutsideOfBoundsFails(int x, int y, string initialLocation)
		{
			MarsState Mars = new MarsState();

			// create surface
			Mars.CreateSurface(x.ToString(), y.ToString());

			// get initial position
			string[] initialPos = initialLocation.Split(' ');

			// check that placing outside of bounds fails
			RobotState robotState;
			Assert.Throws<ArgumentException>(() => robotState = new(Mars, initialPos[0], initialPos[1], initialPos[2]));
		}

		/// <summary>
		/// Tests that the library complies with the sample inputs and outputs provided by the spec document.
		/// </summary>
		[Theory]
		[InlineData(5, 3, "1 1 E", "RFRFRFRF", "1 1 E")]
		[InlineData(5, 3, "3 2 N", "FRRFLLFFRRFLL", "3 3 N LOST")]
		[InlineData(5, 3, "0 3 W", "LLFFFRFLFL", "4 2 N")]
		public void OriginalDataLeadsToExpectedResult(int x, int y, string initialLocation, string moveCommands, string finalLocation)
		{
			MarsState Mars = new MarsState();

			// create surface
			Mars.CreateSurface(x.ToString(), y.ToString());

			// get initial position
			string[] initialPos = initialLocation.Split(' ');

			// place robot
			RobotState robotState = new(Mars, initialPos[0], initialPos[1], initialPos[2]);

			// move robot
			robotState.Move(moveCommands);

			// test
			Assert.Equal(robotState.ToString(), finalLocation);
		}

		/// <summary>
		/// Tests that robots are actually lost when they move off the edge of a cliff.
		/// </summary>
		[Theory]
		[InlineData(5, 5, "5 5 N", "F")]
		[InlineData(5, 5, "5 5 E", "F")]
		[InlineData(5, 5, "0 0 W", "F")]
		[InlineData(5, 5, "0 0 S", "F")]
		public void RobotIsLostWhenFallingOffTheEdge(int x, int y, string initialLocation, string moveCommands)
		{
			MarsState Mars = new MarsState();

			// create surface
			Mars.CreateSurface(x.ToString(), y.ToString());

			// get initial position
			string[] initialPos = initialLocation.Split(' ');

			// place robot
			RobotState robotState = new(Mars, initialPos[0], initialPos[1], initialPos[2]);

			// move robot
			robotState.Move(moveCommands);

			// check if lost
			Assert.True(robotState.Lost);
		}

		/// <summary>
		/// Tests that robots leave a scent when they move off the edge of a cliff.
		/// </summary>
		[Theory]
		[InlineData(5, 5, "5 5 N", "F")]
		[InlineData(5, 5, "5 5 E", "F")]
		[InlineData(5, 5, "0 0 W", "F")]
		[InlineData(5, 5, "0 0 S", "F")]
		public void RobotLeavesScentWhenFallingOffTheEdge(int x, int y, string initialLocation, string moveCommands)
		{
			MarsState Mars = new MarsState();

			// create surface
			Mars.CreateSurface(x.ToString(), y.ToString());

			// get initial position
			string[] initialPos = initialLocation.Split(' ');

			// place robot
			RobotState robotState = new(Mars, initialPos[0], initialPos[1], initialPos[2]);

			// move robot
			robotState.Move(moveCommands);

			// check if cell is scented
			Assert.True(Mars.Surface[int.Parse(initialPos[0]), int.Parse(initialPos[1])].HasRobotScent);
		}

		/// <summary>
		/// Tests that robots won't jump off a cliff if they've detected a danger scent.
		/// </summary>
		[Theory]
		[InlineData(5, 5, "5 5 N", "F")]
		[InlineData(5, 5, "5 5 E", "F")]
		[InlineData(5, 5, "0 0 W", "F")]
		[InlineData(5, 5, "0 0 S", "F")]
		public void RobotDoesNotFallWhenScented(int x, int y, string initialLocation, string moveCommands)
		{
			MarsState Mars = new MarsState();

			// create surface
			Mars.CreateSurface(x.ToString(), y.ToString());

			// get initial position
			string[] initialPos = initialLocation.Split(' ');

			// place robot
			RobotState robotState = new(Mars, initialPos[0], initialPos[1], initialPos[2]);

			// make cell scented
			Mars.Surface[int.Parse(initialPos[0]), int.Parse(initialPos[1])].HasRobotScent = true;

			// move robot
			robotState.Move(moveCommands);

			// check that the robot didn't jump over the cliff
			Assert.False(robotState.Lost);
		}

		[Theory]
		[InlineData("XXX")]
		[InlineData("ASDFG")]
		[InlineData("FFFFRRRRRX")]
		[InlineData("FLFLRFLRXFLRFLF")]
		public void InvalidMovementCommandsFail(string invalidCommand)
		{
			MarsState Mars = new MarsState();

			// create surface
			Mars.CreateSurface("5", "5");

			// place robot
			RobotState robotState = new(Mars, "0", "0", "E");

			// try to move robot with invalid commands
			Assert.Throws<ArgumentException>(() => robotState.Move(invalidCommand));
			
		}
	}
}
