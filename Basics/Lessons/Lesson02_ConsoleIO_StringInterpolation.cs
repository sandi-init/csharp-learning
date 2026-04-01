// ============================================================
// LESSON 2: Console Input/Output, String Interpolation, Parse()
// ============================================================

// ============================================
// Console Output
// ============================================

Console.WriteLine("Hello, World!");          // prints + new line
Console.Write("No newline here → ");         // prints without new line
Console.WriteLine("continues on same line");

// ============================================
// String Interpolation ($"")
// ============================================

string name = "Santhosh";
int age = 28;

// Old way — string concatenation
Console.WriteLine("Name: " + name + ", Age: " + age);

// Modern way — string interpolation (preferred)
Console.WriteLine($"Name: {name}, Age: {age}");

// Expressions inside interpolation
Console.WriteLine($"Next year you'll be {age + 1}");
Console.WriteLine($"Name uppercase: {name.ToUpper()}");
Console.WriteLine($"Name length: {name.Length}");

// Formatting
double price = 1234.5678;
Console.WriteLine($"Currency: {price:C}");       // $1,234.57
Console.WriteLine($"2 decimals: {price:F2}");    // 1234.57
Console.WriteLine($"With commas: {price:N2}");   // 1,234.57
Console.WriteLine($"Padded: {name,15}");         // right-aligned, 15 chars wide
Console.WriteLine($"Padded: {name,-15}|");       // left-aligned

// ============================================
// Console Input
// ============================================

// Console.ReadLine() returns a string (or null)
// Console.Write("Enter your name: ");
// string? input = Console.ReadLine();
// Console.WriteLine($"Hello, {input}!");

// ============================================
// Parse() — Converting strings to other types
// ============================================

string ageInput = "28";
string priceInput = "19.99";
string boolInput = "true";

// Parse — throws exception if invalid
int parsedAge = int.Parse(ageInput);
double parsedPrice = double.Parse(priceInput);
bool parsedBool = bool.Parse(boolInput);

Console.WriteLine($"Parsed: age={parsedAge}, price={parsedPrice}, bool={parsedBool}");

// TryParse — safe, returns bool instead of throwing
string badInput = "abc";
if (int.TryParse(badInput, out int result))
{
    Console.WriteLine($"Parsed: {result}");
}
else
{
    Console.WriteLine($"'{badInput}' is not a valid number");
}

// TryParse with real input
string userAge = "25";
if (int.TryParse(userAge, out int validAge) && validAge > 0 && validAge < 150)
{
    Console.WriteLine($"Valid age: {validAge}");
}

// ============================================
// String Methods
// ============================================

string text = "  Hello, World!  ";
Console.WriteLine($"Trim: '{text.Trim()}'");
Console.WriteLine($"Upper: '{text.ToUpper()}'");
Console.WriteLine($"Lower: '{text.ToLower()}'");
Console.WriteLine($"Contains: {text.Contains("World")}");
Console.WriteLine($"Replace: '{text.Replace("World", "C#")}'");
Console.WriteLine($"Substring: '{text.Trim().Substring(0, 5)}'");
Console.WriteLine($"Split: {string.Join(", ", "a,b,c".Split(','))}");
Console.WriteLine($"StartsWith: {text.Trim().StartsWith("Hello")}");
Console.WriteLine($"IsNullOrEmpty: {string.IsNullOrEmpty("")}");
Console.WriteLine($"IsNullOrWhiteSpace: {string.IsNullOrWhiteSpace("   ")}");

// ============================================
// Verbatim and Raw Strings
// ============================================

// Verbatim string — no escape sequences needed
string path = @"C:\Users\skumar20\Desktop";
Console.WriteLine($"Path: {path}");

// Raw string literal (C# 11+)
string json = """
    {
        "name": "Santhosh",
        "age": 28
    }
    """;
Console.WriteLine(json);
