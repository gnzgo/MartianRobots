using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static MartianRobotsLib.MarsState;

namespace MartianRobotsRESTAPI
{
	public class MartianSimulation
	{
		[Required]
		public MarsSize MarsSize { get; set; }

		[Required]
		public RobotCommand[] RobotCommands { get; set; }
	}

	public class MarsSize
	{
		[Required]
		[Range(0, MartianRobotsLib.MarsState.MaxCoordinateValue)]
		public int X { get; set; }

		[Required]
		[Range(0, MartianRobotsLib.MarsState.MaxCoordinateValue)]
		public int Y { get; set; }
	}

	public class RobotCommand
	{
		[Required]
		public RobotPosition StartingPosition { get; set; }

		[Required]
		[MaxLength(MartianRobotsLib.MarsState.MaxRawMovementCommandLength)]
		public string MovementCommands { get; set; }
	}

	public class RobotPosition
	{
		[Required]
		[Range(0, MartianRobotsLib.MarsState.MaxCoordinateValue)]
		public int X { get; set; }

		[Required]
		[Range(0, MartianRobotsLib.MarsState.MaxCoordinateValue)]
		public int Y { get; set; }

		[Required]
		public string Orientation { get; set; }
	}

	public class RobotResult
	{
		public RobotPosition FinalPosition { get; set; }

		public bool Lost { get; set; }
	}
}
