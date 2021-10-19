using MartianRobotsLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MartianRobotsLib.MarsState;

namespace MartianRobotsRESTAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MartianSimulationController : ControllerBase
	{
		// dependency injection
		private readonly ILogger<MartianSimulationController> _logger;

		public MartianSimulationController(ILogger<MartianSimulationController> logger)
		{
			_logger = logger;
		}

		// endpoint definition
		[HttpPost]
		public IEnumerable<RobotResult> Post([FromBody] MartianSimulation body)
		{
			// initialize a new Mars business logic object
			MarsState Mars = new MarsState();

			// initialize the surface of Mars
			Mars.CreateSurface(body.MarsSize.X.ToString(), body.MarsSize.Y.ToString());

			// initalize the simulation result
			List<RobotResult> results = new();

			// start simulating
			MarsState.RobotState robotState = null;

			for (int i = 0; i < body.RobotCommands.Length; i++)
			{
				// calculate initial state
				robotState = new RobotState(Mars,
											body.RobotCommands[i].StartingPosition.X.ToString(),
											body.RobotCommands[i].StartingPosition.Y.ToString(),
											body.RobotCommands[i].StartingPosition.Orientation);

				// execute move commands
				robotState.Move(body.RobotCommands[i].MovementCommands);

				// gather results
				RobotResult robotResult = new();

				robotResult.FinalPosition = new();
				robotResult.FinalPosition.X = robotState.X;
				robotResult.FinalPosition.Y = robotState.Y;
				robotResult.FinalPosition.Orientation = Enum.GetName(typeof(RobotOrientation), robotState.Orientation);
				robotResult.Lost = robotState.Lost;

				results.Add(robotResult);
			}

			return results;
		}
	}
}
