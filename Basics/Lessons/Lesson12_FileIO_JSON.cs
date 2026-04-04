// ============================================================
// LESSON 12: File I/O & JSON Serialization
// ============================================================
// Reading/writing files and working with JSON — essential for:
// - Plugin config files
// - Logging
// - API responses
// - Data persistence
// ============================================================
using System.Text.Json;
using System.Text.Json.Serialization;

// ============================================
// PART 1: File Basics — Read & Write Text
// ============================================
Console.WriteLine("=== PART 1: File Basics — Read & Write ===\n");

string filePath = "demo.txt";

// --- Write entire file at once ---
File.WriteAllText(filePath, "Hello from C#!\nThis is line 2.\nLine 3 here.");
Console.WriteLine($"  Wrote to {filePath}");

// --- Read entire file at once ---
string content = File.ReadAllText(filePath);
Console.WriteLine($"  Read entire file:\n{content}\n");

// --- Read as lines (array) ---
string[] lines = File.ReadAllLines(filePath);
Console.WriteLine($"  Line count: {lines.Length}");
for (int i = 0; i < lines.Length; i++)
{
    Console.WriteLine($"    Line {i}: \"{lines[i]}\"");
}
Console.WriteLine();

// --- Append to file (doesn't overwrite) ---
File.AppendAllText(filePath, "\nAppended line 4!");
Console.WriteLine($"  After append:");
Console.WriteLine($"  {File.ReadAllText(filePath)}\n");

// --- Write lines from a collection ---
string[] logEntries = { "2026-04-04 10:00 - App started", "2026-04-04 10:01 - User logged in", "2026-04-04 10:05 - Data loaded" };
File.WriteAllLines("log.txt", logEntries);
Console.WriteLine("  Wrote log.txt with WriteAllLines:");
foreach (var line in File.ReadAllLines("log.txt"))
{
    Console.WriteLine($"    {line}");
}

// Cleanup
File.Delete(filePath);
File.Delete("log.txt");
Console.WriteLine();


// ============================================
// PART 2: File Existence & Path Operations
// ============================================
Console.WriteLine("=== PART 2: File & Directory Operations ===\n");

// --- Check if file exists ---
Console.WriteLine($"  demo.txt exists? {File.Exists("demo.txt")}");       // false (deleted)
Console.WriteLine($"  Program.cs exists? {File.Exists("Program.cs")}");   // true

// --- Path class — platform-safe path operations ---
string fullPath = Path.GetFullPath("Program.cs");
Console.WriteLine($"\n  Full path:   {fullPath}");
Console.WriteLine($"  File name:   {Path.GetFileName(fullPath)}");
Console.WriteLine($"  Extension:   {Path.GetExtension(fullPath)}");
Console.WriteLine($"  Directory:   {Path.GetDirectoryName(fullPath)}");
Console.WriteLine($"  Without ext: {Path.GetFileNameWithoutExtension(fullPath)}");

// --- Combine paths safely (handles slashes for you) ---
string configPath = Path.Combine("config", "plugins", "zoom.json");
Console.WriteLine($"\n  Combined path: {configPath}");
// On Windows: config\plugins\zoom.json
// On Mac/Linux: config/plugins/zoom.json
// Path.Combine handles it — NEVER manually concatenate with "/" or "\"

// --- Directory operations ---
string testDir = "test_folder";
Directory.CreateDirectory(testDir);
Console.WriteLine($"\n  Created directory: {testDir}");
Console.WriteLine($"  Exists? {Directory.Exists(testDir)}");

// Create some files in it
File.WriteAllText(Path.Combine(testDir, "file1.txt"), "hello");
File.WriteAllText(Path.Combine(testDir, "file2.txt"), "world");
File.WriteAllText(Path.Combine(testDir, "data.json"), "{}");

// List files
Console.WriteLine($"  Files in {testDir}:");
foreach (string file in Directory.GetFiles(testDir))
{
    Console.WriteLine($"    {Path.GetFileName(file)}");
}

// Filter by extension
Console.WriteLine($"  Only .txt files:");
foreach (string file in Directory.GetFiles(testDir, "*.txt"))
{
    Console.WriteLine($"    {Path.GetFileName(file)}");
}

// Cleanup
Directory.Delete(testDir, recursive: true);
Console.WriteLine($"  Deleted {testDir} (recursive)\n");


// ============================================
// PART 3: StreamReader & StreamWriter (for large files)
// ============================================
Console.WriteLine("=== PART 3: Streams — For Large Files ===\n");

// File.ReadAllText loads ENTIRE file into memory
// For a 2GB log file → your app uses 2GB RAM → crash
//
// Streams read/write LINE BY LINE — constant memory usage

// --- Write with StreamWriter ---
using (var writer = new StreamWriter("large_demo.txt"))
{
    for (int i = 0; i < 1000; i++)
    {
        writer.WriteLine($"Log entry {i}: Something happened at {DateTime.Now:HH:mm:ss.fff}");
    }
}
Console.WriteLine("  Wrote 1000 lines with StreamWriter");

// --- Read with StreamReader (line by line) ---
int lineCount = 0;
string? firstLine = null;
string? lastLine = null;

using (var reader = new StreamReader("large_demo.txt"))
{
    string? line;
    while ((line = reader.ReadLine()) != null)
    {
        lineCount++;
        firstLine ??= line;   // ??= means "assign only if null"
        lastLine = line;
    }
}
Console.WriteLine($"  Read {lineCount} lines with StreamReader");
Console.WriteLine($"  First: {firstLine}");
Console.WriteLine($"  Last:  {lastLine}");

// Memory comparison:
Console.WriteLine(@"
  File.ReadAllText('big.log')  → loads ALL into memory (bad for large files)
  StreamReader line by line    → only 1 line in memory at a time (safe)

  Rule of thumb:
  ┌──────────────────────────────────────────────┐
  │ Small files (< 1MB):  File.ReadAllText()     │
  │ Large files (> 1MB):  StreamReader            │
  │ Unknown size:         StreamReader (be safe)  │
  └──────────────────────────────────────────────┘
");

File.Delete("large_demo.txt");


// ============================================
// PART 4: JSON Serialization with System.Text.Json
// ============================================
Console.WriteLine("=== PART 4: JSON Serialization ===\n");

// Serialization:   C# object → JSON string
// Deserialization:  JSON string → C# object

// --- Basic serialization ---
var device = new Device
{
    Id = 1,
    Name = "MX Master 3S",
    Type = "Mouse",
    BatteryLevel = 85,
    IsConnected = true
};

string json = JsonSerializer.Serialize(device);
Console.WriteLine($"  Default JSON:\n  {json}\n");

// --- Pretty print (indented) ---
var prettyOptions = new JsonSerializerOptions { WriteIndented = true };
string prettyJson = JsonSerializer.Serialize(device, prettyOptions);
Console.WriteLine($"  Pretty JSON:\n{prettyJson}\n");

// --- Deserialization (JSON → object) ---
string inputJson = """
{
    "Id": 2,
    "Name": "MX Keys S",
    "Type": "Keyboard",
    "BatteryLevel": 92,
    "IsConnected": false
}
""";

Device? parsed = JsonSerializer.Deserialize<Device>(inputJson);
Console.WriteLine($"  Deserialized: {parsed?.Name}, Battery: {parsed?.BatteryLevel}%\n");


// ============================================
// PART 5: JSON Options & Customization
// ============================================
Console.WriteLine("=== PART 5: JSON Options ===\n");

// --- camelCase (standard for APIs) ---
var camelOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

string camelJson = JsonSerializer.Serialize(device, camelOptions);
Console.WriteLine($"  camelCase JSON:\n{camelJson}\n");

// --- Case-insensitive deserialization ---
string weirdJson = """{"id": 3, "NAME": "Craft Keyboard", "type": "Keyboard", "batteryLevel": 50, "isConnected": true}""";

// This would FAIL with default options (case-sensitive)
// Device? fails = JsonSerializer.Deserialize<Device>(weirdJson);  // all nulls/defaults!

var caseInsensitive = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
Device? flexible = JsonSerializer.Deserialize<Device>(weirdJson, caseInsensitive);
Console.WriteLine($"  Case-insensitive parse: {flexible?.Name}, Type: {flexible?.Type}\n");

// --- Ignore null values ---
var deviceWithNulls = new DeviceWithOptionals
{
    Id = 4,
    Name = "StreamDeck",
    FirmwareVersion = null  // don't include this in JSON
};

var ignoreNullOptions = new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = true
};

string noNullsJson = JsonSerializer.Serialize(deviceWithNulls, ignoreNullOptions);
Console.WriteLine($"  Ignore nulls:\n{noNullsJson}\n");


// ============================================
// PART 6: JSON Attributes
// ============================================
Console.WriteLine("=== PART 6: JSON Attributes ===\n");

var plugin = new PluginInfo
{
    PluginId = "zoom-plugin",
    DisplayName = "Zoom Integration",
    Version = "2.1.0",
    InternalSecret = "do-not-serialize",
    MaxRetries = 3
};

string pluginJson = JsonSerializer.Serialize(plugin, prettyOptions);
Console.WriteLine($"  Plugin JSON (with attributes):\n{pluginJson}");
Console.WriteLine("  Notice: InternalSecret is NOT in the JSON ([JsonIgnore])");
Console.WriteLine("  Notice: PluginId became 'plugin_id' ([JsonPropertyName])\n");

// Deserialize with custom names
string remoteJson = """
{
    "plugin_id": "figma-plugin",
    "display_name": "Figma Integration",
    "version": "1.0.0",
    "max_retries": 5
}
""";

PluginInfo? remotePlugin = JsonSerializer.Deserialize<PluginInfo>(remoteJson);
Console.WriteLine($"  Deserialized: {remotePlugin?.DisplayName} v{remotePlugin?.Version}\n");


// ============================================
// PART 7: Working with JSON Files
// ============================================
Console.WriteLine("=== PART 7: JSON Files — Read & Write ===\n");

// --- Save object to JSON file ---
var config = new PluginConfig
{
    PluginId = "zoom-plugin",
    HostApp = "Zoom",
    Settings = new Dictionary<string, string>
    {
        ["mute_key"] = "F6",
        ["video_key"] = "F7",
        ["hand_raise_key"] = "F8"
    },
    EnabledFeatures = new List<string> { "mute_toggle", "video_toggle", "reactions" }
};

string configJson = JsonSerializer.Serialize(config, new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});

string configPath2 = "plugin_config.json";
File.WriteAllText(configPath2, configJson);
Console.WriteLine($"  Saved config to {configPath2}:");
Console.WriteLine($"{File.ReadAllText(configPath2)}\n");

// --- Load object from JSON file ---
string loadedJson = File.ReadAllText(configPath2);
var loadedConfig = JsonSerializer.Deserialize<PluginConfig>(loadedJson, new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
Console.WriteLine($"  Loaded config:");
Console.WriteLine($"    Plugin: {loadedConfig?.PluginId}");
Console.WriteLine($"    Host: {loadedConfig?.HostApp}");
Console.WriteLine($"    Mute key: {loadedConfig?.Settings?["mute_key"]}");
Console.WriteLine($"    Features: {string.Join(", ", loadedConfig?.EnabledFeatures ?? new())}");

File.Delete(configPath2);
Console.WriteLine();


// ============================================
// PART 8: Dynamic JSON (JsonDocument)
// ============================================
Console.WriteLine("=== PART 8: Dynamic JSON (JsonDocument) ===\n");

// Sometimes you don't know the structure beforehand
// Or you only need one field from a huge JSON

string apiResponse = """
{
    "status": "success",
    "data": {
        "user": {
            "id": 42,
            "name": "Santhosh",
            "role": "automation_engineer"
        },
        "devices": [
            {"name": "MX Master 3S", "connected": true},
            {"name": "MX Keys S", "connected": false}
        ]
    },
    "timestamp": "2026-04-04T18:30:00Z"
}
""";

using JsonDocument doc = JsonDocument.Parse(apiResponse);
JsonElement root = doc.RootElement;

// Navigate the JSON tree
string status = root.GetProperty("status").GetString()!;
string userName = root.GetProperty("data").GetProperty("user").GetProperty("name").GetString()!;
int userId = root.GetProperty("data").GetProperty("user").GetProperty("id").GetInt32();

Console.WriteLine($"  Status: {status}");
Console.WriteLine($"  User: {userName} (ID: {userId})");

// Iterate arrays
Console.WriteLine("  Devices:");
JsonElement devices = root.GetProperty("data").GetProperty("devices");
foreach (JsonElement device2 in devices.EnumerateArray())
{
    string name = device2.GetProperty("name").GetString()!;
    bool connected = device2.GetProperty("connected").GetBoolean();
    Console.WriteLine($"    - {name}: {(connected ? "connected" : "disconnected")}");
}

// TryGetProperty — safe access (no exception if missing)
if (root.TryGetProperty("error", out JsonElement errorElement))
{
    Console.WriteLine($"  Error: {errorElement}");
}
else
{
    Console.WriteLine("  No error field (safe check with TryGetProperty)");
}

Console.WriteLine(@"
  When to use what:
  ┌──────────────────────────────────────────────────────────┐
  │ JsonSerializer.Deserialize<T>()                          │
  │   → You know the structure. Map to a class. Most common. │
  ├──────────────────────────────────────────────────────────┤
  │ JsonDocument.Parse()                                     │
  │   → Unknown/dynamic structure. Read specific fields.     │
  │   → API responses where you need 1 field from huge JSON. │
  └──────────────────────────────────────────────────────────┘
");


// ============================================
// PART 9: Async File I/O
// ============================================
Console.WriteLine("=== PART 9: Async File I/O ===\n");

// File operations can be slow (disk, network drives)
// Use async versions to avoid blocking

var deviceList = new List<Device>
{
    new() { Id = 1, Name = "MX Master 3S", Type = "Mouse", BatteryLevel = 85, IsConnected = true },
    new() { Id = 2, Name = "MX Keys S", Type = "Keyboard", BatteryLevel = 92, IsConnected = true },
    new() { Id = 3, Name = "StreamDeck", Type = "Accessory", BatteryLevel = 100, IsConnected = false }
};

// Async write
string devicesPath = "devices.json";
string devicesJson = JsonSerializer.Serialize(deviceList, new JsonSerializerOptions { WriteIndented = true });
await File.WriteAllTextAsync(devicesPath, devicesJson);
Console.WriteLine($"  Async wrote {deviceList.Count} devices to {devicesPath}");

// Async read
string readJson = await File.ReadAllTextAsync(devicesPath);
var loadedDevices = JsonSerializer.Deserialize<List<Device>>(readJson);
Console.WriteLine($"  Async read {loadedDevices?.Count ?? 0} devices:");
foreach (var d in loadedDevices ?? new())
{
    Console.WriteLine($"    {d.Name} ({d.Type}) — Battery: {d.BatteryLevel}%, Connected: {d.IsConnected}");
}

File.Delete(devicesPath);
Console.WriteLine();


// ============================================
// PRACTICE EXERCISES
// ============================================
Console.WriteLine("=== PRACTICE EXERCISES ===\n");

// Exercise 1: Plugin Config Manager
Console.WriteLine("  Exercise 1: Plugin Config Manager\n");

var configManager = new PluginConfigManager("plugin_settings.json");

// Save config
await configManager.SaveAsync(new PluginConfig
{
    PluginId = "figma-plugin",
    HostApp = "Figma",
    Settings = new Dictionary<string, string>
    {
        ["zoom_dial"] = "scroll",
        ["brush_size_dial"] = "resize"
    },
    EnabledFeatures = new List<string> { "zoom", "brush_control", "layer_switch" }
});
Console.WriteLine("  Saved plugin config");

// Load config
var loaded = await configManager.LoadAsync();
Console.WriteLine($"  Loaded: {loaded?.PluginId} for {loaded?.HostApp}");
Console.WriteLine($"  Features: {string.Join(", ", loaded?.EnabledFeatures ?? new())}");

// Update a setting
if (loaded != null)
{
    loaded.Settings["zoom_dial"] = "pan";
    await configManager.SaveAsync(loaded);
    Console.WriteLine("  Updated zoom_dial to 'pan'");

    var reloaded = await configManager.LoadAsync();
    Console.WriteLine($"  Verified: zoom_dial = {reloaded?.Settings["zoom_dial"]}");
}

configManager.Cleanup();
Console.WriteLine();

// Exercise 2: Simple Log Writer
Console.WriteLine("  Exercise 2: Device Event Logger\n");

var logger = new DeviceEventLogger("device_events.log");

await logger.LogEventAsync("MX Master 3S", "connected");
await logger.LogEventAsync("MX Master 3S", "dial_rotate", "45 degrees");
await logger.LogEventAsync("MX Keys S", "key_press", "F6 (mute)");
await logger.LogEventAsync("MX Master 3S", "disconnected");

Console.WriteLine("  Log contents:");
string[] logLines = await File.ReadAllLinesAsync("device_events.log");
foreach (var logLine in logLines)
{
    Console.WriteLine($"    {logLine}");
}

File.Delete("device_events.log");
Console.WriteLine();


Console.WriteLine("=== LESSON 12 COMPLETE ===");
Console.WriteLine("  You now understand:");
Console.WriteLine("  - File.ReadAllText / WriteAllText / AppendAllText");
Console.WriteLine("  - Path.Combine, Directory operations");
Console.WriteLine("  - StreamReader/Writer for large files");
Console.WriteLine("  - JsonSerializer.Serialize / Deserialize");
Console.WriteLine("  - JSON options (camelCase, ignore nulls, attributes)");
Console.WriteLine("  - JsonDocument for dynamic JSON");
Console.WriteLine("  - Async file I/O");
Console.WriteLine("  - Real patterns: config manager, event logger");
Console.WriteLine("\n  PHASE 1A COMPLETE! Next: Phase 1B — ASP.NET Core Web API!");


// ============================================================
// CLASSES
// ============================================================

class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int BatteryLevel { get; set; }
    public bool IsConnected { get; set; }
}

class DeviceWithOptionals
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? FirmwareVersion { get; set; }
}

class PluginInfo
{
    [JsonPropertyName("plugin_id")]
    public string PluginId { get; set; } = "";

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = "";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [JsonIgnore]  // Never include in JSON output
    public string InternalSecret { get; set; } = "";

    [JsonPropertyName("max_retries")]
    public int MaxRetries { get; set; }
}

class PluginConfig
{
    public string PluginId { get; set; } = "";
    public string HostApp { get; set; } = "";
    public Dictionary<string, string> Settings { get; set; } = new();
    public List<string> EnabledFeatures { get; set; } = new();
}

// Exercise 1: Config Manager
class PluginConfigManager
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public PluginConfigManager(string filePath)
    {
        _filePath = filePath;
    }

    public async Task SaveAsync(PluginConfig config)
    {
        string json = JsonSerializer.Serialize(config, _options);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<PluginConfig?> LoadAsync()
    {
        if (!File.Exists(_filePath)) return null;
        string json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<PluginConfig>(json, _options);
    }

    public void Cleanup() => File.Delete(_filePath);
}

// Exercise 2: Event Logger
class DeviceEventLogger
{
    private readonly string _logPath;

    public DeviceEventLogger(string logPath)
    {
        _logPath = logPath;
    }

    public async Task LogEventAsync(string deviceName, string eventType, string? details = null)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string logEntry = details != null
            ? $"[{timestamp}] {deviceName} | {eventType} | {details}"
            : $"[{timestamp}] {deviceName} | {eventType}";

        await File.AppendAllTextAsync(_logPath, logEntry + Environment.NewLine);
    }
}
