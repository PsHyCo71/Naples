# 🧵 Naples 🇮🇹

![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![License](https://img.shields.io/badge/license-GPLv3-blue)
[![NuGet](https://img.shields.io/nuget/v/Naples.svg?color=green)](https://www.nuget.org/packages/Naples)

Naples is a simple asynchronous runtime for C#, built as a learning project inspired by the Rust library Tokio. The name is a nod to the Italian city — and to Tokio itself, which is also named after a city.

## Features
- Priority-based task scheduling
- Multi-threaded executor with work stealing
- Thread-safe `Channel<T>` for inter-task communication
- `CancellationToken` support on every task
- Centralized exception handling via `OnTaskFailed`
- TCP support via `Listener` and `Client`
- Timer via `Timer.Delay()`

## How does it work
- **Waker**: holds a callback and fires it once when signaled, telling the scheduler a task is ready;
- **Scheduler**: maintains a queue of tasks and runs them one by one — if a task is not completed it gets re-queued;
- **Runtime**: the public face of the library, exposes `Spawn()` to add tasks and `Run()` to start the executor loop;
- **Channel\<T\>**: a typed pipe between two tasks — one side sends values with `Send()`, the other receives them with `TryReceive()`;
- **Timer**: a timer that lets you delay task;
- **Work Stealing**: as soon as a queue is empty it searches for the longest queue and steals the last task in the list;
- **Listener**: a TCP server that accepts incoming connections;
- **Client**: a TCP client that connects to a server, reads and writes data; 

## Example 
```csharp
using Naples;

// Create a multi-threaded runtime with 4 threads
var runtime = new Runtime(4);
var channel = new Channel<string>();
var cts = new CancellationTokenSource();

// Handle task failures
runtime.OnTaskFailed += ex => Console.WriteLine($"Task failed: {ex.Message}");

// Task 1 - Producer (priority 1, runs first)
// Sends 3 messages through the channel then cancels the runtime
runtime.Spawn(async (token) =>
{
    string[] messages = ["Hello", "from", "Naples!"];
    foreach (var msg in messages)
    {
        channel.Send(msg);
        await Timer.Delay(500, token); // wait 500ms between messages
    }
    cts.Cancel(); // signal all tasks to stop
}, 1);

// Task 2 - Consumer (priority 2)
// Reads messages from the channel until cancelled
runtime.Spawn(async (token) =>
{
    while (!token.IsCancellationRequested)
    {
        if (channel.TryReceive(out var message))
            Console.WriteLine($"Received: {message}");

        await Timer.Delay(100, token);
    }
}, 2);

// Task 3 - TCP Server (priority 3)
// Accepts one connection, reads a message and prints it
runtime.Spawn(async (token) =>
{
    var listener = new Listener(9999);
    listener.Start();
    var client = await listener.AcceptClient(token);
    var data = await client.ReadAsync(token);
    Console.WriteLine($"TCP received: {data}");
}, 3);

// Task 4 - TCP Client (priority 4)
// Connects to the server and sends a message
runtime.Spawn(async (token) =>
{
    await Timer.Delay(200, token); // wait for server to start
    var client = await Client.ConnectAsync("localhost", 9999, token);
    await client.WriteAsync("Hi from TCP!", token);
}, 4);

// Run everything
runtime.Run(cts.Token);
```
This example creates a 4-thread runtime that runs four tasks concurrently. A **producer** sends three messages through a `Channel` with a 500ms delay between each one, then cancels the runtime when done. A **consumer** reads those messages from the channel in a loop until cancelled. Meanwhile, a **TCP server** listens on port 9999 and prints the first message it receives, and a **TCP client** connects to it after a short delay and sends a message. All tasks run with different priorities, share a `CancellationToken` to shut down cleanly, and any failure is caught by `OnTaskFailed`.

## Requirements
- .NET SDK 10.0 or later
- Windows 10 or later / macOS 12 or later / Linux (any modern distro)

## How to use 
To add **Naples** as a dependency in your C# project execute this command: 
```bash
dotnet add package Naples --version 2.0.0 # In the command prompt or powershell
```
Now in the `.csproj` file should appear this line:
```xml
<PackageReference Include="Naples" Version="2.0.0" />
```

## Limitations
- Async I/O tasks (e.g. TCP) must be run outside the scheduler

## License
This project is licensed under the [GNU GPL v3.0](https://www.gnu.org/licenses/gpl-3.0.html)