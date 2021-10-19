using MartianRobotsLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MartianRobotsScriptlike
{
	class Program
	{
		// bla.exe "C:\inputFolder" "C:\outputFolder"
		public static async Task Main(string[] args)
		{
			// validate input and output folder as valid paths
			string inputPath = Path.GetFullPath(args[0]);
			string outputPath = Path.GetFullPath(args[1]);

			// validate that the input folder exists
			if (!Directory.Exists(inputPath)) throw new ArgumentException($"The input path, '{inputPath}', doesn't exist.");

			// create the output folder if it doesn't exist
			if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

			// find every textfile in the input folder
			string [] textFiles = Directory.GetFiles(inputPath, "*.txt");

			// process every file asynchronously
			List<Task> processingTasks = new();

			foreach (string textFile in textFiles)
			{
				processingTasks.Add(ProcessTextFile(textFile, outputPath));
			}

			await Task.WhenAll(processingTasks);
		}

		public static async Task ProcessTextFile(string filePath, string outputPath)
		{
			// read text asynchronously
			string[] text = await File.ReadAllLinesAsync(filePath);

			if (text.Length == 0) return;

			// initialize the Mars business logic object
			MarsState Mars = new MarsState();

			// process first line (Mars size)
			string[] sizeValues = text[0].Split(' ');
			Mars.CreateSurface(sizeValues[0], sizeValues[1]);

			// start processing robot positions and movement commands
			MarsState.RobotState robotState = null;
			List<string> finalPositions = new();

			for (int i = 1; i < text.Length; i++)
			{
				// odd lines are starting positions
				if (i % 2 != 0)
				{
					string[] robotInitialState = text[i].Split(' ');

					if (robotInitialState.Length != 3)
					{
						throw new ArgumentException($"[Line {i}] Three values separated by a whitespace were expected. Got {robotInitialState.Length} value(s) instead.");
					}

					robotState = new MarsState.RobotState(Mars, robotInitialState[0], robotInitialState[1], robotInitialState[2]);
				}

				// even lines are movement commands
				if (i % 2 == 0)
				{
					robotState.Move(text[i]);
					finalPositions.Add(robotState.ToString());
				}
			}

			string finalPositionsText = string.Join(Environment.NewLine, finalPositions);
			string finalPositionsFilename = $"{Path.GetFileNameWithoutExtension(filePath)}_output.txt";
			string finalPositionsFullPath = Path.Combine(outputPath, finalPositionsFilename);

			await File.WriteAllTextAsync(finalPositionsFullPath, finalPositionsText);
		}
	}
}
