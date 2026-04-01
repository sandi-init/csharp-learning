// ============================================================
// LESSON 5: Loops (for, while, do-while, break, continue, nested)
// ============================================================

// ============================================
// for loop — when you know the count
// ============================================

Console.WriteLine("=== for loop ===");
for (int i = 1; i <= 5; i++)
{
    Console.WriteLine($"  Count: {i}");
}

// Counting backwards
for (int i = 5; i >= 1; i--)
{
    Console.Write($"{i} ");
}
Console.WriteLine("Go!\n");

// Step by 2
for (int i = 0; i <= 10; i += 2)
{
    Console.Write($"{i} ");
}
Console.WriteLine("\n");

// ============================================
// while loop — when you don't know the count
// ============================================

Console.WriteLine("=== while loop ===");
int number = 1;
while (number <= 5)
{
    Console.WriteLine($"  Number: {number}");
    number++;
}

// Real use case: keep dividing until 1
int value = 128;
Console.Write($"Halving {value}: ");
while (value > 1)
{
    value /= 2;
    Console.Write($"{value} ");
}
Console.WriteLine("\n");

// ============================================
// do-while — always runs at least once
// ============================================

Console.WriteLine("=== do-while loop ===");
int attempt = 1;
do
{
    Console.WriteLine($"  Attempt {attempt}");
    attempt++;
} while (attempt <= 3);

// Useful for menu-driven input:
// do {
//     Console.Write("Enter choice (1-3): ");
//     input = Console.ReadLine();
// } while (input != "1" && input != "2" && input != "3");
Console.WriteLine();

// ============================================
// foreach — iterate over collections
// ============================================

Console.WriteLine("=== foreach loop ===");
string[] fruits = { "apple", "banana", "cherry" };
foreach (string fruit in fruits)
{
    Console.WriteLine($"  Fruit: {fruit}");
}

// Works with any IEnumerable — strings are char collections
foreach (char c in "Hello")
{
    Console.Write($"{c}-");
}
Console.WriteLine("\n");

// ============================================
// break and continue
// ============================================

Console.WriteLine("=== break (exit loop) ===");
for (int i = 1; i <= 10; i++)
{
    if (i == 5) break;  // stop the loop entirely
    Console.Write($"{i} ");
}
Console.WriteLine("(stopped at 5)\n");

Console.WriteLine("=== continue (skip iteration) ===");
for (int i = 1; i <= 10; i++)
{
    if (i % 2 == 0) continue;  // skip even numbers
    Console.Write($"{i} ");
}
Console.WriteLine("(odd numbers only)\n");

// ============================================
// Nested Loops
// ============================================

Console.WriteLine("=== Nested loops ===");

// Multiplication table
Console.WriteLine("3x table:");
for (int i = 1; i <= 5; i++)
{
    Console.WriteLine($"  3 x {i} = {3 * i}");
}

// Star pattern
Console.WriteLine("\nStar pattern:");
for (int row = 1; row <= 5; row++)
{
    for (int col = 1; col <= row; col++)
    {
        Console.Write("* ");
    }
    Console.WriteLine();
}

// ============================================
// Practice: FizzBuzz
// ============================================

Console.WriteLine("\n=== FizzBuzz ===");
for (int i = 1; i <= 20; i++)
{
    if (i % 3 == 0 && i % 5 == 0)
        Console.Write("FizzBuzz ");
    else if (i % 3 == 0)
        Console.Write("Fizz ");
    else if (i % 5 == 0)
        Console.Write("Buzz ");
    else
        Console.Write($"{i} ");
}
Console.WriteLine();

// ============================================
// Practice: Login system with attempts
// ============================================

Console.WriteLine("\n=== Login system (simulated) ===");
string correctPassword = "secret123";
string[] attempts = { "wrong", "nope", "secret123" };
int maxAttempts = 3;

for (int i = 0; i < maxAttempts; i++)
{
    string pwd = attempts[i]; // simulating input
    if (pwd == correctPassword)
    {
        Console.WriteLine($"  Attempt {i + 1}: Login successful!");
        break;
    }
    Console.WriteLine($"  Attempt {i + 1}: Wrong password ({maxAttempts - i - 1} left)");
}
