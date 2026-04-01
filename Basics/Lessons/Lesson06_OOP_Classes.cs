// ============================================================
// LESSON 6: OOP — Classes, Objects, Methods, Constructors,
//           Properties, Static, Stack vs Heap
// ============================================================

// ============================================
// Basic Class and Object
// ============================================

Console.WriteLine("=== Basic Class ===\n");

var car1 = new Car("Toyota", "Camry", 2024);
var car2 = new Car("Honda", "Civic", 2023);

car1.Start();
car1.Accelerate(60);
car1.DisplayInfo();

Console.WriteLine($"Total cars created: {Car.TotalCars}\n");

// ============================================
// Stack vs Heap
// ============================================

// STACK: value types (int, bool, struct) — fast, auto-cleanup
// HEAP: reference types (class, string, array) — garbage collected

// Value type — each has its own copy
int a = 10;
int b = a;    // COPY of the value
b = 20;
Console.WriteLine($"Value type: a={a}, b={b}");  // a=10, b=20

// Reference type — both point to same object
var carA = new Car("Tesla", "Model 3", 2025);
var carB = carA;  // REFERENCE to the same object
// carB and carA point to the SAME car on the heap

Console.WriteLine();

// ============================================
// Practice: BankAccount
// ============================================

Console.WriteLine("=== BankAccount ===\n");

var account = new BankAccount("Santhosh", 1000);
account.Deposit(500);
account.Withdraw(200);
account.Withdraw(2000); // should fail
account.DisplayBalance();

// ============================================
// Class Definitions
// ============================================

class Car
{
    // --- Fields (private — internal state) ---
    private string _make;
    private string _model;
    private int _speed;

    // --- Properties (public — controlled access) ---
    public int Year { get; }                    // read-only (set in constructor only)
    public string Color { get; set; } = "White"; // auto-property with default

    // --- Static field (shared across ALL instances) ---
    public static int TotalCars { get; private set; } = 0;

    // --- Constructor ---
    public Car(string make, string model, int year)
    {
        _make = make;
        _model = model;
        _speed = 0;
        Year = year;
        TotalCars++; // increment shared counter
    }

    // --- Methods ---
    public void Start()
    {
        Console.WriteLine($"  {_make} {_model} started!");
    }

    public void Accelerate(int amount)
    {
        _speed += amount;
        Console.WriteLine($"  Accelerating to {_speed} km/h");
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"  {Year} {_make} {_model} | Speed: {_speed} km/h | Color: {Color}");
    }

    // --- Static method (called on class, not instance) ---
    public static void ShowTotalCars()
    {
        Console.WriteLine($"  Total cars: {TotalCars}");
    }
}

class BankAccount
{
    // Properties
    public string Owner { get; }
    public decimal Balance { get; private set; } // read from outside, write only internally

    // Constructor
    public BankAccount(string owner, decimal initialBalance)
    {
        Owner = owner;
        Balance = initialBalance;
        Console.WriteLine($"  Account created for {Owner} with balance: {Balance:C}");
    }

    // Methods
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("  Deposit must be positive!");
            return;
        }
        Balance += amount;
        Console.WriteLine($"  Deposited {amount:C}. New balance: {Balance:C}");
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("  Withdrawal must be positive!");
            return;
        }
        if (amount > Balance)
        {
            Console.WriteLine($"  Insufficient funds! Balance: {Balance:C}, Requested: {amount:C}");
            return;
        }
        Balance -= amount;
        Console.WriteLine($"  Withdrew {amount:C}. New balance: {Balance:C}");
    }

    public void DisplayBalance()
    {
        Console.WriteLine($"  {Owner}'s balance: {Balance:C}");
    }
}

// ============================================
// Key Concepts Summary
// ============================================
//
// Fields     → private data (internal state): private int _speed;
// Properties → public access with control: public int Year { get; }
// Constructor → called with "new", initializes the object
// Methods    → behavior / actions the object can do
// Static     → belongs to the CLASS, not to any instance
//               Car.TotalCars (not car1.TotalCars)
//
// Access Modifiers:
//   public    → accessible from anywhere
//   private   → only inside this class (default for fields)
//   protected → this class + children (inheritance)
//   internal  → within the same assembly/project
