// ============================================================
// LESSON 9: Async/Await & Tasks — Deep Dive
// ============================================================

// ============================================
// PART 1: Synchronous vs Asynchronous
// ============================================

// First, let's see the SYNCHRONOUS (blocking) way
using System.Text.Json;
Console.WriteLine("=== SYNCHRONOUS EXAMPLE ===\n");

void MakeBreakfastSync()
{
    Console.WriteLine("Starting breakfast...");
    var sw = System.Diagnostics.Stopwatch.StartNew();

    // Each step BLOCKS until done — like one waiter doing everything sequentially
    BoilWaterSync();
    ToastBreadSync();
    FryEggSync();

    sw.Stop();
    Console.WriteLine($"\nBreakfast ready! Took {sw.ElapsedMilliseconds}ms");
    Console.WriteLine("(Notice: ~3 seconds because each step waited for the previous one)\n");
}

void BoilWaterSync()
{
    Console.WriteLine("  Boiling water...");
    Thread.Sleep(1000); // Simulates waiting — thread is BLOCKED, doing nothing
    Console.WriteLine("  Water boiled!");
}

void ToastBreadSync()
{
    Console.WriteLine("  Toasting bread...");
    Thread.Sleep(1000);
    Console.WriteLine("  Bread toasted!");
}

void FryEggSync()
{
    Console.WriteLine("  Frying egg...");
    Thread.Sleep(1000);
    Console.WriteLine("  Egg fried!");
}

MakeBreakfastSync();

// Now the ASYNCHRONOUS way
Console.WriteLine("=== ASYNCHRONOUS EXAMPLE ===\n");

async Task MakeBreakfastAsync()
{
    Console.WriteLine("Starting breakfast...");
    var sw = System.Diagnostics.Stopwatch.StartNew();

    // Start ALL tasks at once — like telling 3 appliances to go
    Task waterTask = BoilWaterAsync();
    Task toastTask = ToastBreadAsync();
    Task eggTask = FryEggAsync();

    // Now wait for ALL of them to finish
    await Task.WhenAll(waterTask, toastTask, eggTask);

    sw.Stop();
    Console.WriteLine($"\nBreakfast ready! Took {sw.ElapsedMilliseconds}ms");
    Console.WriteLine("(Notice: ~1 second because all 3 ran in parallel!)\n");
}

async Task BoilWaterAsync()
{
    Console.WriteLine("  Boiling water...");
    await Task.Delay(1000); // Non-blocking! Thread is FREE to do other work
    Console.WriteLine("  Water boiled!");
}

async Task ToastBreadAsync()
{
    Console.WriteLine("  Toasting bread...");
    await Task.Delay(1000);
    Console.WriteLine("  Bread toasted!");
}

async Task FryEggAsync()
{
    Console.WriteLine("  Frying egg...");
    await Task.Delay(1000);
    Console.WriteLine("  Egg fried!");
}

await MakeBreakfastAsync();


// ============================================
// PART 2: Task and Task<T> — What They Are
// ============================================
Console.WriteLine("=== PART 2: Task and Task<T> ===\n");

// Task = a promise that some work will complete (returns nothing)
// Task<T> = a promise that some work will complete AND return a value of type T
// Think of it like: Task is void, Task<T> is a return type

async Task<string> FetchUserNameAsync(int userId)
{
    Console.WriteLine($"  Fetching user {userId}...");
    await Task.Delay(500); // Simulate API call
    return userId switch
    {
        1 => "Santhosh",
        2 => "Aki",
        3 => "Claude",
        _ => "Unknown"
    };
}

async Task<int> FetchUserAgeAsync(int userId)
{
    Console.WriteLine($"  Fetching age for user {userId}...");
    await Task.Delay(300);
    return userId switch
    {
        1 => 28,
        2 => 35,
        3 => 2,
        _ => 0
    };
}

// You can await to get the VALUE out of a Task<T>
string name = await FetchUserNameAsync(1);
int age = await FetchUserAgeAsync(1);
Console.WriteLine($"  Result: {name}, age {age}\n");

// Or run them in parallel and await both
Console.WriteLine("  Fetching name and age in parallel...");
Task<string> nameTask = FetchUserNameAsync(2);  // Starts immediately
Task<int> ageTask = FetchUserAgeAsync(2);        // Also starts immediately

// await both — total time = max(500ms, 300ms) = 500ms, not 800ms
await Task.WhenAll(nameTask, ageTask);
Console.WriteLine($"  Result: {nameTask.Result}, age {ageTask.Result}\n");


// ============================================
// PART 3: The Rules of Async/Await
// ============================================
Console.WriteLine("=== PART 3: The Rules ===\n");

// RULE 1: "async" goes on the method signature
// RULE 2: "await" goes before ANY async call you want to wait for
// RULE 3: An async method MUST return Task, Task<T>, or void (avoid void!)
// RULE 4: You can only use "await" inside an "async" method

// WHY avoid async void? Because you can't:
// - await it
// - catch its exceptions
// - know when it's done
// The ONLY valid use of async void is event handlers (WPF/WinForms buttons)

// BAD — Don't do this:
// async void DoSomethingBad() { await Task.Delay(100); }

// GOOD — Do this:
async Task DoSomethingGood()
{
    await Task.Delay(100);
    Console.WriteLine("  This is properly awaitable!");
}
await DoSomethingGood();

// RULE 5: If you don't await a Task, it runs as "fire and forget"
// This is DANGEROUS because exceptions are silently swallowed
Console.WriteLine("  Rules understood!\n");


// ============================================
// PART 4: Real Patterns You'll Use
// ============================================
Console.WriteLine("=== PART 4: Real-World Patterns ===\n");

// --- Pattern 1: Sequential async (when order matters) ---
Console.WriteLine("--- Pattern 1: Sequential async ---");
async Task ProcessOrderAsync()
{
    string user = await FetchUserNameAsync(1);  // Must get user first
    Console.WriteLine($"  Processing order for {user}");
    await Task.Delay(200);                       // Then process
    Console.WriteLine("  Order processed!");
}
await ProcessOrderAsync();

// --- Pattern 2: Parallel async (when independent) ---
Console.WriteLine("\n--- Pattern 2: Parallel async ---");
async Task FetchAllUsersAsync()
{
    // Start all fetches at once
    var tasks = new List<Task<string>>
    {
        FetchUserNameAsync(1),
        FetchUserNameAsync(2),
        FetchUserNameAsync(3)
    };

    string[] names = await Task.WhenAll(tasks);
    Console.WriteLine($"  All users: {string.Join(", ", names)}");
}
await FetchAllUsersAsync();

// --- Pattern 3: WhenAny (first one wins) ---
Console.WriteLine("\n--- Pattern 3: WhenAny (race condition) ---");
async Task<string> FastServerAsync()
{
    await Task.Delay(100);
    return "Fast server responded!";
}

async Task<string> SlowServerAsync()
{
    await Task.Delay(2000);
    return "Slow server responded!";
}

Task<string> fast = FastServerAsync();
Task<string> slow = SlowServerAsync();
Task<string> winner = await Task.WhenAny(fast, slow);
Console.WriteLine($"  Winner: {await winner}");  // Fast server wins!

// --- Pattern 4: Timeout pattern ---
Console.WriteLine("\n--- Pattern 4: Timeout ---");
async Task<string> SlowOperationAsync()
{
    await Task.Delay(5000); // Takes 5 seconds
    return "Done!";
}

async Task<string> WithTimeoutAsync(Task<string> operation, int timeoutMs)
{
    Task timeoutTask = Task.Delay(timeoutMs);
    Task completedTask = await Task.WhenAny(operation, timeoutTask);

    if (completedTask == timeoutTask)
        throw new TimeoutException($"Operation timed out after {timeoutMs}ms");

    return await (Task<string>)completedTask;
}

try
{
    string result = await WithTimeoutAsync(SlowOperationAsync(), 1000);
    Console.WriteLine($"  {result}");
}
catch (TimeoutException ex)
{
    Console.WriteLine($"  Caught: {ex.Message}");
}


// ============================================
// PART 5: CancellationToken — Graceful Shutdown
// ============================================
Console.WriteLine("\n=== PART 5: CancellationToken ===\n");

// In plugins, you NEED this. When the user closes the app or switches profiles,
// your plugin must stop its async work cleanly — not leave zombie tasks running.

async Task LongRunningPluginWorkAsync(CancellationToken cancellationToken)
{
    for (int i = 1; i <= 10; i++)
    {
        // Check if cancellation was requested
        cancellationToken.ThrowIfCancellationRequested();

        Console.WriteLine($"  Processing item {i}/10...");
        await Task.Delay(200, cancellationToken); // Also accepts token!
    }
    Console.WriteLine("  All items processed!");
}

// Create a token source — the thing that triggers cancellation
using var cts = new CancellationTokenSource();

// Cancel after 500ms (simulating user closing the app)
cts.CancelAfter(500);

try
{
    await LongRunningPluginWorkAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("  Work was cancelled gracefully! (This is expected)\n");
}


// ============================================
// PART 6: Under the Hood — State Machine
// ============================================
Console.WriteLine("=== PART 6: What 'await' Actually Does ===\n");

// When the compiler sees "async/await", it rewrites your method into a STATE MACHINE.
//
// Your code:
//   async Task DoWork()
//   {
//       Console.WriteLine("Before");
//       await Task.Delay(1000);
//       Console.WriteLine("After");
//   }
//
// Compiler turns it into roughly:
//   class DoWork_StateMachine
//   {
//       int state = 0;
//       void MoveNext()
//       {
//           switch (state)
//           {
//               case 0:
//                   Console.WriteLine("Before");
//                   state = 1;
//                   // Schedule Task.Delay, RETURN the thread to the pool
//                   break;
//               case 1:
//                   // Task.Delay completed, we resume here
//                   Console.WriteLine("After");
//                   break;
//           }
//       }
//   }
//
// KEY INSIGHT: The thread is NOT blocked. It's returned to the thread pool.
// When the I/O completes, a (possibly different) thread picks up where you left off.
//
// This is why async scales: 1000 concurrent HTTP requests don't need 1000 threads.
// They need maybe 4-8 threads taking turns.

Console.WriteLine("  await = 'pause here, free the thread, resume when ready'");
Console.WriteLine("  It does NOT create a new thread!");
Console.WriteLine("  It does NOT spin-wait or poll!");
Console.WriteLine("  It's compiler magic — a state machine.\n");


// ============================================
// PART 7: Common Mistakes
// ============================================
Console.WriteLine("=== PART 7: Common Mistakes to Avoid ===\n");

// MISTAKE 1: Using .Result or .Wait() — DEADLOCK RISK!
// BAD:  string name = FetchUserNameAsync(1).Result;  // Can deadlock!
// GOOD: string name = await FetchUserNameAsync(1);
Console.WriteLine("  Mistake 1: Never use .Result or .Wait() — use await instead");

// MISTAKE 2: async void (except event handlers)
// BAD:  async void HandleClick() { ... }  // Exceptions vanish!
// GOOD: async Task HandleClickAsync() { ... }
Console.WriteLine("  Mistake 2: Never use async void (except UI event handlers)");

// MISTAKE 3: Not passing CancellationToken
// Your plugin WILL need to cancel work. Always accept and pass tokens.
Console.WriteLine("  Mistake 3: Always accept and pass CancellationToken");

// MISTAKE 4: Thread.Sleep in async code
// BAD:  Thread.Sleep(1000);   // BLOCKS the thread!
// GOOD: await Task.Delay(1000); // Frees the thread!
Console.WriteLine("  Mistake 4: Use Task.Delay, not Thread.Sleep in async code");

// MISTAKE 5: Forgetting to await
// BAD:  DoSomethingAsync();       // Fire and forget — exceptions lost!
// GOOD: await DoSomethingAsync(); // Properly awaited
Console.WriteLine("  Mistake 5: Always await your async calls\n");


// ============================================
// PRACTICE EXERCISES
// ============================================
Console.WriteLine("=== PRACTICE TIME ===\n");
Console.WriteLine("Try these exercises (uncomment one at a time):\n");

// EXERCISE 1: Parallel Web Fetcher
//Create 3 async methods that simulate fetching data from different APIs
async Task<Dictionary<string, string>> StudentRankAsync()
{
    await Task.Delay(100); // Simulate fetching student rank from school API
    return new Dictionary<string, string> { ["rank"] = "1st", ["name"] = "Santhosh" };
}
async Task<Dictionary<string, string>> WeatherAsync()
{
    await Task.Delay(500); // Simulate fetching weather data

    return new Dictionary<string, string> { ["temp"] = "30°C", ["condition"] = "Sunny" };
}       
async Task<Dictionary<string, string>> NewsAsync()
{
    await Task.Delay(200); // Simulate fetching news headlines
    return new Dictionary<string, string> { ["headline"] = "C# Async/Await Mastered!", ["source"] = "Tech News" };
}

// (weather, news, stocks). Each takes a different time.
// Fetch all 3 in parallel and print results.
async Task FetchAllDataAsync()
{
    var studentTask = StudentRankAsync();
    var weatherTask = WeatherAsync();
    var newsTask = NewsAsync();

    await Task.WhenAll(studentTask, weatherTask, newsTask);

    Console.WriteLine($"Student: {studentTask.Result["name"]} is ranked {studentTask.Result["rank"]}");
    Console.WriteLine($"Weather: {weatherTask.Result["temp"]}, {weatherTask.Result["condition"]}");
    Console.WriteLine($"News: \"{newsTask.Result["headline"]}\" from {newsTask.Result["source"]}");
}
await FetchAllDataAsync();
// Target: total time should be max(individual times), not sum.

// EXERCISE 2: Retry Pattern
// Write an async method that simulates a flaky API (fails 2 out of 3 times).
// Write a RetryAsync wrapper that retries up to 3 times with increasing delay
// (100ms, 200ms, 400ms — exponential backoff).

Random random = new Random();
async Task<string> FlakyApiAsync()
{
    await Task.Delay(100); // Simulate API call
    if (random.Next(3) < 2) // 66% chance to fail (0 or 1 out of 0,1,2)
        throw new Exception("Flaky API failed!");
    return "Success!";
}   
async Task<string> RetryAsync(Func<Task<string>> operation, int maxRetries)
{
    int delay = 100;
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await operation();
        }
        catch
        {
            if (attempt == maxRetries)
                throw; // Rethrow if it's the last attempt
            Console.WriteLine($"Attempt {attempt} failed. Retrying in {delay}ms...");
            await Task.Delay(delay);
            delay *= 2; // Exponential backoff
        }
    }
    return null!; // Unreachable, but required for compilation
}
try
{
    string result = await RetryAsync(FlakyApiAsync, 3);
    Console.WriteLine($"Final result: {result}");
}
catch (Exception ex)
{
    Console.WriteLine($"All retries failed: {ex.Message}");
}

// EXERCISE 3: Plugin Event Listener
// Simulate a plugin that:
// - Listens for "device events" in a loop (use Task.Delay to simulate)
// - Processes each event asynchronously
// - Supports CancellationToken to stop listening
// - Logs "Listening...", "Event received: dial_turn", "Processing...", etc.
// Cancel after 2 seconds.

async Task deviceEventListenerAsync(CancellationToken cancellationToken)
{
    int eventCount = 0;
    while (!cancellationToken.IsCancellationRequested)
    {
        Console.WriteLine("Listening for device events...");
        await Task.Delay(500, cancellationToken); // Simulate waiting for an event

        // Simulate receiving an event
        string deviceEvent = $"dial_turn_{++eventCount}";
        Console.WriteLine($"Event received: {deviceEvent}");

        // Process the event asynchronously
        await Task.Delay(300, cancellationToken); // Simulate processing time
        Console.WriteLine($"Processed event: {deviceEvent}\n");
    }
    Console.WriteLine("Stopped listening for device events.");
}
using var cts2 = new CancellationTokenSource();
cts2.CancelAfter(2000); // Stop after 2 seconds
try
{
    await deviceEventListenerAsync(cts2.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Event listener cancelled gracefully.");      
}

Console.WriteLine("Exercises are described in the comments above.");
Console.WriteLine("Pick one and implement it below!");


