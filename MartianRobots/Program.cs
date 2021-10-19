using MartianRobotsLib;
using System;
using System.Linq;

namespace MartianRobots
{
	class Program
	{
		private static MarsState Mars;

		static void Main(string[] args)
		{
			// initialize Mars data
			Program.Mars = new MarsState();

			// show a greeting message for the user
			DisplayGreetingMessage();

			// ask the user for the initial Mars setup data (valid width and height)
			AskForMarsSetupData();

			// await initial robot coordinates and movement sequence
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine("Excellent! It's now time to move some little robots around.");
			Console.ForegroundColor = ConsoleColor.White;
			AskForNewRobotInput();

			// await possible additional robot coordinates and movement sequences
			bool askForNewInput = true;
			while (askForNewInput)
			{
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.WriteLine("Do you wish to input another robot's starting position and movement commands?");
				Console.WriteLine("Type 'y' to do so, or any other key to view the final results.");
				Console.ForegroundColor = ConsoleColor.White;

				ConsoleKeyInfo typedKey = Console.ReadKey();

				if (typedKey.KeyChar == 'y' || typedKey.KeyChar == 'Y')
				{
					Console.WriteLine("");
					AskForNewRobotInput();
				}
				else
				{
					askForNewInput = false;
				}
			}

			ShowFinalStatistics();

			Console.WriteLine("Press any key to exit.");
			ConsoleKeyInfo wait = Console.ReadKey();
		}

		private static void ShowFinalStatistics()
		{
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine("");
			Console.WriteLine("Well, the exploration mission is done! Here's a summary of everything that happened:");
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.White;

			// stats: lost robots ('2/3 alive'), total and percentage of tiles walked ('13/50, 25%')
			PrintMarsStats();
			Console.WriteLine("");

			// visible grid showing walked and scented tiles
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine("Here's a graphical grid showing the surface of Mars, and the robots' paths.");
			Console.WriteLine("W = a robot walked here");
			Console.WriteLine("! = a robot was lost here :(");
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine(MarsGrid());
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("");
		}

		private static void PrintMarsStats()
		{
			// print data and stats
			Console.WriteLine($"Total robots placed:		{Mars.AliveRobots + Mars.DeadRobots} robots");
			Console.WriteLine($"Alive robots:			{Mars.AliveRobots}/{Mars.AliveRobots + Mars.DeadRobots}");

			double tilesExploredPercentage = (double)Mars.WalkedTiles / (double)Mars.TotalTiles * 100;
			tilesExploredPercentage = Math.Round(tilesExploredPercentage, 2);

			Console.WriteLine($"Tiles explored:			{Mars.WalkedTiles}/{Mars.TotalTiles} ({tilesExploredPercentage}%)");
		}

		private static string MarsGrid()
		{
			string marsGrid = "	";

			for (int y = Mars.Surface.GetLength(1) - 1; y >= 0; y--)
			{
				for (int x = 0; x < Mars.Surface.GetLength(0); x++)
				{
					string newChar = "·";
					if (Mars.Surface[x, y].HasBeenWalked) newChar = "W";
					if (Mars.Surface[x, y].HasRobotScent) newChar = "!";

					marsGrid += newChar;
				}

				marsGrid += Environment.NewLine + "	";
			}

			return marsGrid;
		}

		private static void AskForMarsSetupData()
		{
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine("Please specify the size of the 2-dimentional terrain we'll be using, in the format 'X Y'.");
			Console.ForegroundColor = ConsoleColor.White;

			bool inputWasCorrect = false;

			while (!inputWasCorrect)
			{
				string sizeOfMars = Console.ReadLine();
				string[] sizeValues = sizeOfMars.Split(' ');

				try
				{
					if (sizeValues.Length != 2)
					{
						throw new ArgumentException($"Two values separated by a whitespace were expected. Got {sizeValues.Length} value(s) instead.");
					}

					Program.Mars.CreateSurface(sizeValues[0], sizeValues[1]);
					inputWasCorrect = true;
				}
				catch (Exception ex)
				{
					PrintException(ex);
				}
			}

			Console.WriteLine("");
		}

		private static void DisplayGreetingMessage()
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine(".     .       .  .   . .   .   . .    +  .");
			Console.WriteLine("  .     .  :     .    .. :. .___---------___.");
			Console.WriteLine("       .  .   .    .  :.:. _\".^ .^ ^.  '.. :\"-_. .");
			Console.WriteLine(@"    .  :       .  .  .:../:            . .^  :.:\.");
			Console.WriteLine(@"        .   . :: +. :.:/: .   .    .        . . .:\");
			Console.WriteLine(@" .  :    .     . _ :::/:               .  ^ .  . .:\");
			Console.WriteLine(@"  .. . .   . - : :.:./.                        .  .:\");
			Console.WriteLine(@"  .      .     . :..|:                    .  .  ^. .:|");
			Console.WriteLine(@"    .       . : : ..||        .                . . !:|");
			Console.WriteLine(@"  .     . . . ::. ::\(                           . :)/");
			Console.WriteLine(@" .   .     : . : .:.|. ######              .#######::|");
			Console.WriteLine(@"  :.. .  :-  : .:  ::|.#######           ..########:|");
			Console.WriteLine(@" .  .  .  ..  .  .. :\ ########          :######## :/");
			Console.WriteLine(@"  .        .+ :: : -.:\ ########       . ########.:/");
			Console.WriteLine(@"    .  .+   . . . . :.:\. #######       #######..:/");
			Console.WriteLine(@"      :: . . . . ::.:..:.\           .   .   ..:/");
			Console.WriteLine(@"   .   .   .  .. :  -::::.\.       | |     . .:/");
			Console.WriteLine("      .  :  .  .  .-:.\":.::.\\             ..:/");
			Console.WriteLine(@" .      -.   . . . .: .:::.:.\.           .:/");
			Console.WriteLine(@".   .   .  :      : ....::_:..:\   ___.  :/");
			Console.WriteLine(@"   .   .  .   .:. .. .  .: :.:.:\       :/");
			Console.WriteLine(@"     +   .   .   : . ::. :.:. .:.|\  .:/|");
			Console.WriteLine(@"     .         +   .  .  ...:: ..|  --.:|");
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine(@"######################################################");
			Console.WriteLine(@"################# Welcome to Mars! ###################");
			Console.WriteLine(@"######################################################");
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static void AskForNewRobotInput()
		{
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine("");
			Console.WriteLine("First, specify the starting coordinates in the format 'X Y' and an orientation (N, S, E, W) for the robot.");
			Console.WriteLine("For example: '3 2 S' for '3, 2, facing South'.");
			Console.ForegroundColor = ConsoleColor.White;

			MarsState.RobotState robotState = null;

			bool inputWasCorrect = false;

			while (!inputWasCorrect)
			{
				string robotStartingState = Console.ReadLine();
				string[] robotStartingStateValues = robotStartingState.Split(' ');

				try
				{
					if (robotStartingStateValues.Length != 3)
					{
						throw new ArgumentException($"Three values separated by a whitespace were expected. Got {robotStartingStateValues.Length} value(s) instead.");
					}

					robotState = new MarsState.RobotState(Mars, robotStartingStateValues[0], robotStartingStateValues[1], robotStartingStateValues[2]);
					inputWasCorrect = true;
				}
				catch (Exception ex)
				{
					PrintException(ex);
				}
			}

			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine("");
			Console.WriteLine("Well done! The robot is now placed on Mars.");
			Console.WriteLine("");
			Console.WriteLine("Next up: let's make it move. Please input a movement sequence.");
			Console.WriteLine("A movement sequence is a sequence of the letters 'F, 'L', and 'R'. They stand for 'forward', 'left', and 'right'.");
			Console.WriteLine("An example sequence would be 'FFFRLFRLF'.");
			Console.ForegroundColor = ConsoleColor.White;

			inputWasCorrect = false;
			while (!inputWasCorrect)
			{
				string movementCommands = Console.ReadLine();

				try
				{
					robotState.Move(movementCommands);
					inputWasCorrect = true;
				}
				catch (Exception ex)
				{
					PrintException(ex);
				}
			}

			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine("");
			Console.WriteLine("Movement applied. The ending position and orientation are as follows:");
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(robotState.ToString());
			Console.WriteLine("");
		}

		public static void PrintException(Exception exception)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("");
			Console.WriteLine($"Oops. That didn't look right. Please try again! Here are some details:");
			Console.WriteLine($"{exception.Message}");
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}
