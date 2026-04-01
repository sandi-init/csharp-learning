// ============================================================
// LESSON 4: Control Flow (if/else, ternary, switch, pattern matching)
// ============================================================

// ============================================
// if / else if / else
// ============================================

int score = 85;

if (score >= 90)
{
    Console.WriteLine("Grade: A");
}
else if (score >= 80)
{
    Console.WriteLine("Grade: B");
}
else if (score >= 70)
{
    Console.WriteLine("Grade: C");
}
else
{
    Console.WriteLine("Grade: F");
}

// Single line (no braces) — only for simple statements
if (score > 50) Console.WriteLine("Passed!");

// ============================================
// Ternary Operator (condition ? true : false)
// ============================================

string result = score >= 60 ? "Pass" : "Fail";
Console.WriteLine($"Result: {result}");

// Nested ternary (use sparingly — hard to read)
string grade = score >= 90 ? "A" : score >= 80 ? "B" : score >= 70 ? "C" : "F";
Console.WriteLine($"Grade: {grade}");

// ============================================
// Switch Statement (classic)
// ============================================

string day = "Monday";
switch (day)
{
    case "Monday":
    case "Tuesday":
    case "Wednesday":
    case "Thursday":
    case "Friday":
        Console.WriteLine($"{day} is a weekday");
        break;
    case "Saturday":
    case "Sunday":
        Console.WriteLine($"{day} is a weekend");
        break;
    default:
        Console.WriteLine("Invalid day");
        break;
}

// ============================================
// Switch Expression (modern C# 8+)
// ============================================

// Cleaner, returns a value directly
string dayType = day switch
{
    "Monday" or "Tuesday" or "Wednesday" or "Thursday" or "Friday" => "Weekday",
    "Saturday" or "Sunday" => "Weekend",
    _ => "Invalid"  // _ is the discard/default
};
Console.WriteLine($"{day} is a {dayType}");

// Switch expression with ranges
int age = 28;
string category = age switch
{
    < 0 => "Invalid",
    < 13 => "Child",
    < 18 => "Teenager",
    < 65 => "Adult",
    _ => "Senior"
};
Console.WriteLine($"Age {age}: {category}");

// ============================================
// Pattern Matching
// ============================================

// Type pattern
object value = 42;
if (value is int number)
{
    Console.WriteLine($"It's an int: {number}");
}
else if (value is string text)
{
    Console.WriteLine($"It's a string: {text}");
}

// Property pattern
var person = new { Name = "Santhosh", Age = 28, Role = "Engineer" };

string description = person switch
{
    { Age: < 18 } => "Minor",
    { Role: "Engineer", Age: >= 25 } => "Experienced Engineer",
    { Role: "Engineer" } => "Junior Engineer",
    _ => "Other"
};
Console.WriteLine($"{person.Name}: {description}");

// Tuple pattern
string action = ("admin", "delete") switch
{
    ("admin", _) => "Full access",
    ("user", "read") => "Read only",
    ("user", "write") => "Write allowed",
    _ => "Access denied"
};
Console.WriteLine($"Action: {action}");

// Logical patterns (and, or, not)
int temp = 25;
string weather = temp switch
{
    < 0 => "Freezing",
    >= 0 and < 15 => "Cold",
    >= 15 and < 30 => "Comfortable",
    >= 30 and < 40 => "Hot",
    _ => "Extreme heat"
};
Console.WriteLine($"Temperature {temp}°C: {weather}");

// ============================================
// Practice: Simple calculator logic
// ============================================
char op = '+';
double num1 = 10, num2 = 3;

double calcResult = op switch
{
    '+' => num1 + num2,
    '-' => num1 - num2,
    '*' => num1 * num2,
    '/' when num2 != 0 => num1 / num2,
    '/' => throw new DivideByZeroException("Cannot divide by zero"),
    _ => throw new InvalidOperationException($"Unknown operator: {op}")
};
Console.WriteLine($"{num1} {op} {num2} = {calcResult}");
