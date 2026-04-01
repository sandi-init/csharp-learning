// ============================================================
// LESSON 9B: State Machines, Context & Thread Switching
// ============================================================

// ============================================
// PART 1: See the State Machine with Your Own Eyes
// ============================================
Console.WriteLine("=== PART 1: Watching the State Machine ===\n");

// This method has 3 awaits = 4 states (0, 1, 2, 3)
async Task<int> ThreeStepWorkAsync()
{
    Console.WriteLine($"  State 0: Starting          | Thread {Environment.CurrentManagedThreadId}");

    await Task.Delay(100);
    Console.WriteLine($"  State 1: After 1st await   | Thread {Environment.CurrentManagedThreadId}");

    await Task.Delay(100);
    Console.WriteLine($"  State 2: After 2nd await   | Thread {Environment.CurrentManagedThreadId}");

    await Task.Delay(100);
    Console.WriteLine($"  State 3: After 3rd await   | Thread {Environment.CurrentManagedThreadId}");

    return 42;
}

await ThreeStepWorkAsync();
Console.WriteLine();

// What the compiler generated (you can verify with ILDASM):
//
// class ThreeStepWorkAsync_StateMachine : IAsyncStateMachine
// {
//     public int _state;                    // which await are we at?
//     public AsyncTaskMethodBuilder<int> _builder;  // manages the Task<int>
//     private TaskAwaiter _awaiter;          // the thing we're waiting on
//
//     void MoveNext()
//     {
//         switch (_state)
//         {
//             case 0:
//                 Console.WriteLine("State 0: Starting");
//                 _awaiter = Task.Delay(100).GetAwaiter();
//                 if (!_awaiter.IsCompleted)
//                 {
//                     _state = 1;
//                     _builder.AwaitUnsafeOnCompleted(ref _awaiter, ref this);
//                     return;  // ← THREAD IS FREE. Method is "paused" on heap.
//                 }
//                 goto case 1;
//
//             case 1:
//                 _awaiter.GetResult();  // check for exceptions
//                 Console.WriteLine("State 1: After 1st await");
//                 _awaiter = Task.Delay(100).GetAwaiter();
//                 if (!_awaiter.IsCompleted)
//                 {
//                     _state = 2;
//                     _builder.AwaitUnsafeOnCompleted(ref _awaiter, ref this);
//                     return;  // ← THREAD IS FREE again
//                 }
//                 goto case 2;
//
//             case 2:  ... same pattern ...
//             case 3:
//                 _builder.SetResult(42);  // Task<int> completes with value 42
//                 break;
//         }
//     }
// }


// ============================================
// PART 2: Local Variables Become Fields
// ============================================
Console.WriteLine("=== PART 2: How Local Variables Survive Across Awaits ===\n");

async Task LocalVariableSurvivalAsync()
{
    // These local variables are on the STACK... normally.
    // But with async, the compiler LIFTS them into fields on the state machine class.
    int counter = 0;
    string label = "progress";

    Console.WriteLine($"  Before await: counter={counter}, label={label}");
    Console.WriteLine($"  (These are now FIELDS on the heap, not stack variables)\n");

    for (int i = 1; i <= 3; i++)
    {
        await Task.Delay(50);
        counter += 10;
        // After each await, we might be on a different thread
        // But counter and label SURVIVE because they're on the heap
        Console.WriteLine($"  After await #{i}: counter={counter}, label={label} | Thread {Environment.CurrentManagedThreadId}");
    }

    Console.WriteLine($"\n  Final: counter={counter}");
    Console.WriteLine("  The variable survived 3 thread switches because it's a FIELD, not a stack variable.\n");
}

// Compiler transforms the above into roughly:
//
// class LocalVariableSurvival_StateMachine
// {
//     public int counter;     // ← was a local variable, now a field
//     public string label;    // ← was a local variable, now a field
//     public int i;           // ← even the loop variable!
//     public int _state;
//     ...
// }
//
// Stack (dies with thread) → Heap (lives independently of any thread)

await LocalVariableSurvivalAsync();


// ============================================
// PART 3: What DOESN'T Survive — Thread State
// ============================================
Console.WriteLine("=== PART 3: What Gets Lost Across Awaits ===\n");

// Thread-local storage does NOT survive because you might be on a different thread

// ThreadLocal<T> = each thread has its own copy
ThreadLocal<string?> threadLocalValue = new ThreadLocal<string?>();

async Task ThreadStateLossAsync()
{
    threadLocalValue.Value = "I was set on this thread";
    Console.WriteLine($"  Before await: threadLocalValue = \"{threadLocalValue.Value}\"");
    Console.WriteLine($"  Set on Thread {Environment.CurrentManagedThreadId}");

    await Task.Delay(200);

    // After await, we might be on a DIFFERENT thread
    // That thread has its OWN copy of threadLocalValue (which is null!)
    Console.WriteLine($"  After await:  threadLocalValue = \"{threadLocalValue.Value ?? "NULL — LOST!"}\"");
    Console.WriteLine($"  Now on Thread {Environment.CurrentManagedThreadId}");
    Console.WriteLine("  The value was lost because ThreadLocal is per-thread, and we switched threads!\n");
}

await ThreadStateLossAsync();

// What survives vs what's lost:
Console.WriteLine("  ┌─────────────────────┬──────────────────┐");
Console.WriteLine("  │ SURVIVES (heap)      │ LOST (thread)   │");
Console.WriteLine("  ├─────────────────────┼──────────────────┤");
Console.WriteLine("  │ Local variables      │ [ThreadStatic]  │");
Console.WriteLine("  │ Method parameters    │ Thread.Name     │");
Console.WriteLine("  │ Loop counters        │ Thread-local    │");
Console.WriteLine("  │ Captured closures    │   storage       │");
Console.WriteLine("  │ The Task itself      │ Stack frames    │");
Console.WriteLine("  │ Exception info       │ Thread identity │");
Console.WriteLine("  └─────────────────────┴──────────────────┘\n");


// ============================================
// PART 4: SynchronizationContext in Action
// ============================================
Console.WriteLine("=== PART 4: SynchronizationContext Demo ===\n");

// Console apps have NO SynchronizationContext — let's verify
Console.WriteLine($"  Current SynchronizationContext: {SynchronizationContext.Current?.GetType().Name ?? "null"}\n");

// Let's CREATE a custom one to see how it works
// This simulates what WPF/WinForms/Options+ does internally

// In a real UI app (WPF/Options+), the framework provides a SynchronizationContext.
// We'll simulate one using a simple wrapper that logs when Post is called.
var targetThreadId = Environment.CurrentManagedThreadId;
var customContext = new CustomSyncContext(targetThreadId);

// Show behavior WITHOUT SynchronizationContext (default for console)
Console.WriteLine("  --- Without SynchronizationContext ---");
async Task NoContextAsync()
{
    Console.WriteLine($"  Before await: Thread {Environment.CurrentManagedThreadId}");
    await Task.Delay(100);
    Console.WriteLine($"  After await:  Thread {Environment.CurrentManagedThreadId} (any thread pool thread)");
}
await NoContextAsync();

Console.WriteLine();

// Show behavior WITH SynchronizationContext
Console.WriteLine("  --- With SynchronizationContext ---");
var previousContext = SynchronizationContext.Current;
SynchronizationContext.SetSynchronizationContext(customContext);

async Task WithContextAsync()
{
    Console.WriteLine($"  Before await: Thread {Environment.CurrentManagedThreadId}");
    await Task.Delay(100);
    // The SyncContext's Post method gets called to schedule the continuation
    Console.WriteLine($"  After await:  Thread {Environment.CurrentManagedThreadId} (SyncContext tried to marshal back)");
}
await WithContextAsync();
await Task.Delay(200); // Give time for Post to execute

// Restore
SynchronizationContext.SetSynchronizationContext(previousContext);
Console.WriteLine();


// ============================================
// PART 5: ConfigureAwait — Opting Out of Context
// ============================================
Console.WriteLine("=== PART 5: ConfigureAwait(false) — Skip the Context ===\n");

SynchronizationContext.SetSynchronizationContext(customContext);

async Task WithConfigureAwaitFalseAsync()
{
    Console.WriteLine($"  Before: Thread {Environment.CurrentManagedThreadId}");

    // ConfigureAwait(true) — DEFAULT: uses SyncContext to resume
    await Task.Delay(100).ConfigureAwait(true);
    Console.WriteLine($"  After ConfigureAwait(true):  Thread {Environment.CurrentManagedThreadId} (context used)");

    // ConfigureAwait(false) — SKIP: resume on any thread pool thread
    await Task.Delay(100).ConfigureAwait(false);
    Console.WriteLine($"  After ConfigureAwait(false): Thread {Environment.CurrentManagedThreadId} (context skipped)");
}

await WithConfigureAwaitFalseAsync();
await Task.Delay(200);

SynchronizationContext.SetSynchronizationContext(previousContext);

Console.WriteLine(@"
  When to use ConfigureAwait(false):
  ┌──────────────────────────────────────────────────────────────┐
  │ YOUR PLUGIN CODE:                                            │
  │                                                              │
  │ async Task HandleDialTurnAsync()     // UI event handler     │
  │ {                                                            │
  │     // Internal work — doesn't touch UI                      │
  │     var data = await FetchDataAsync()                        │
  │                     .ConfigureAwait(false);  // ← skip UI    │
  │                                                              │
  │     var result = await ProcessAsync(data)                    │
  │                       .ConfigureAwait(false); // ← skip UI   │
  │                                                              │
  │     // Update UI — NEED the context here, so DON'T skip      │
  │     UpdateLabel(result);  // must be on UI thread            │
  │ }                                                            │
  │                                                              │
  │ RULE: Use ConfigureAwait(false) everywhere EXCEPT where      │
  │       you directly touch UI elements.                        │
  └──────────────────────────────────────────────────────────────┘
");


// ============================================
// PART 6: Deadlock Demo — Why .Result is Dangerous
// ============================================
Console.WriteLine("=== PART 6: The Classic Deadlock (Explained) ===\n");

// We can't safely DEMO a deadlock (it would hang forever), so let's trace it:
//
// SCENARIO: UI app (WPF/Options+) with SynchronizationContext
//
//   async Task<string> GetDataAsync()
//   {
//       await Task.Delay(100);  // needs to resume on UI thread (SyncContext)
//       return "data";
//   }
//
//   void ButtonClick()  // runs on UI thread
//   {
//       string result = GetDataAsync().Result;  // ← BLOCKS UI THREAD
//   }
//
// DEADLOCK TIMELINE:
//
//   1. ButtonClick() runs on UI Thread
//   2. Calls GetDataAsync() — starts the async work
//   3. .Result BLOCKS the UI thread — it's now sitting, waiting
//   4. Task.Delay(100) completes
//   5. State machine wants to resume...
//   6. SyncContext says "resume on UI thread"
//   7. But UI thread is BLOCKED by .Result at step 3!
//   8. DEADLOCK — UI thread waits for Task, Task waits for UI thread
//
//   ┌──────────────────────────────────────────┐
//   │  UI Thread                               │
//   │  ┌──────────────────┐                    │
//   │  │ .Result          │ ← waiting for Task │
//   │  │ (BLOCKED)        │                    │
//   │  └────────┬─────────┘                    │
//   │           │ needs UI thread to resume    │
//   │  ┌────────▼─────────┐                    │
//   │  │ Task continuation│ ← waiting for UI   │
//   │  │ (QUEUED)         │   thread            │
//   │  └──────────────────┘                    │
//   │           DEADLOCK!                      │
//   └──────────────────────────────────────────┘
//
//   FIX 1: Use await instead of .Result
//   FIX 2: Use ConfigureAwait(false) in the async method

Console.WriteLine("  Deadlock = UI thread blocked by .Result + SyncContext wants UI thread");
Console.WriteLine("  Fix: ALWAYS use await, NEVER use .Result or .Wait() in UI code");
Console.WriteLine("  This is the #1 async bug in plugin/UI development.\n");

// Safe alternative demo:
async Task<string> SafeGetDataAsync()
{
    await Task.Delay(100);
    return "data fetched safely!";
}

// GOOD — non-blocking
string safeResult = await SafeGetDataAsync();
Console.WriteLine($"  Safe result: {safeResult}\n");


// ============================================
// PART 7: See It in ILDASM (Try This Yourself)
// ============================================
Console.WriteLine("=== PART 7: Verify with ILDASM ===\n");

Console.WriteLine("  To see the ACTUAL state machine the compiler generates:");
Console.WriteLine("  1. Build:    dotnet build");
Console.WriteLine("  2. Find DLL: ls bin/Debug/net10.0/Basics.dll");
Console.WriteLine("  3. Decompile with ILSpy or dotPeek, OR:");
Console.WriteLine("     dotnet tool install --global dotnet-ildasm");
Console.WriteLine("     dotnet ildasm bin/Debug/net10.0/Basics.dll");
Console.WriteLine();
Console.WriteLine("  You'll see classes like:");
Console.WriteLine("    <<Main>$>d__0     ← top-level state machine");
Console.WriteLine("    Fields: int <>1__state, TaskAwaiter <>u__1");
Console.WriteLine("    Method: MoveNext() ← the switch statement\n");

Console.WriteLine("  This proves async/await is pure COMPILER MAGIC — not runtime magic.");
Console.WriteLine("  The CLR sees normal classes and method calls. No special 'async' instruction exists in IL.\n");


// === Class must be declared after top-level statements ===
class CustomSyncContext : SynchronizationContext
{
    private readonly int _targetThreadId;

    public CustomSyncContext(int targetThreadId)
    {
        _targetThreadId = targetThreadId;
    }

    public override void Post(SendOrPostCallback callback, object? state)
    {
        Console.WriteLine($"    [SyncContext] Post called — wants Thread {_targetThreadId}, currently on Thread {Environment.CurrentManagedThreadId}");
        ThreadPool.QueueUserWorkItem(_ => callback(state));
    }
}
