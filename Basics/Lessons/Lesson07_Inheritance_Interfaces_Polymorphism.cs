// ============================================================
// LESSON 7: Inheritance, Interfaces, Abstract Classes,
//           Polymorphism, Virtual Dispatch
// ============================================================

// ============================================
// Inheritance — "is a" relationship
// ============================================

Console.WriteLine("=== Inheritance ===\n");

var dog = new Dog("Buddy", 3, "Golden Retriever");
dog.Eat();          // inherited from Animal
dog.MakeSound();    // overridden in Dog
dog.Fetch();        // Dog-specific
Console.WriteLine();

var cat = new Cat("Whiskers", 5);
cat.Eat();
cat.MakeSound();
cat.Purr();
Console.WriteLine();

// ============================================
// Polymorphism — treat different types uniformly
// ============================================

Console.WriteLine("=== Polymorphism ===\n");

// All are Animal references, but each calls its OWN MakeSound()
Animal[] animals = { dog, cat, new Dog("Rex", 2, "Labrador"), new Cat("Luna", 1) };

foreach (Animal animal in animals)
{
    Console.Write($"  {animal.Name}: ");
    animal.MakeSound(); // virtual dispatch — calls the ACTUAL type's method
}
Console.WriteLine();

// ============================================
// Abstract Classes — can't be instantiated
// ============================================

Console.WriteLine("=== Abstract Class (Shape) ===\n");

// Shape shape = new Shape(); // ❌ Cannot create instance of abstract class

Shape circle = new Circle(5);
Shape rectangle = new Rectangle(4, 6);

Console.WriteLine($"  Circle: Area={circle.Area():F2}, Perimeter={circle.Perimeter():F2}");
Console.WriteLine($"  Rectangle: Area={rectangle.Area():F2}, Perimeter={rectangle.Perimeter():F2}");

// Polymorphism with abstract classes
Shape[] shapes = { circle, rectangle, new Circle(3), new Rectangle(10, 2) };
double totalArea = 0;
foreach (Shape shape in shapes)
{
    totalArea += shape.Area();
    shape.Describe(); // calls virtual method
}
Console.WriteLine($"  Total area: {totalArea:F2}\n");

// ============================================
// Interfaces — "can do" contract
// ============================================

Console.WriteLine("=== Interfaces ===\n");

// Interface = a contract that says "I can do X"
// A class can implement MULTIPLE interfaces (unlike single inheritance)

var laptop = new Laptop();
laptop.TurnOn();
laptop.Connect("WiFi-5G");
laptop.Charge();

// Interface as type — program to the interface, not implementation
IPowerable[] devices = { laptop, new SmartWatch() };
foreach (IPowerable device in devices)
{
    device.TurnOn();
}
Console.WriteLine();

// ============================================
// Interface vs Abstract Class
// ============================================
//
// ┌────────────────────┬──────────────────────┐
// │ Abstract Class     │ Interface             │
// ├────────────────────┼──────────────────────┤
// │ "is a" (Dog is     │ "can do" (Laptop can │
// │  an Animal)        │  be powered on)       │
// │                    │                       │
// │ Can have fields,   │ Only method/property  │
// │ constructors,      │ signatures (C# 8+     │
// │ method bodies      │ allows default impl)  │
// │                    │                       │
// │ Single inheritance │ Multiple interfaces   │
// │ (one parent only)  │ (implement many)      │
// │                    │                       │
// │ Use when: shared   │ Use when: unrelated   │
// │ base behavior      │ types share behavior  │
// └────────────────────┴──────────────────────┘

// ============================================
// Virtual Dispatch — How Polymorphism Works
// ============================================
//
// When you call animal.MakeSound():
// 1. Compiler sees the REFERENCE type (Animal)
// 2. Runtime checks the ACTUAL type (Dog or Cat)
// 3. Looks up the method in the ACTUAL type's vtable
// 4. Calls Dog.MakeSound() or Cat.MakeSound()
//
// This is called "virtual dispatch" or "dynamic dispatch"
// It only works with virtual/override methods, not regular methods

// ============================================
// Class Definitions
// ============================================

// --- Inheritance hierarchy ---
class Animal
{
    public string Name { get; }
    public int Age { get; }

    public Animal(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public void Eat()
    {
        Console.WriteLine($"  {Name} is eating");
    }

    // virtual = CAN be overridden by children
    public virtual void MakeSound()
    {
        Console.WriteLine($"  {Name} makes a sound");
    }
}

class Dog : Animal  // Dog inherits from Animal
{
    public string Breed { get; }

    // Call base constructor with : base(...)
    public Dog(string name, int age, string breed) : base(name, age)
    {
        Breed = breed;
    }

    // override = REPLACE the base method
    public override void MakeSound()
    {
        Console.WriteLine($"  {Name} barks! Woof!");
    }

    // Dog-specific method
    public void Fetch()
    {
        Console.WriteLine($"  {Name} ({Breed}) fetches the ball!");
    }
}

class Cat : Animal
{
    public Cat(string name, int age) : base(name, age) { }

    public override void MakeSound()
    {
        Console.WriteLine($"  {Name} meows! Meow!");
    }

    public void Purr()
    {
        Console.WriteLine($"  {Name} purrs...");
    }
}

// --- Abstract class ---
abstract class Shape
{
    // Abstract methods = MUST be implemented by children
    public abstract double Area();
    public abstract double Perimeter();

    // Regular method with implementation — inherited as-is
    public virtual void Describe()
    {
        Console.WriteLine($"  Shape: Area={Area():F2}");
    }
}

class Circle : Shape
{
    public double Radius { get; }

    public Circle(double radius) { Radius = radius; }

    public override double Area() => Math.PI * Radius * Radius;
    public override double Perimeter() => 2 * Math.PI * Radius;
}

class Rectangle : Shape
{
    public double Width { get; }
    public double Height { get; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public override double Area() => Width * Height;
    public override double Perimeter() => 2 * (Width + Height);
}

// --- Interfaces ---
interface IPowerable
{
    void TurnOn();
    void TurnOff();
}

interface IConnectable
{
    void Connect(string network);
}

interface IChargeable
{
    void Charge();
    int BatteryLevel { get; }
}

// A class can implement MULTIPLE interfaces
class Laptop : IPowerable, IConnectable, IChargeable
{
    public int BatteryLevel { get; private set; } = 50;

    public void TurnOn() => Console.WriteLine("  Laptop powered on");
    public void TurnOff() => Console.WriteLine("  Laptop powered off");
    public void Connect(string network) => Console.WriteLine($"  Laptop connected to {network}");
    public void Charge()
    {
        BatteryLevel = 100;
        Console.WriteLine($"  Laptop charged to {BatteryLevel}%");
    }
}

class SmartWatch : IPowerable
{
    public void TurnOn() => Console.WriteLine("  SmartWatch powered on");
    public void TurnOff() => Console.WriteLine("  SmartWatch powered off");
}
