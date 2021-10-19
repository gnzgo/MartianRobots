using System;

namespace MartianRobotsLib
{
	public partial class MarsState
	{
		/// <summary>
		/// The state of a particular robot at any moment in time.
		/// </summary>
		public class RobotState
		{
			// reference to the main Mars object
			private MarsState MarsState;

			// status
			public int X;
			public int Y;
			public RobotOrientation Orientation;
			public bool Lost;

			// statistics
			public int MovementsMade;

			/// <summary>
			/// Constructor for a new robot position, based on raw starting-point string arguments.
			/// </summary>
			/// <param name="rawX">A robot's zero-based X position within the Mars' surface grid.</param>
			/// <param name="rawY">A robot's zero-based Y position within the Mars' surface grid.</param>
			/// <param name="rawOrientation">A robot's orientation (north, south, east, west).</param>
			public RobotState(MarsState marsState, string rawX, string rawY, string rawOrientation)
			{
				MarsState = marsState;

				// NULL CHECKING & EMPTY CHECKING THE INPUT PARAMETERS
				if (string.IsNullOrWhiteSpace(rawX))
				{
					throw new ArgumentException($"'{nameof(rawX)}' cannot be null or empty.", nameof(rawX));
				}

				if (string.IsNullOrWhiteSpace(rawY))
				{
					throw new ArgumentException($"'{nameof(rawY)}' cannot be null or empty.", nameof(rawY));
				}

				if (string.IsNullOrWhiteSpace(rawOrientation))
				{
					throw new ArgumentException($"'{nameof(rawOrientation)}' cannot be null or empty.", nameof(rawOrientation));
				}

				// NULL CHECKS DONE; DECLARING VALUES FOR CASTING ATTEMPTS
				int x;
				int y;
				RobotOrientation orientation;

				// validate X, Y coordinates as valid integers
				if (!int.TryParse(rawX, out x))
				{
					throw new ArgumentException($"A robot's X coordinate, '{rawX}', cannot be parsed into an integer.", nameof(rawX));
				}

				if (!int.TryParse(rawY, out y))
				{
					throw new ArgumentException($"A robot's Y coordinate, '{rawY}', cannot be parsed into an integer.", nameof(rawY));
				}

				// validate X, Y coordinates as being within the maximum range defined by the requirements document
				if (x > MaxCoordinateValue)
				{
					throw new ArgumentException($"A robot's X coordinate, '{x}', is above the maximum ('{MaxCoordinateValue}').", nameof(rawX));
				}

				if (y > MaxCoordinateValue)
				{
					throw new ArgumentException($"A robot's Y coordinate, '{y}', is above the maximum ('{MaxCoordinateValue}').", nameof(rawY));
				}

				// validate X, Y coordinates as being within Mars
				if (x < 0 || x >= MarsState.Surface.GetLength(0))
				{
					throw new ArgumentException($"A robot's X coordinate, '{x}', is outside of Mars.", nameof(rawX));
				}

				if (y < 0 || y >= MarsState.Surface.GetLength(1))
				{
					throw new ArgumentException($"A robot's Y coordinate, '{y}', is outside of Mars.", nameof(rawY));
				}

				// validate the supplied orientation
				if (!Enum.TryParse(rawOrientation, out orientation))
				{
					throw new ArgumentException($"A robot's orientation, '{rawOrientation}', cannot be parsed into a valid orientation ('N', 'S', 'E', 'W').", nameof(rawOrientation));
				}

				// EVERYTHING IS VALID; COMPOSE THE FINAL STRUCT
				this.X = x;
				this.Y = y;
				this.Orientation = orientation;
				this.Lost = false;
				this.MovementsMade = 0;

				// ...and set the cell as walked
				MarsState.Surface[x, y].HasBeenWalked = true;

				// ...and add the robot to the list, to compose final statistics later on
				if (MarsState.Robots == null) MarsState.Robots = new();
				MarsState.Robots.Add(this);
			}

			/// <summary>
			/// Calculates a new robot position, based on its previous position and a movement sequence.
			/// </summary>
			/// <param name="mars">A reference to the surface of Mars.</param>
			/// <param name="rawMovementSequence">A raw movement sequence, comprised of left (L), forward (F), and right (R) movements.</param>
			public void Move(string rawMovementSequence)
			{
				// NULL CHECKING THE INPUT PARAMETERS (not checking for empty, since it's possible that a robot makes no movements)
				if (rawMovementSequence == null)
				{
					throw new ArgumentException($"'{nameof(rawMovementSequence)}' cannot be null.", nameof(rawMovementSequence));
				}

				// CHECKING MAXIMUM MOVEMENT SEQUENCE MAX LENGTH, ACCORDING TO SPECS
				if (rawMovementSequence.Length > MaxRawMovementCommandLength)
				{
					throw new ArgumentException($"'{nameof(rawMovementSequence)}' had a length of {rawMovementSequence.Length}; the maximum is {MaxRawMovementCommandLength}.", nameof(rawMovementSequence));
				}

				// PROCESS EACH INDIVIDUAL MOVEMENT IN THE SEQUENCE
				for (int i = 0; i <= rawMovementSequence.Length - 1; i++)
				{
					// if we're already lost, go no further
					if (this.Lost) break;

					// first, validate the current movement command
					string rawCurrentMovement = rawMovementSequence[i].ToString();
					MovementCommand currentMovement;

					if (!Enum.TryParse(rawCurrentMovement, out currentMovement))
					{
						throw new ArgumentException($"The movement at index '{i}' ('{rawCurrentMovement}'), in the movement sequence '{rawMovementSequence}', isn't valid.", nameof(rawMovementSequence));
					}

					// PERFORM THE MOVEMENT
					switch (currentMovement)
					{
						// rotate left
						case MovementCommand.L:
							switch (this.Orientation)
							{
								case RobotOrientation.W:
									this.Orientation = RobotOrientation.S;
									break;
								case RobotOrientation.E:
									this.Orientation = RobotOrientation.N;
									break;
								case RobotOrientation.S:
									this.Orientation = RobotOrientation.E;
									break;
								case RobotOrientation.N:
									this.Orientation = RobotOrientation.W;
									break;
							}
							break;
						// rotate right
						case MovementCommand.R:
							switch (this.Orientation)
							{
								case RobotOrientation.W:
									this.Orientation = RobotOrientation.N;
									break;
								case RobotOrientation.E:
									this.Orientation = RobotOrientation.S;
									break;
								case RobotOrientation.S:
									this.Orientation = RobotOrientation.W;
									break;
								case RobotOrientation.N:
									this.Orientation = RobotOrientation.E;
									break;
							}
							break;
						// move forward
						case MovementCommand.F:
							switch (this.Orientation)
							{
								case RobotOrientation.W:
									// check to see if we will fall when moving
									if (this.X - 1 < 0)
									{
										// if the current tile has a warning scent, we do nothing
										if (MarsState.Surface[this.X, this.Y].HasRobotScent)
										{
											continue;
										}

										// else, we are lost :(
										// set scent
										MarsState.Surface[this.X, this.Y].HasRobotScent = true;

										// set robot as lost
										this.Lost = true;

										// don't process any further movement, robot's lost :(
										continue;
									}

									// make the move
									this.X -= 1;

									// set the cell as walked
									MarsState.Surface[this.X, this.Y].HasBeenWalked = true;

									break;

								case RobotOrientation.E:
									// check to see if we will fall when moving
									if (this.X + 1 >= MarsState.Surface.GetLength(0))
									{
										// if the current tile has a warning scent, we do nothing
										if (MarsState.Surface[this.X, this.Y].HasRobotScent)
										{
											continue;
										}

										// else, we are lost :(
										// set scent
										MarsState.Surface[this.X, this.Y].HasRobotScent = true;

										// set robot as lost
										this.Lost = true;

										// don't process any further movement, robot's lost :(
										continue;
									}

									// make the move
									this.X += 1;

									// set the cell as walked
									MarsState.Surface[this.X, this.Y].HasBeenWalked = true;

									break;

								case RobotOrientation.S:
									// check to see if we will fall when moving
									if (this.Y - 1 < 0)
									{
										// if the current tile has a warning scent, we do nothing
										if (MarsState.Surface[this.X, this.Y].HasRobotScent)
										{
											continue;
										}

										// else, we are lost :(
										// set scent
										MarsState.Surface[this.X, this.Y].HasRobotScent = true;

										// set robot as lost
										this.Lost = true;

										// don't process any further movement, robot's lost :(
										continue;
									}

									// make the move
									this.Y -= 1;

									// set the cell as walked
									MarsState.Surface[this.X, this.Y].HasBeenWalked = true;

									break;

								case RobotOrientation.N:

									// check to see if we will fall when moving
									if (this.Y + 1 >= MarsState.Surface.GetLength(1))
									{
										// if the current tile has a warning scent, we do nothing
										if (MarsState.Surface[this.X, this.Y].HasRobotScent)
										{
											continue;
										}

										// else, we are lost :(
										// set scent
										MarsState.Surface[this.X, this.Y].HasRobotScent = true;

										// set robot as lost
										this.Lost = true;

										// don't process any further movement, robot's lost :(
										continue;
									}

									// make the move, if there's no scent
									this.Y += 1;

									// set the cell as walked
									MarsState.Surface[this.X, this.Y].HasBeenWalked = true;

									break;
							}
							this.MovementsMade++;
							break;
					}
					//Console.WriteLine($"{i + 1}. ({rawCurrentMovement}) {this}");
				}
			}

			public override string ToString()
			{
				string coordX = X.ToString();
				string coordY = Y.ToString();
				string orientation = Enum.GetName(typeof(RobotOrientation), Orientation);
				string lost = Lost ? " LOST" : "";

				return $"{coordX} {coordY} {orientation}{lost}";
			}
		}
	}
}
