// ============================================================
// LESSON 8: Collections, Generics, LINQ
// ============================================================

// ============================================
// List<T> — dynamic array
// ============================================

Console.WriteLine("=== List<T> ===\n");

List<string> fruits = new List<string> { "apple", "banana", "cherry" };

// Add
fruits.Add("date");
fruits.AddRange(new[] { "elderberry", "fig" });
fruits.Insert(0, "avocado"); // insert at index

// Access
Console.WriteLine($"  First: {fruits[0]}");
Console.WriteLine($"  Count: {fruits.Count}");
Console.WriteLine($"  Contains banana: {fruits.Contains("banana")}");
Console.WriteLine($"  Index of cherry: {fruits.IndexOf("cherry")}");

// Remove
fruits.Remove("fig");
fruits.RemoveAt(0); // remove avocado

// Iterate
Console.Write("  All fruits: ");
foreach (string fruit in fruits)
    Console.Write($"{fruit} ");
Console.WriteLine("\n");

// ============================================
// Dictionary<TKey, TValue> — key-value pairs
// ============================================

Console.WriteLine("=== Dictionary<TKey, TValue> ===\n");

Dictionary<string, int> ages = new Dictionary<string, int>
{
    ["Santhosh"] = 28,
    ["Aki"] = 35,
    ["Claude"] = 2
};

// Add / update
ages["Bob"] = 30;            // add new
ages["Santhosh"] = 29;       // update existing

// Access — always check first!
if (ages.TryGetValue("Santhosh", out int age))
    Console.WriteLine($"  Santhosh's age: {age}");

// ages["Unknown"] would throw KeyNotFoundException!
Console.WriteLine($"  Contains 'Aki': {ages.ContainsKey("Aki")}");
Console.WriteLine($"  Count: {ages.Count}");

// Iterate
foreach (var kvp in ages)
    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
Console.WriteLine();

// ============================================
// HashSet<T> — unique values only
// ============================================

Console.WriteLine("=== HashSet<T> ===\n");

HashSet<string> skills = new HashSet<string> { "C#", "Python", "C#" }; // duplicate ignored
skills.Add("TypeScript");
skills.Add("C#"); // ignored — already exists

Console.WriteLine($"  Count: {skills.Count}"); // 3, not 4
Console.Write("  Skills: ");
foreach (string skill in skills)
    Console.Write($"{skill} ");

// Set operations
var teamSkills = new HashSet<string> { "Java", "C#", "Go" };
skills.IntersectWith(teamSkills);  // common skills
Console.Write("\n  Common with team: ");
foreach (string skill in skills)
    Console.Write($"{skill} ");
Console.WriteLine("\n");

// ============================================
// Queue<T> — FIFO (First In, First Out)
// ============================================

Console.WriteLine("=== Queue<T> (FIFO) ===\n");

Queue<string> printQueue = new Queue<string>();
printQueue.Enqueue("Document1.pdf");
printQueue.Enqueue("Photo.jpg");
printQueue.Enqueue("Report.docx");

Console.WriteLine($"  Next in queue: {printQueue.Peek()}"); // look without removing
while (printQueue.Count > 0)
{
    string job = printQueue.Dequeue(); // remove and return
    Console.WriteLine($"  Printing: {job}");
}
Console.WriteLine();

// ============================================
// Stack<T> — LIFO (Last In, First Out)
// ============================================

Console.WriteLine("=== Stack<T> (LIFO) ===\n");

Stack<string> undoHistory = new Stack<string>();
undoHistory.Push("Type 'Hello'");
undoHistory.Push("Bold text");
undoHistory.Push("Change font");

Console.WriteLine($"  Last action: {undoHistory.Peek()}");
while (undoHistory.Count > 0)
{
    string action = undoHistory.Pop();
    Console.WriteLine($"  Undo: {action}");
}
Console.WriteLine();

// ============================================
// Generics — Type-safe reusable code
// ============================================

Console.WriteLine("=== Generics ===\n");

// Without generics — no type safety
// ArrayList list = new ArrayList();
// list.Add(1);
// list.Add("hello"); // mixed types — runtime errors!

// With generics — compiler enforces type
List<int> numbers = new List<int> { 1, 2, 3 };
// numbers.Add("hello"); // ❌ Compile error — type safety!

// Generic method
T GetFirst<T>(List<T> items)
{
    return items[0];
}

Console.WriteLine($"  First number: {GetFirst(numbers)}");
Console.WriteLine($"  First fruit: {GetFirst(fruits)}");
Console.WriteLine();

// ============================================
// LINQ — Language Integrated Query
// ============================================

Console.WriteLine("=== LINQ ===\n");

List<int> nums = new List<int> { 5, 12, 8, 3, 17, 9, 1, 15, 6 };

// Filtering
var evenNumbers = nums.Where(n => n % 2 == 0);
Console.WriteLine($"  Even: {string.Join(", ", evenNumbers)}");

var bigNumbers = nums.Where(n => n > 10);
Console.WriteLine($"  > 10: {string.Join(", ", bigNumbers)}");

// Sorting
var sorted = nums.OrderBy(n => n);
Console.WriteLine($"  Sorted: {string.Join(", ", sorted)}");

var sortedDesc = nums.OrderByDescending(n => n);
Console.WriteLine($"  Desc: {string.Join(", ", sortedDesc)}");

// Transform (Select = map)
var doubled = nums.Select(n => n * 2);
Console.WriteLine($"  Doubled: {string.Join(", ", doubled)}");

var asStrings = nums.Select(n => $"num_{n}");
Console.WriteLine($"  As strings: {string.Join(", ", asStrings)}");

// Aggregation
Console.WriteLine($"  Sum: {nums.Sum()}");
Console.WriteLine($"  Average: {nums.Average():F1}");
Console.WriteLine($"  Min: {nums.Min()}, Max: {nums.Max()}");
Console.WriteLine($"  Count > 5: {nums.Count(n => n > 5)}");

// First / Last / Single
Console.WriteLine($"  First: {nums.First()}");
Console.WriteLine($"  First > 10: {nums.First(n => n > 10)}");
Console.WriteLine($"  FirstOrDefault > 100: {nums.FirstOrDefault(n => n > 100)}"); // 0 if none

// Any / All
Console.WriteLine($"  Any > 15? {nums.Any(n => n > 15)}");
Console.WriteLine($"  All > 0? {nums.All(n => n > 0)}");

// Chaining — the power of LINQ
var result = nums
    .Where(n => n > 3)           // filter: > 3
    .OrderBy(n => n)              // sort ascending
    .Select(n => n * 10)          // transform: multiply by 10
    .Take(3);                     // take first 3
Console.WriteLine($"  Chained: {string.Join(", ", result)}");

Console.WriteLine();

// ============================================
// Practice: Word Frequency Counter
// ============================================

Console.WriteLine("=== Practice: Word Frequency ===\n");

string text = "the cat sat on the mat the cat liked the mat";
string[] words = text.Split(' ');

// Using LINQ GroupBy
var wordCounts = words
    .GroupBy(w => w)
    .OrderByDescending(g => g.Count())
    .Select(g => new { Word = g.Key, Count = g.Count() });

foreach (var wc in wordCounts)
    Console.WriteLine($"  {wc.Word}: {wc.Count}");

string mostCommon = words.GroupBy(w => w).MaxBy(g => g.Count())!.Key;
Console.WriteLine($"\n  Most common: {mostCommon}");

// ============================================
// Practice: Student Grade Tracker
// ============================================

Console.WriteLine("\n=== Practice: Student Grades ===\n");

var students = new List<(string Name, int Grade)>
{
    ("Alice", 92), ("Bob", 78), ("Charlie", 95),
    ("Diana", 88), ("Eve", 65), ("Frank", 73)
};

var topStudents = students.Where(s => s.Grade >= 80).OrderByDescending(s => s.Grade);
Console.WriteLine("  Top students (80+):");
foreach (var s in topStudents)
    Console.WriteLine($"    {s.Name}: {s.Grade}");

Console.WriteLine($"  Class average: {students.Average(s => s.Grade):F1}");
Console.WriteLine($"  Highest: {students.MaxBy(s => s.Grade)?.Name}");
Console.WriteLine($"  Lowest: {students.MinBy(s => s.Grade)?.Name}");
