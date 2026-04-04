// ============================================================
// LESSON 11: Dependency Injection (DI)
// ============================================================
// DI is THE most important pattern in modern C#/.NET.
// ASP.NET Core is BUILT on it. LPS plugins use it.
// Master this — everything else clicks.
// ============================================================
using Microsoft.Extensions.DependencyInjection;

// ============================================
// PART 1: The Problem — Tight Coupling
// ============================================
Console.WriteLine("=== PART 1: The Problem — Tight Coupling ===\n");

// Imagine you're building a plugin that sends notifications.
// The WRONG way — hardcoded dependency:

Console.WriteLine("  --- Tightly coupled (BAD) ---");

var badPlugin = new TightlyCoupledPlugin();
badPlugin.HandleDeviceEvent("Dial rotated");

// What's wrong?
// 1. TightlyCoupledPlugin CREATES its own EmailNotifier inside
// 2. Want to switch to Slack notifications? REWRITE the plugin.
// 3. Want to test without sending real emails? IMPOSSIBLE.
// 4. The plugin KNOWS too much about how notifications work.
Console.WriteLine(@"
  Problem: TightlyCoupledPlugin creates 'new EmailNotifier()' inside itself.
  - Can't swap to SlackNotifier without changing the plugin code
  - Can't test without sending real emails
  - Plugin knows too much — it's married to EmailNotifier
");


// ============================================
// PART 2: The Fix — Dependency Injection
// ============================================
Console.WriteLine("=== PART 2: The Fix — Dependency Injection ===\n");

// Step 1: Define a contract (interface)
// Step 2: Depend on the interface, not the concrete class
// Step 3: INJECT the dependency from outside

Console.WriteLine("  --- Loosely coupled (GOOD) ---");

// We can inject ANY notifier that implements INotifier
INotifier emailNotifier = new EmailNotifier();
var pluginWithEmail = new LooseCoupledPlugin(emailNotifier);
pluginWithEmail.HandleDeviceEvent("Key pressed");

Console.WriteLine();

// Switch to Slack? Just inject a different implementation!
INotifier slackNotifier = new SlackNotifier();
var pluginWithSlack = new LooseCoupledPlugin(slackNotifier);
pluginWithSlack.HandleDeviceEvent("Dial rotated");

Console.WriteLine(@"
  The plugin doesn't know or care HOW notifications are sent.
  It just knows: 'I have something that can Send().'

  This is called 'Inversion of Control' (IoC):
  - Before: Plugin controls which notifier to use (new EmailNotifier())
  - After:  CALLER controls which notifier the plugin gets

  The 3 types of DI:
  ┌─────────────────────────────────────────────────────────────┐
  │ 1. Constructor Injection  ← Most common, preferred         │
  │    class Plugin(INotifier notifier) { }                     │
  │                                                             │
  │ 2. Method Injection       ← For per-call dependencies      │
  │    void Process(INotifier notifier) { }                     │
  │                                                             │
  │ 3. Property Injection     ← Rare, for optional deps        │
  │    public INotifier? Notifier { get; set; }                 │
  └─────────────────────────────────────────────────────────────┘
");


// ============================================
// PART 3: The DI Container (ServiceCollection)
// ============================================
Console.WriteLine("=== PART 3: DI Container (ServiceCollection) ===\n");

// Manually wiring dependencies is fine for 2-3 classes.
// But real apps have DOZENS. That's where the DI Container helps.
//
// .NET has a built-in DI container:
//   ServiceCollection  → register services (the recipe book)
//   ServiceProvider     → resolve services (the chef)

// Step 1: Create the container and register services
var services = new ServiceCollection();

services.AddTransient<INotifier, EmailNotifier>();  // "When someone asks for INotifier, give them EmailNotifier"
services.AddTransient<LooseCoupledPlugin>();         // "You can also create LooseCoupledPlugin"

// Step 2: Build the provider
ServiceProvider provider = services.BuildServiceProvider();

// Step 3: Resolve — the container creates everything automatically!
var plugin = provider.GetRequiredService<LooseCoupledPlugin>();
plugin.HandleDeviceEvent("Gesture detected");

Console.WriteLine(@"
  What just happened:
  1. We asked for LooseCoupledPlugin
  2. Container saw it needs INotifier (constructor parameter)
  3. Container looked up INotifier → found EmailNotifier registered
  4. Container created EmailNotifier, injected it into Plugin
  5. Returned the fully assembled Plugin

  This is 'auto-wiring' — the container builds the dependency tree!
");


// ============================================
// PART 4: Service Lifetimes — Critical!
// ============================================
Console.WriteLine("=== PART 4: Service Lifetimes ===\n");

// Three lifetimes — choosing wrong = bugs or memory leaks

var services2 = new ServiceCollection();

// 1. TRANSIENT — new instance EVERY time
services2.AddTransient<TransientService>();

// 2. SINGLETON — ONE instance for the entire app
services2.AddSingleton<SingletonService>();

// 3. SCOPED — one instance per "scope" (per HTTP request in ASP.NET)
services2.AddScoped<ScopedService>();

var provider2 = services2.BuildServiceProvider();

Console.WriteLine("  --- Transient: new instance every time ---");
var t1 = provider2.GetRequiredService<TransientService>();
var t2 = provider2.GetRequiredService<TransientService>();
Console.WriteLine($"  t1 ID: {t1.Id}");
Console.WriteLine($"  t2 ID: {t2.Id}");
Console.WriteLine($"  Same instance? {ReferenceEquals(t1, t2)}");  // false

Console.WriteLine("\n  --- Singleton: same instance always ---");
var s1 = provider2.GetRequiredService<SingletonService>();
var s2 = provider2.GetRequiredService<SingletonService>();
Console.WriteLine($"  s1 ID: {s1.Id}");
Console.WriteLine($"  s2 ID: {s2.Id}");
Console.WriteLine($"  Same instance? {ReferenceEquals(s1, s2)}");  // true

Console.WriteLine("\n  --- Scoped: same within scope, different across scopes ---");
using (var scope1 = provider2.CreateScope())
{
    var sc1a = scope1.ServiceProvider.GetRequiredService<ScopedService>();
    var sc1b = scope1.ServiceProvider.GetRequiredService<ScopedService>();
    Console.WriteLine($"  Scope 1, request A: {sc1a.Id}");
    Console.WriteLine($"  Scope 1, request B: {sc1b.Id}");
    Console.WriteLine($"  Same in scope? {ReferenceEquals(sc1a, sc1b)}");  // true
}
using (var scope2 = provider2.CreateScope())
{
    var sc2 = scope2.ServiceProvider.GetRequiredService<ScopedService>();
    Console.WriteLine($"  Scope 2: {sc2.Id}");
    Console.WriteLine($"  Different scope = different instance");
}

Console.WriteLine(@"
  LIFETIME CHEAT SHEET:
  ┌────────────┬────────────────────────────────────────────────┐
  │ Transient  │ New instance every time. Lightweight,          │
  │            │ stateless services. (e.g., validators)         │
  ├────────────┼────────────────────────────────────────────────┤
  │ Scoped     │ One per scope/request. Database contexts,      │
  │            │ unit-of-work. (e.g., DbContext in ASP.NET)     │
  ├────────────┼────────────────────────────────────────────────┤
  │ Singleton  │ One for entire app. Expensive to create,       │
  │            │ shared state. (e.g., HttpClient, caches)       │
  └────────────┴────────────────────────────────────────────────┘

  DANGER: Never inject Scoped/Transient into Singleton!
  The scoped service gets 'captured' and lives forever = memory leak.
  .NET will throw an error in Development mode if you try this.
");


// ============================================
// PART 5: Multiple Implementations
// ============================================
Console.WriteLine("=== PART 5: Multiple Implementations ===\n");

// What if you have MULTIPLE implementations of the same interface?

var services3 = new ServiceCollection();
services3.AddTransient<INotifier, EmailNotifier>();
services3.AddTransient<INotifier, SlackNotifier>();
services3.AddTransient<INotifier, ConsoleNotifier>();
services3.AddTransient<NotificationHub>();

var provider3 = services3.BuildServiceProvider();

// GetRequiredService<INotifier>() → returns the LAST registered (ConsoleNotifier)
var singleNotifier = provider3.GetRequiredService<INotifier>();
Console.WriteLine($"  Single resolve: {singleNotifier.GetType().Name} (last registered wins)\n");

// GetServices<INotifier>() → returns ALL of them
var allNotifiers = provider3.GetServices<INotifier>();
Console.WriteLine("  All registered INotifier implementations:");
foreach (var n in allNotifiers)
{
    Console.WriteLine($"    - {n.GetType().Name}");
}

// Real use case: NotificationHub that uses ALL notifiers
Console.WriteLine();
var hub = provider3.GetRequiredService<NotificationHub>();
hub.NotifyAll("Build completed!");

Console.WriteLine();


// ============================================
// PART 6: Real-World Pattern — Options Pattern
// ============================================
Console.WriteLine("=== PART 6: Configuration with DI ===\n");

// In real apps, services need configuration (URLs, timeouts, etc.)
// Don't hardcode — inject configuration too!

var services4 = new ServiceCollection();

// Register configuration as a singleton
services4.AddSingleton(new PluginConfig
{
    HostAppName = "Zoom",
    MaxRetries = 3,
    TimeoutMs = 5000
});

services4.AddTransient<ConfigurablePlugin>();
var provider4 = services4.BuildServiceProvider();

var configPlugin = provider4.GetRequiredService<ConfigurablePlugin>();
configPlugin.Connect();

Console.WriteLine();


// ============================================
// PART 7: DI in Testing — The Killer Feature
// ============================================
Console.WriteLine("=== PART 7: DI Makes Testing Easy ===\n");

// THIS is why DI matters. You can swap real services with fakes.

// Production: real notifier sends actual emails
Console.WriteLine("  --- Production mode ---");
INotifier prodNotifier = new EmailNotifier();
var prodPlugin = new LooseCoupledPlugin(prodNotifier);
prodPlugin.HandleDeviceEvent("Key press");

Console.WriteLine();

// Testing: fake notifier just records what was sent
Console.WriteLine("  --- Test mode (with mock) ---");
var mockNotifier = new MockNotifier();
var testPlugin = new LooseCoupledPlugin(mockNotifier);
testPlugin.HandleDeviceEvent("Dial turn");

// In your test, you can now VERIFY what happened
Console.WriteLine($"  Mock recorded {mockNotifier.SentMessages.Count} message(s):");
foreach (var msg in mockNotifier.SentMessages)
{
    Console.WriteLine($"    - \"{msg}\"");
}

Console.WriteLine(@"
  Without DI:  Plugin creates EmailNotifier → tests send real emails
  With DI:     Plugin receives INotifier → tests inject MockNotifier

  This is how professional C# testing works:
  1. Define interface (INotifier)
  2. Production uses real implementation (EmailNotifier)
  3. Tests inject mock/fake implementation (MockNotifier)
  4. Assert on mock's recorded behavior

  Libraries like Moq and NSubstitute generate mocks automatically.
  You'll use this heavily in Phase 1B with xUnit!
");


// ============================================
// PRACTICE EXERCISES
// ============================================
Console.WriteLine("=== PRACTICE EXERCISES ===\n");

// Exercise 1: Plugin System with DI
Console.WriteLine("  Exercise 1: Plugin System");
Console.WriteLine("  Build a plugin host that manages multiple plugins\n");

var pluginServices = new ServiceCollection();
pluginServices.AddSingleton(new PluginConfig
{
    HostAppName = "Figma",
    MaxRetries = 2,
    TimeoutMs = 3000
});
pluginServices.AddTransient<INotifier, ConsoleNotifier>();
pluginServices.AddTransient<IDeviceHandler, DialHandler>();
pluginServices.AddTransient<IDeviceHandler, KeyHandler>();
pluginServices.AddTransient<PluginHost>();

var pluginProvider = pluginServices.BuildServiceProvider();
var host = pluginProvider.GetRequiredService<PluginHost>();
host.Start();
host.SimulateEvent("dial_rotate", 45);
host.SimulateEvent("key_press", 1);
host.SimulateEvent("unknown", 0);

Console.WriteLine();

// Exercise 2: Swappable Logger
Console.WriteLine("  Exercise 2: Swappable Logger\n");

var logServices = new ServiceCollection();
logServices.AddSingleton<ILogger, FileLogger>();         // swap to ConsoleLogger easily
logServices.AddTransient<OrderProcessor>();

var logProvider = logServices.BuildServiceProvider();
var processor = logProvider.GetRequiredService<OrderProcessor>();
processor.ProcessOrder("ORD-001", 49.99m);
processor.ProcessOrder("ORD-002", -10.00m);

Console.WriteLine();
provider2.Dispose();
provider3.Dispose();
provider4.Dispose();
pluginProvider.Dispose();
logProvider.Dispose();

Console.WriteLine("=== LESSON 11 COMPLETE ===");
Console.WriteLine("  You now understand:");
Console.WriteLine("  - Why tight coupling is bad");
Console.WriteLine("  - Constructor injection");
Console.WriteLine("  - ServiceCollection / ServiceProvider");
Console.WriteLine("  - Transient vs Scoped vs Singleton lifetimes");
Console.WriteLine("  - Multiple implementations");
Console.WriteLine("  - DI for testing (mock injection)");
Console.WriteLine("  - Real-world patterns (config, plugin host)");
Console.WriteLine("\n  Next: File I/O & JSON Serialization!");


// ============================================================
// CLASSES — defined after top-level statements
// ============================================================

// --- Interfaces ---
interface INotifier
{
    void Send(string message);
}

interface IDeviceHandler
{
    string DeviceType { get; }
    void Handle(int value);
}

interface ILogger
{
    void Log(string message);
}

// --- Tightly coupled (BAD) ---
class TightlyCoupledPlugin
{
    private readonly EmailNotifier _notifier = new EmailNotifier(); // hardcoded!

    public void HandleDeviceEvent(string eventName)
    {
        Console.WriteLine($"  [TightPlugin] Event: {eventName}");
        _notifier.Send($"Event occurred: {eventName}");
    }
}

// --- Loosely coupled (GOOD) ---
class LooseCoupledPlugin
{
    private readonly INotifier _notifier; // depends on INTERFACE

    // Constructor injection — dependency comes from outside
    public LooseCoupledPlugin(INotifier notifier)
    {
        _notifier = notifier;
    }

    public void HandleDeviceEvent(string eventName)
    {
        Console.WriteLine($"  [LoosePlugin] Event: {eventName}");
        _notifier.Send($"Event occurred: {eventName}");
    }
}

// --- Notifier implementations ---
class EmailNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"    [Email] {message}");
}

class SlackNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"    [Slack] {message}");
}

class ConsoleNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"    [Console] {message}");
}

class MockNotifier : INotifier
{
    public List<string> SentMessages { get; } = new();

    public void Send(string message)
    {
        SentMessages.Add(message);
        Console.WriteLine($"    [Mock] Recorded: {message}");
    }
}

// --- NotificationHub (uses ALL notifiers) ---
class NotificationHub
{
    private readonly IEnumerable<INotifier> _notifiers;

    public NotificationHub(IEnumerable<INotifier> notifiers)
    {
        _notifiers = notifiers;
    }

    public void NotifyAll(string message)
    {
        Console.WriteLine($"  Hub sending to all channels:");
        foreach (var notifier in _notifiers)
        {
            notifier.Send(message);
        }
    }
}

// --- Lifetime demo services ---
class TransientService
{
    public Guid Id { get; } = Guid.NewGuid();
}

class SingletonService
{
    public Guid Id { get; } = Guid.NewGuid();
}

class ScopedService
{
    public Guid Id { get; } = Guid.NewGuid();
}

// --- Configuration ---
class PluginConfig
{
    public string HostAppName { get; set; } = "";
    public int MaxRetries { get; set; }
    public int TimeoutMs { get; set; }
}

class ConfigurablePlugin
{
    private readonly PluginConfig _config;

    public ConfigurablePlugin(PluginConfig config)
    {
        _config = config;
    }

    public void Connect()
    {
        Console.WriteLine($"  Connecting to {_config.HostAppName}...");
        Console.WriteLine($"  Max retries: {_config.MaxRetries}, Timeout: {_config.TimeoutMs}ms");
        Console.WriteLine($"  Connected!");
    }
}

// --- Exercise 1: Plugin Host ---
class DialHandler : IDeviceHandler
{
    public string DeviceType => "dial_rotate";

    public void Handle(int value)
    {
        Console.WriteLine($"    [Dial] Rotated by {value} degrees");
    }
}

class KeyHandler : IDeviceHandler
{
    public string DeviceType => "key_press";

    public void Handle(int value)
    {
        Console.WriteLine($"    [Key] Key {value} pressed");
    }
}

class PluginHost
{
    private readonly PluginConfig _config;
    private readonly INotifier _notifier;
    private readonly IEnumerable<IDeviceHandler> _handlers;

    public PluginHost(PluginConfig config, INotifier notifier, IEnumerable<IDeviceHandler> handlers)
    {
        _config = config;
        _notifier = notifier;
        _handlers = handlers;
    }

    public void Start()
    {
        Console.WriteLine($"  [Host] Starting plugin for {_config.HostAppName}");
        Console.WriteLine($"  [Host] Registered {_handlers.Count()} device handlers");
        _notifier.Send($"Plugin started for {_config.HostAppName}");
    }

    public void SimulateEvent(string deviceType, int value)
    {
        var handler = _handlers.FirstOrDefault(h => h.DeviceType == deviceType);
        if (handler != null)
        {
            handler.Handle(value);
        }
        else
        {
            Console.WriteLine($"    [Host] No handler for '{deviceType}'");
        }
    }
}

// --- Exercise 2: Swappable Logger ---
class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"    [LOG] {message}");
}

class FileLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"    [FILE-LOG] {message}");
    // In real code, this would write to a file
}

class OrderProcessor
{
    private readonly ILogger _logger;

    public OrderProcessor(ILogger logger)
    {
        _logger = logger;
    }

    public void ProcessOrder(string orderId, decimal amount)
    {
        _logger.Log($"Processing order {orderId}...");

        if (amount <= 0)
        {
            _logger.Log($"REJECTED: {orderId} — invalid amount: {amount:C}");
            return;
        }

        _logger.Log($"SUCCESS: {orderId} — charged {amount:C}");
    }
}
