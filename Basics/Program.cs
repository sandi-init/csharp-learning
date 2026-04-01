// ============================================================
// LESSON 10: Exception Handling Patterns
// ============================================================
using System.Text.Json;

// ============================================
// PART 1: Basics — try/catch/finally
// ============================================
Console.WriteLine("=== PART 1: try/catch/finally ===\n");

// try   = "attempt this code"
// catch = "if it fails, handle it here"
// finally = "run this NO MATTER WHAT (success or failure)"

try
{
    int[] numbers = { 1, 2, 3 };
    Console.WriteLine($"  Accessing index 10...");
    int value = numbers[10]; // throws IndexOutOfRangeException
    Console.WriteLine("  This line never runs");
}
catch (IndexOutOfRangeException ex)
{
    Console.WriteLine($"  Caught: {ex.GetType().Name} — {ex.Message}");
}
finally
{
    Console.WriteLine("  Finally block: always runs (cleanup goes here)\n");
}

// Multiple catch blocks — order matters (most specific FIRST)
try
{
    string? input = null;
    int length = input!.Length; // throws NullReferenceException
}
catch (NullReferenceException ex)
{
    Console.WriteLine($"  Caught specific: {ex.GetType().Name}");
}
catch (Exception ex)
{
    // This catches EVERYTHING — always put this LAST
    Console.WriteLine($"  Caught general: {ex.Message}");
}

Console.WriteLine();


// ============================================
// PART 2: Exception Hierarchy
// ============================================
Console.WriteLine("=== PART 2: Exception Hierarchy ===\n");

// All exceptions inherit from System.Exception
//
//   Exception
//   ├── SystemException (runtime errors)
//   │   ├── NullReferenceException
//   │   ├── IndexOutOfRangeException
//   │   ├── InvalidOperationException
//   │   ├── ArgumentException
//   │   │   ├── ArgumentNullException
//   │   │   └── ArgumentOutOfRangeException
//   │   ├── DivideByZeroException
//   │   ├── IOException
//   │   │   ├── FileNotFoundException
//   │   │   └── DirectoryNotFoundException
//   │   └── NotImplementedException
//   ├── ApplicationException (your custom exceptions — legacy, prefer Exception directly)
//   └── AggregateException (wraps multiple exceptions — async!)

// Catching a parent catches ALL children
try
{
    throw new ArgumentNullException("userId", "User ID cannot be null");
}
catch (ArgumentException ex)
{
    // This catches ArgumentNullException AND ArgumentOutOfRangeException
    // because they inherit from ArgumentException
    Console.WriteLine($"  Caught ArgumentException (or child): {ex.GetType().Name}");
    Console.WriteLine($"  Message: {ex.Message}\n");
}


// ============================================
// PART 3: throw vs throw ex — Critical Difference
// ============================================
Console.WriteLine("=== PART 3: throw vs throw ex ===\n");

void DeeplyNestedMethod()
{
    throw new InvalidOperationException("Something broke deep inside");
}

void MiddleMethod()
{
    DeeplyNestedMethod();
}

// BAD — throw ex resets the stack trace
try
{
    try
    {
        MiddleMethod();
    }
    catch (Exception ex)
    {
        throw ex; // ❌ Stack trace starts HERE, you lose where it actually happened
    }
}
catch (Exception ex)
{
    Console.WriteLine("  throw ex — Stack trace (truncated, original location LOST):");
    Console.WriteLine($"  {ex.StackTrace?.Split('\n')[0]}\n");
}

// GOOD — throw preserves the original stack trace
try
{
    try
    {
        MiddleMethod();
    }
    catch (Exception)
    {
        throw; // ✅ Stack trace preserved — you can see DeeplyNestedMethod
    }
}
catch (Exception ex)
{
    Console.WriteLine("  throw — Stack trace (original location PRESERVED):");
    Console.WriteLine($"  {ex.StackTrace?.Split('\n')[0]}\n");
}

// RULE: Always use "throw;" not "throw ex;" when re-throwing
Console.WriteLine("  RULE: Use 'throw;' to preserve stack trace, never 'throw ex;'\n");


// ============================================
// PART 4: Exception Filters (when)
// ============================================
Console.WriteLine("=== PART 4: Exception Filters (when keyword) ===\n");

// C# 6+ feature: filter exceptions WITHOUT catching them

HttpClient client = new HttpClient();

async Task<string> FetchWithFilterAsync(string url)
{
    try
    {
        return await client.GetStringAsync(url);
    }
    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        Console.WriteLine("  404 Not Found — resource doesn't exist");
        return "not_found";
    }
    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
        Console.WriteLine("  401 Unauthorized — need to authenticate");
        return "unauthorized";
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"  Other HTTP error: {ex.StatusCode} — {ex.Message}");
        return "error";
    }
}

// Test with a URL that returns 404
string result = await FetchWithFilterAsync("https://jsonplaceholder.typicode.com/users/99999");
Console.WriteLine($"  Result: {result}");

// Why filters are better than catching + checking inside:
//
// BAD:
//   catch (HttpRequestException ex)
//   {
//       if (ex.StatusCode == 404) { ... }
//       else throw;  // re-throw — but you already "caught" it (debugger stops here)
//   }
//
// GOOD (with when):
//   catch (HttpRequestException ex) when (ex.StatusCode == 404)
//   { ... }
//   // If condition is false, exception passes through WITHOUT being caught
//   // Debugger only stops if the filter matches — much better debugging!
Console.WriteLine();


// ============================================
// PART 5: Custom Exceptions
// ============================================
Console.WriteLine("=== PART 5: Custom Exceptions ===\n");

// When to create custom exceptions:
// - When callers need to handle YOUR errors differently from system errors
// - When you need to carry extra data with the exception

// For your plugin:
// - PluginConnectionException (IPC failed)
// - DeviceNotFoundException (device disconnected)
// - PluginTimeoutException (host app didn't respond)

async Task<string> GetPluginDataAsync(string pluginId)
{
    if (string.IsNullOrEmpty(pluginId))
        throw new PluginException("Plugin ID is required", "INVALID_ID");

    if (pluginId == "disconnected")
        throw new PluginConnectionException("zoom", "Zoom is not running");

    await Task.Delay(50); // simulate work
    return $"Data for plugin {pluginId}";
}

// Handle different plugin errors differently
string[] testIds = { "", "disconnected", "zoom-plugin" };

foreach (string id in testIds)
{
    try
    {
        string pluginData = await GetPluginDataAsync(id);
        Console.WriteLine($"  Success [{id}]: {pluginData}");
    }
    catch (PluginConnectionException connEx)
    {
        Console.WriteLine($"  Connection error [{id}]: {connEx.HostApp} — {connEx.Message}");
        Console.WriteLine($"    Error code: {connEx.ErrorCode}");
        // Maybe retry, or show "waiting for Zoom to start"
    }
    catch (PluginException plugEx)
    {
        Console.WriteLine($"  Plugin error [{id}]: {plugEx.Message}");
        Console.WriteLine($"    Error code: {plugEx.ErrorCode}");
        // General plugin error — log it
    }
}
Console.WriteLine();


// ============================================
// PART 6: Exceptions in Async Code
// ============================================
Console.WriteLine("=== PART 6: Exceptions in Async Code ===\n");

// This ties directly into yesterday's lesson

// --- 6a: await catches exceptions normally ---
Console.WriteLine("--- 6a: await unwraps exceptions ---");
async Task ThrowInAsyncAsync()
{
    await Task.Delay(50);
    throw new InvalidOperationException("Async boom!");
}

try
{
    await ThrowInAsyncAsync();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  Caught normally: {ex.Message}");
    // await automatically unwraps the exception from the Task
}

// --- 6b: Task.WhenAll — what happens when MULTIPLE tasks fail? ---
Console.WriteLine("\n--- 6b: WhenAll with multiple failures ---");

async Task FailAsync(string name, int delayMs)
{
    await Task.Delay(delayMs);
    throw new Exception($"{name} failed!");
}

try
{
    await Task.WhenAll(
        FailAsync("API-1", 50),
        FailAsync("API-2", 100),
        FailAsync("API-3", 150)
    );
}
catch (Exception ex)
{
    // await only gives you the FIRST exception
    Console.WriteLine($"  Caught (first only): {ex.Message}");
}

// To get ALL exceptions, capture the task
Console.WriteLine("\n  Getting ALL exceptions:");
Task allTasks = Task.WhenAll(
    FailAsync("API-A", 50),
    FailAsync("API-B", 100),
    FailAsync("API-C", 150)
);

try
{
    await allTasks;
}
catch
{
    // allTasks.Exception is an AggregateException containing ALL failures
    if (allTasks.Exception != null)
    {
        foreach (var innerEx in allTasks.Exception.InnerExceptions)
        {
            Console.WriteLine($"    - {innerEx.Message}");
        }
    }
}

// --- 6c: Unobserved exceptions (fire-and-forget danger) ---
Console.WriteLine("\n--- 6c: Fire-and-forget = swallowed exceptions ---");

async Task SilentFailAsync()
{
    await Task.Delay(50);
    throw new Exception("Nobody will ever see this!");
}

// BAD: not awaited — exception is swallowed silently
_ = SilentFailAsync(); // The exception vanishes!
await Task.Delay(100); // Give it time to fail
Console.WriteLine("  SilentFailAsync() threw, but we never saw it (no await = no catch)");
Console.WriteLine("  This is why async void and unawaited tasks are dangerous!\n");


// ============================================
// PART 7: When to Throw vs When to Return
// ============================================
Console.WriteLine("=== PART 7: When to Throw vs Return ===\n");

// THROW for: unexpected errors, broken invariants, can't continue
// RETURN for: expected outcomes, validation results, "not found" scenarios

// Pattern 1: TryParse pattern — return bool, out value
bool TryParseAge(string input, out int age)
{
    age = 0;
    if (!int.TryParse(input, out age)) return false;
    if (age < 0 || age > 150) return false;
    return true;
}

string[] inputs = { "25", "abc", "-5", "200" };
foreach (string input in inputs)
{
    if (TryParseAge(input, out int age))
        Console.WriteLine($"  TryParse \"{input}\": valid, age={age}");
    else
        Console.WriteLine($"  TryParse \"{input}\": invalid");
}

Console.WriteLine();

// Pattern 2: Result pattern (more modern, used in APIs)
async Task<(bool Success, string? Data, string? Error)> FetchUserSafeAsync(int id)
{
    try
    {
        if (id <= 0) return (false, null, "Invalid ID");
        string json = await client.GetStringAsync($"https://jsonplaceholder.typicode.com/users/{id}");
        var doc = JsonDocument.Parse(json);
        string name = doc.RootElement.GetProperty("name").GetString()!;
        return (true, name, null);
    }
    catch (HttpRequestException ex)
    {
        return (false, null, $"HTTP error: {ex.Message}");
    }
}

var (success, data, error) = await FetchUserSafeAsync(1);
Console.WriteLine($"  Result pattern: Success={success}, Data={data}");

var (success2, data2, error2) = await FetchUserSafeAsync(-1);
Console.WriteLine($"  Result pattern: Success={success2}, Error={error2}");

Console.WriteLine(@"
  WHEN TO THROW:
  ┌──────────────────────────────────────────────────────┐
  │ • Null arguments to public methods (ArgumentNull)    │
  │ • Broken preconditions (InvalidOperation)            │
  │ • File not found when it MUST exist                  │
  │ • Database connection failed                         │
  │ • Unrecoverable state                                │
  └──────────────────────────────────────────────────────┘

  WHEN TO RETURN (error code / bool / result tuple):
  ┌──────────────────────────────────────────────────────┐
  │ • User input validation (expected to be wrong)       │
  │ • 'Not found' in a search (normal outcome)           │
  │ • TryParse / TryGet patterns                         │
  │ • High-frequency paths (exceptions are expensive)    │
  └──────────────────────────────────────────────────────┘
");


// ============================================
// PART 8: Finally & IDisposable (using statement)
// ============================================
Console.WriteLine("=== PART 8: Finally & using Statement ===\n");

// finally = guaranteed cleanup
// using = syntactic sugar for try/finally + Dispose()

// Manual cleanup with finally
Console.WriteLine("  --- Manual cleanup with finally ---");
StreamWriter? writer = null;
try
{
    writer = new StreamWriter("test_finally.txt");
    writer.WriteLine("Hello from finally demo");
    Console.WriteLine("  Wrote to file");
}
finally
{
    writer?.Dispose(); // Always close the file, even if exception
    Console.WriteLine("  File handle released in finally");
}

// Better: using statement does the same thing
Console.WriteLine("\n  --- Cleaner: using statement ---");
using (var writer2 = new StreamWriter("test_using.txt"))
{
    writer2.WriteLine("Hello from using demo");
    Console.WriteLine("  Wrote to file");
} // Dispose() called automatically here — even if exception
Console.WriteLine("  File handle released automatically by using");

// Modern C# (8+): using declaration — even cleaner
Console.WriteLine("\n  --- Modern: using declaration ---");
{
    using var writer3 = new StreamWriter("test_modern.txt");
    writer3.WriteLine("Hello from modern using");
    Console.WriteLine("  Wrote to file");
} // Dispose() at end of scope
Console.WriteLine("  File handle released at end of scope");

// Cleanup temp files
File.Delete("test_finally.txt");
File.Delete("test_using.txt");
File.Delete("test_modern.txt");

Console.WriteLine(@"
  RULE: Anything that holds a resource (file, network, database)
        must be in a 'using' block. This includes:
        - StreamReader/StreamWriter
        - HttpClient (usually singleton though)
        - CancellationTokenSource (yesterday!)
        - Database connections
        - File handles
");


// ============================================
// PRACTICE EXERCISES
// ============================================
Console.WriteLine("=== PRACTICE EXERCISES ===\n");

Console.WriteLine("  Exercise 1: Resilient API Client");
Console.WriteLine("    Build FetchWithRetryAsync(url, maxRetries)");
Console.WriteLine("    - Retry on HttpRequestException");
Console.WriteLine("    - Don't retry on ArgumentException (bad URL)");
Console.WriteLine("    - Use exception filters (when) to decide");
Console.WriteLine("    - Exponential backoff: 100ms, 200ms, 400ms\n");

async Task<string> FetchWithRetryAsync(string url, int maxRetries)
{
    int delay = 100;
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await client.GetStringAsync(url);
        }
        catch (HttpRequestException) when (attempt <= maxRetries)
        {
            Console.WriteLine($"  Attempt {attempt} failed, retrying in {delay}ms...");
            await Task.Delay(delay);
            delay *= 2; // Exponential backoff
        }
        // Don't catch ArgumentException — this is a bad URL, no point retrying
        catch (ArgumentException ex)
        {
            Console.WriteLine($"  Invalid URL: {ex.Message}");
            throw; // re-throw to caller
        }
    }
    throw new Exception($"Failed to fetch after {maxRetries} attempts");
}

try
{
    await FetchWithRetryAsync("https://jsonplaceholder.typicode.cousrs/-1", 3);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"  Final failure after retries: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"  Error: {ex.Message}");
}
Console.WriteLine();    

Console.WriteLine("  Exercise 2: Plugin Error Handler");
Console.WriteLine("    Simulate a plugin that:");
Console.WriteLine("    - Connects to a host app (may fail)");
Console.WriteLine("    - Handles device events (may throw)");
Console.WriteLine("    - Must always cleanup on exit (finally/using)");
Console.WriteLine("    - Uses custom exceptions (PluginException hierarchy)\n");

async Task SimulatePluginAsync()
{
    try
    {
        Console.WriteLine("  Connecting to host app...");
        await GetPluginDataAsync("disconnected"); // Simulate connection failure

        Console.WriteLine("  Listening for device events...");
        throw new Exception("Device event handler failed!"); // Simulate event handler error
    }
    catch (PluginConnectionException connEx)
    {
        Console.WriteLine($"  Connection error: {connEx.HostApp} — {connEx.Message}");
    }
    catch (PluginException plugEx)
    {
        Console.WriteLine($"  Plugin error: {plugEx.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  Unexpected error: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("  Cleaning up plugin resources...");
        // Dispose connections, stop listeners, etc.
    }
}

await SimulatePluginAsync();
Console.WriteLine();

Console.WriteLine("  Exercise 3: Input Validator");
Console.WriteLine("    Build ValidateUserInput(name, email, age)");
Console.WriteLine("    - Return a list of validation errors (don't throw)");
Console.WriteLine("    - Throw only for null/unexpected input");
Console.WriteLine("    - Use TryParse for age\n");

async Task<(string? Success, string? data, List<string>? Errors)> ValidateUserInputAsync(string? name, string? email, string? ageInput)
{
    var errors = new List<string>();

    if (name == null) throw new ArgumentNullException(nameof(name));
    if (email == null) throw new ArgumentNullException(nameof(email));
    if (ageInput == null) throw new ArgumentNullException(nameof(ageInput));

    if (string.IsNullOrWhiteSpace(name)) errors.Add("Name is required");
    if (string.IsNullOrWhiteSpace(email)) errors.Add("Email is required");

    if (!TryParseAge(ageInput, out int age))
        errors.Add("Age must be a valid number");
    else if (age < 18) errors.Add("Must be at least 18 years old");

    if (errors.Count > 0)
        return ("failure", null, errors);

    // Simulate async work
    await Task.Delay(50);
    return ("success", $"User {name} validated successfully", errors);
}

// Test 1: Valid input
try
{
    var (res1, data1, errs1) = await ValidateUserInputAsync("Alice", "alice@example.com", "25");
    Console.WriteLine($"  Result: {res1}, Data: {data1}");
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"  Exception: {ex.ParamName} is null");
}

// Test 2: Invalid input (empty strings) — should return errors, not throw
try
{
    var (res2, data2a, errs2) = await ValidateUserInputAsync("", "", "abc");
    Console.WriteLine($"  Result: {res2}");
    if (errs2 != null && errs2.Count > 0)
    {
        foreach (var err in errs2)
            Console.WriteLine($"    - {err}");
    }
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"  Exception: {ex.ParamName} is null");
}

// Test 3: Null input — should THROW (unexpected, not just invalid)
try
{
    var (res3, data3, errs3) = await ValidateUserInputAsync(null, null, null);
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"  Exception thrown (correct!): {ex.ParamName} is null");
}       




// === Classes must be declared after top-level statements ===

// Custom exception hierarchy for plugin development
class PluginException : Exception
{
    public string ErrorCode { get; }

    public PluginException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public PluginException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

class PluginConnectionException : PluginException
{
    public string HostApp { get; }

    public PluginConnectionException(string hostApp, string message)
        : base(message, "CONNECTION_FAILED")
    {
        HostApp = hostApp;
    }
}

class PluginTimeoutException : PluginException
{
    public int TimeoutMs { get; }

    public PluginTimeoutException(int timeoutMs)
        : base($"Plugin timed out after {timeoutMs}ms", "TIMEOUT")
    {
        TimeoutMs = timeoutMs;
    }
}
