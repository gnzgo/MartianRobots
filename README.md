# MartianRobots
A series of cross-platform projects, all of them targeting the latest .NET 5 release, regarding robots, Mars, and very dangerous scents.

🤖🪐

This solution is comprised of 5 smaller, individual projects:
1. **MartianRobotsLib**. Contains the core business logic of the Mars surface, navigation, and robot behavior.
2. **MartianRobotsRESTAPI**. A fully Dockerized, Swagger-enabled ASP.NET Web API that offers networked access to the core library functionality.
3. **MartianRobotsScriptlike**. A multi-threaded, 100% asynchronous CLI application to process batches of text files in bulk.
4. **MartianRobotsCLI**. An interactive graphical command-line application that enables any non-technical user to interact with the core business logic of this project.
5. **MartianRobotsTests**. An xUnit project with around 30 tests that cover the original specs document, and every other bit of functionality offered by the core library.

## MartianRobotsLib
A .NET 5 class library containing the main functionality and business logic that every other project uses behind the scenes. 

Referenced by every other project in this solution, it instances, validates, and processes the Mars surface, its state, robot movement, and robot loss. Also supports some additional interesting statistics, such as the percentage of the surface that's been explored by the robots.

## MartianRobotsRESTAPI
An ASP.NET Web API that exposes the core library functionality for networked access via a REST API. It's fully Dockerized, and supports Swagger for in-browser API  testing and detailed JSON body definitions and responses.

![](https://i.imgur.com/ck683XU.png)

The exposed endpoint would be `/MartianSimulation/` expecting a POST request with initial Mars parameters, initial robot positions, and move commands, as outlined by the specs document.

The Swagger endpoint can be accessed at `/swagger/index.html`.

Here's an example request, and its response, using the sample inputs and outputs detailed in the specs document:

<details>
  
  <summary>Request (click to expand)</summary>
  
```json
{
   "marsSize":{
      "x":5,
      "y":3
   },
   "robotCommands": [
      {
         "startingPosition": {
            "x": 1,
            "y": 1,
            "orientation": "E"
         },
         "movementCommands": "RFRFRFRF"
      },
      {
         "startingPosition": {
            "x": 3,
            "y": 2,
            "orientation": "N"
         },
         "movementCommands": "FRRFLLFFRRFLL"
      },
      {
         "startingPosition": {
            "x": 0,
            "y": 3,
            "orientation": "W"
         },
         "movementCommands": "LLFFFRFLFL"
      }
   ]
}
```
  
</details>

<details>
  
  <summary>Response (click to expand)</summary>
  
```json
[
  {
    "finalPosition": {
      "x": 1,
      "y": 1,
      "orientation": "E"
    },
    "lost": false
  },
  {
    "finalPosition": {
      "x": 3,
      "y": 3,
      "orientation": "N"
    },
    "lost": true
  },
  {
    "finalPosition": {
      "x": 4,
      "y": 2,
      "orientation": "N"
    },
    "lost": false
  }
]
```
  
</details>

## MartianRobotsScriptlike
A very efficient, fully multi-threaded and asynchronous CLI application designed with large workloads in mind.

It can batch-process thousands of files in parallel in less than one second.

It accepts two arguments: an input folder, and an output folder, wrapped in double quotation marks. It will look for any text files that conform to the structure detailed in the specs document, and will output text files in the output folder with the results of executing the moves detailed in those files.

#### Example call

`C:\MartianRobotsScriptlike.exe "C:\inputFolder" "C:\outputFolder"`

#### Example input

<details>
  
  <summary>test.txt (click to expand)</summary>
  
```
5 3
1 1 E
RFRFRFRF
3 2 N
FRRFLLFFRRFLL
0 3 W
LLFFFRFLFL
```
  
</details>

#### Example output


<details>
  
  <summary>test_output.txt (click to expand)</summary>
  
```
1 1 E
3 3 N LOST
4 2 N
```
  
</details>

## MartianRobotsCLI
An interactive command-line tool that allows any non-technical user to interact with the business logic of the project in a graphical, user-friendly way, with clearly defined steps, detailed validation errors, and a final graphical grid detailing the surface of Mars, the paths that each robot took, and the spots where robots jumped off a cliff and were sadly lost.

It simply needs to be run as a regular executable file. The instructions are simple to follow and provided within the application itself.

![](https://i.imgur.com/h8uj6qw.png)

## MartianRobotsTests

A test suite based on xUnit, which is integrated in Visual Studio without any need for additional installs, configuration, or external packages.

It contains about 30 tests that cover the whole main core functionality and logic, including valid/invalid inputs, correct behavior, the sample inputs and outputs provided by the specs document, and others.

![](https://i.imgur.com/6bSzoiZ.png)
