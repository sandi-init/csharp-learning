// ============================================================
// LESSON 3: Operators and Type Conversions
// ============================================================

// ============================================
// Arithmetic Operators
// ============================================

int a = 10, b = 3;
Console.WriteLine($"a + b = {a + b}");    // 13
Console.WriteLine($"a - b = {a - b}");    // 7
Console.WriteLine($"a * b = {a * b}");    // 30
Console.WriteLine($"a / b = {a / b}");    // 3 (integer division — truncates!)
Console.WriteLine($"a % b = {a % b}");    // 1 (modulus — remainder)

// Integer division gotcha
Console.WriteLine($"10 / 3 = {10 / 3}");           // 3 (not 3.33!)
Console.WriteLine($"10.0 / 3 = {10.0 / 3}");       // 3.333... (double division)
Console.WriteLine($"(double)10 / 3 = {(double)10 / 3}"); // 3.333... (cast to double)

// ============================================
// Assignment Operators
// ============================================

int x = 10;
x += 5;   // x = x + 5 → 15
x -= 3;   // x = x - 3 → 12
x *= 2;   // x = x * 2 → 24
x /= 4;   // x = x / 4 → 6
x %= 4;   // x = x % 4 → 2
Console.WriteLine($"x = {x}");

// Increment / Decrement
int count = 5;
count++;   // count = 6 (post-increment)
++count;   // count = 7 (pre-increment)
count--;   // count = 6
Console.WriteLine($"count = {count}");

// ============================================
// Comparison Operators
// ============================================

int p = 10, q = 20;
Console.WriteLine($"p == q: {p == q}");   // false
Console.WriteLine($"p != q: {p != q}");   // true
Console.WriteLine($"p > q: {p > q}");     // false
Console.WriteLine($"p < q: {p < q}");     // true
Console.WriteLine($"p >= 10: {p >= 10}"); // true
Console.WriteLine($"p <= 10: {p <= 10}"); // true

// ============================================
// Logical Operators
// ============================================

bool isAdult = true;
bool hasLicense = false;

Console.WriteLine($"AND: {isAdult && hasLicense}");   // false (both must be true)
Console.WriteLine($"OR: {isAdult || hasLicense}");    // true (at least one true)
Console.WriteLine($"NOT: {!isAdult}");                // false (flips the value)

// Short-circuit evaluation
// && stops at first false, || stops at first true
// This is safe because if name is null, .Length is never evaluated:
string? name = null;
bool isValid = name != null && name.Length > 0;

// ============================================
// Type Conversions
// ============================================

// --- Implicit conversion (safe, no data loss) ---
int smallNum = 100;
long bigNum = smallNum;       // int → long (safe, no data loss)
float floatNum = smallNum;    // int → float (safe)
double doubleNum = floatNum;  // float → double (safe)
Console.WriteLine($"Implicit: {smallNum} → {bigNum} → {floatNum} → {doubleNum}");

// --- Explicit conversion / Cast (may lose data) ---
double pi = 3.14159;
int truncated = (int)pi;     // 3 — decimal part LOST
Console.WriteLine($"Explicit cast: {pi} → {truncated}");

long huge = 9_000_000_000L;
// int overflow = (int)huge;  // DANGEROUS — overflows silently!

// --- Convert class (safe, throws on failure) ---
string numStr = "42";
int converted = Convert.ToInt32(numStr);
double convertedD = Convert.ToDouble("3.14");
bool convertedB = Convert.ToBoolean("true");
Console.WriteLine($"Convert: {converted}, {convertedD}, {convertedB}");

// --- checked keyword (throws on overflow instead of silently wrapping) ---
try
{
    checked
    {
        int max = int.MaxValue;  // 2,147,483,647
        int overflow = max + 1;  // throws OverflowException!
    }
}
catch (OverflowException ex)
{
    Console.WriteLine($"Overflow caught: {ex.Message}");
}

// ============================================
// Null Operators
// ============================================

// Null-conditional (?.)
string? nullName = null;
int? length = nullName?.Length;   // null, not crash
Console.WriteLine($"Null-conditional: {length}");

// Null-coalescing (??)
string displayName = nullName ?? "Unknown";  // "Unknown" if null
Console.WriteLine($"Null-coalescing: {displayName}");

// Null-coalescing assignment (??=)
string? greeting = null;
greeting ??= "Hello!";   // assigns only if null
Console.WriteLine($"Null-coalescing assign: {greeting}");

// Null-forgiving (!)
// string notNull = nullName!.ToUpper();  // tells compiler "trust me, not null"
// BUT: crashes at runtime if actually null!
