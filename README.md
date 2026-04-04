# PointersAndWorkaroundsVB

A comprehensive library **tailored for VB.NET**, providing VB.NET developers with C#-like features including safe pointer operations, functional programming utilities, tuple deconstruction, and workarounds for VB.NET limitations.

This library aims to bridge the gap between VB.NET and C# features, enabling VB.NET developers to make full use of **modern programming features** while maintaining the safety and simplicity of the VB.NET language.

## Requirements

- **[.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)** or later
- **VB.NET** projects
- **Unsafe code** permission (automatically enabled)

## Features

### 🔧 Pointer Operations
- **Safe pointer management** for VB.NET developers
- **MemoryBlock class** for unmanaged memory allocation
- **Typed pointers** with bounds checking
- **Array pinning** to prevent GC movement

### 🧮 Functional Programming
- **Pattern matching** expressions
- **Function composition** and method chaining
- **Memoization** for expensive computations
- **Currying** and partial function application
- **Pipeline operators** for fluent APIs

### 🔄 Workarounds
- **Value swapping** with reference parameters
- **Assignments with return values**
- **Type matching** utilities
- **Async iteration** with loop control
- **Safe array slicing** with bounds checking

### 📦 Tuple Deconstruction
- **Value tuple extensions** for VB.NET
- **Deconstruction helpers** for 2-5 element tuples
- **Type-safe tuple operations**

## Installation

```bash
# Package Manager
Install-Package PointersAndWorkaroundsVB

# .NET CLI
dotnet add package PointersAndWorkaroundsVB

# Package Reference
<PackageReference Include="PointersAndWorkaroundsVB" Version="1.0.0" />
```

## Quick Start

### Pointer Operations

```vb
Imports PointersAndWorkaroundsVB

' Create a pointer from an array
Dim numbers() As Integer = {1, 2, 3, 4, 5}
Dim ptr = Pointer.Create(numbers)

' Access elements safely
For i As Integer = 0 To Pointer.UBound(ptr)
    Console.WriteLine(ptr(i))
Next

' Memory block allocation
Using memoryBlock As New MemoryBlock(1024)
    ' Use the memory block for unmanaged operations
    Dim intPtr = memoryBlock.AsPointer(Of Integer)(256)
    ' ... work with the pointer
End Using
```

### Functional Programming

```vb
Imports PointersAndWorkaroundsVB

' Pattern matching
Dim result = (3.5F).PatternMatch(
    (Function(x) x < 0, "Negative"),
    (Function(x) x = 0, "Zero"),
    (Function(x) x > 0, "Positive")
)

' Function composition
Dim transform = (Function(x As Integer) x).Compose(
    Function(n) n * 2,
    Function(n) n + 1,
    Function(n) n.ToString()
)

' Memoization
Dim expensiveFunction = Function(x As Integer)
    ' Expensive computation here
    Return x * x
End Function

Dim memoized = expensiveFunction.Memoize()
```

### Workarounds

```vb
Imports PointersAndWorkaroundsVB

' Value swapping
Dim a = 10, b = 20
Workarounds.Swap(a, b)

' Assignment with return value
Dim value = Workarounds.Assign(a, 42)

' Type matching
If Workarounds.TryMatchType(someObject, value) Then
    ' Use the typed value
End If

' Safe array slicing
Dim original As Integer() = {1, 2, 3, 4, 5}
Dim slice = Workarounds.Slice(original, 1, 3)
```

### Tuple Deconstruction

```vb
Option Strict On
Option Infer On
Imports PointersAndWorkaroundsVB

Dim person = ("John", "Doe", 30)
' Deconstruct the tuple (ideal syntax, currently not supported)
'Dim (firstName, lastName, age) = person

' It is REALISTIC to write:
Dim firstName As String, lastName As String, age As Integer
person.Deconstruct(firstName, lastName, age)
```

## API Reference

### Pointer Class
- `Pointer.Create(array)` - Create a typed pointer from an array
- `Pointer.UBound(ptr)` - Get the upper bound of a pointer
- `Pointer<T>` - Generic pointer class with indexer access

### MemoryBlock Class
- `MemoryBlock(sizeInBytes)` - Allocate unmanaged memory
- `MemoryBlock.Allocate<T>(elementCount)` - Type-safe allocation
- `AsPointer<T>(elementCount)` - Create typed pointer from memory block

### Workarounds Module
- `Swap(ref T, ref T)` - Swap two values
- `Assign(ref T, T)` - Assign with return value
- `TryMatchType(object, out T)` - Type-safe matching
- `ForEachAsync` - Async iteration with loop control
- `Slice(array, start, end)` - Safe array slicing

### FunctionalExtensions Module
- `PatternMatch` - Pattern matching expressions
- `Compose` - Function composition
- `Memoize` - Function memoization
- `Curry` - Partial function application
- `Pipe` - Pipeline operator simulation

### ValueTupleExtensions Module
- `Deconstruct` methods for 2-5 element tuples

## Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

## Support

If you encounter any issues or have questions, please open an issue on the [GitHub repository](https://github.com/Pac-Dessert1436/PointersAndWorkaroundsVB).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.