// ============================================================
// LESSON 1: Variables and Data Types
// ============================================================

// ============================================
// Value Types (stored on the stack)
// ============================================

// Integer types
byte smallNumber = 255;           // 0 to 255 (1 byte)
short shortNumber = 32_767;       // -32,768 to 32,767 (2 bytes)
int age = 28;                     // -2.1B to 2.1B (4 bytes) — most common
long bigNumber = 9_000_000_000L;  // very large (8 bytes)

// Floating point types
float price = 19.99f;             // 7 digits precision (4 bytes) — needs 'f' suffix
double pi = 3.14159265358979;     // 15 digits precision (8 bytes) — default for decimals
decimal salary = 75_000.50m;      // 28 digits precision (16 bytes) — needs 'm', best for money

// Other value types
bool isActive = true;             // true or false
char grade = 'A';                 // single character (single quotes)

// ============================================
// Reference Types (stored on the heap)
// ============================================

string name = "Santhosh";         // text (double quotes)
string empty = "";                // empty string
string? nullable = null;          // nullable string

// ============================================
// Type Inference with var
// ============================================

var count = 10;                   // compiler infers int
var message = "Hello";            // compiler infers string
var total = 99.99;                // compiler infers double
// var must be initialized — compiler needs to figure out the type

// ============================================
// Constants
// ============================================

const double PI = 3.14159;        // cannot be changed after declaration
const string APP_NAME = "Options+";

// ============================================
// Default Values
// ============================================

// int    → 0
// double → 0.0
// bool   → false
// string → null
// char   → '\0'

// ============================================
// Naming Conventions
// ============================================

// camelCase  → local variables, parameters: int userId, string firstName
// PascalCase → methods, classes, properties: GetUser(), class UserService
// UPPER_CASE → constants: const int MAX_RETRIES = 3
// _camelCase → private fields: private int _count

Console.WriteLine($"Name: {name}, Age: {age}");
Console.WriteLine($"Price: {price:C}, Salary: {salary:C}");
Console.WriteLine($"Pi: {pi}, IsActive: {isActive}");
Console.WriteLine($"Big number: {bigNumber:N0}");
