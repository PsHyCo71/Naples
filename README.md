# 🧵 Naples 🇮🇹

![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![License](https://img.shields.io/badge/license-GPLv3-blue)
![Version](https://img.shields.io/badge/version-1.0.0-green)

Naples is a simple asynchronous runtime for C#, built as a learning project inspired by the Rust library Tokio. The name is a nod to the Italian city — and to Tokio itself, which is also named after a city.

## How does it work
- **Waker**: holds a callback and fires it once when signaled, telling the scheduler a task is ready;
- **Scheduler**: maintains a queue of tasks and runs them one by one — if a task is not completed it gets re-queued;
- **Runtime**: the public face of the library, exposes `Spawn()` to add tasks and `Run()` to start the executor loop;
- **Channel\<T\>**: a typed pipe between two tasks — one side sends values with `Send()`, the other receives them with `TryReceive()`;

## Example 
```csharp
using Naples.Core;
using Naples.Sync;

var runtime = new Runtime();
var channel = new Channel<string>();

// Task producer - sends a message
runtime.Spawn(() =>
{
    channel.Send("Hi from Naples");
    return Task.CompletedTask;
});

// Task consumer - receives a message
runtime.Spawn(() =>
{
    if (channel.TryReceive(out var message))
        Console.WriteLine(message); // "Hi from Naples!"

    return Task.CompletedTask;
});

// Runs everything
runtime.Run();
```
The runtime runs the loop, runs the producer, then the consumer and when the queue is empty it stops.

## Requirements
- Windows 10 or later
- .NET SDK 10.0 or later

## How to use 
To add **Naples** as a dependency in your C# project execute this command: 
```bash
dotnet add package Naples --version 1.0.0 # In the command prompt or powershell
```
Now in the `.csproj` file should appear this line:
```xml
<PackageReference Include="Naples" Version="1.0.0" />
```

## Limitations
- Single-threaded — all tasks run on one thread
- Not thread-safe
- No exception handling inside tasks
- No I/O support (no timers, no sockets)

## License
This project is licensed under the [GNU GPL v3.0](https://www.gnu.org/licenses/gpl-3.0.html)