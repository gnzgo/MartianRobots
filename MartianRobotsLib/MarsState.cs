using System;
using System.Collections.Generic;
using System.Linq;

namespace MartianRobotsLib
{
	public partial class MarsState
	{
		// upper bounds for coordinate values and command lengths
		public const int MaxCoordinateValue = 50;
		public const int MaxRawMovementCommandLength = 100;

		// statistical properties
		public int AliveRobots { get => this.Robots.Where(robot => !robot.Lost).Count(); }
		public int DeadRobots { get => this.Robots.Count - AliveRobots; }
		public int TotalMovementsMade { get => this.Robots.Sum(robot => robot.MovementsMade); }

		public int TotalTiles { get => this.Surface.GetLength(0) * this.Surface.GetLength(1); }
		public int WalkedTiles { get => this.Surface.Cast<MarsState.MarsCoordinate>().Where(coord => coord.HasBeenWalked).Count(); }

		/// <summary>
		/// The state of a single cell (coordinate) in the Mars' 2-dimentional surface.
		/// </summary>
		public struct MarsCoordinate
		{
			public bool HasBeenWalked;
			public bool HasRobotScent;
		}

		/// <summary>
		/// A 2-dimensional array representing the surface of Mars
		/// </summary>
		public MarsCoordinate[,] Surface;

		/// <summary>
		/// List of robots that have stepped on Mars; for statistical purposes.
		/// </summary>
		public List<RobotState> Robots;

		/// <summary>
		/// Creates the surface of Mars and validates its creation arguments.
		/// </summary>
		/// <param name="rawSizeX">The zero-based width (X) of Mars.</param>
		/// <param name="rawSizeY">The zero-based heigh (Y) of Mars.</param>
		/// <returns></returns>
		public void CreateSurface(string rawSizeX, string rawSizeY)
		{
			// NULL CHECKING & EMPTY CHECKING THE INPUT PARAMETERS
			if (string.IsNullOrWhiteSpace(rawSizeX))
			{
				throw new ArgumentException($"'{nameof(rawSizeX)}' cannot be null or empty.", nameof(rawSizeX));
			}

			if (string.IsNullOrWhiteSpace(rawSizeY))
			{
				throw new ArgumentException($"'{nameof(rawSizeY)}' cannot be null or empty.", nameof(rawSizeY));
			}

			// DECLARING VALUES FOR CASTING
			int sizeX;
			int sizeY;

			// validate X, Y sizes as valid integers
			if (!int.TryParse(rawSizeX, out sizeX))
			{
				throw new ArgumentException($"Mars' width (X), '{rawSizeX}', cannot be parsed into an integer.", nameof(rawSizeX));
			}

			if (!int.TryParse(rawSizeY, out sizeY))
			{
				throw new ArgumentException($"Mars' height (Y), '{rawSizeY}', cannot be parsed into an integer.", nameof(rawSizeY));
			}

			// validate X and Y as not being both zero
			if (sizeX == 0 && sizeY == 0)
			{
				throw new ArgumentException($"The total size of Mars can't be zero horizontally and vertically.", nameof(rawSizeY));
			}

			// validate X and Y as not being less than zero; in this house, we respect the laws of physics
			if (sizeX < 0)
			{
				throw new ArgumentException($"Mars' width (X), '{sizeX}', cannot be less than zero. Don't break the universe, please.", nameof(rawSizeY));
			}

			if (sizeY < 0)
			{
				throw new ArgumentException($"Mars' height (Y), '{sizeY}', cannot be less than zero. Don't break the universe, please.", nameof(rawSizeY));
			}

			// validate X, Y sizes as being within the maximum range defined by the requirements document
			if (sizeX > MaxCoordinateValue)
			{
				throw new ArgumentException($"Mars' width (X), '{sizeX}', is above the allowed maximum ('{MaxCoordinateValue}').", nameof(rawSizeX));
			}

			if (sizeY > MaxCoordinateValue)
			{
				throw new ArgumentException($"Mars' height (Y) '{sizeY}', is above the allowed maximum ('{MaxCoordinateValue}').", nameof(rawSizeY));
			}

			// EVERYTHING'S VALID; RETURN OUR MARS 2-DIMENSIONAL COORDINATE ARRAY
			this.Surface = new MarsCoordinate[sizeX + 1, sizeY + 1];
		}
	}
}
