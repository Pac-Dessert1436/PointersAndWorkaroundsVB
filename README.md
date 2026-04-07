# PointersAndWorkaroundsVB

*__Not just a pointer library, but a full comprehensive library tailored for VB.NET__*.

> **New in version 1.0.3**: Added a static class named `SizeOf` that provides compile-time constants for common CLI types, allowing for cross-platform type size calculations, memory layout planning, and buffer calculations.

This NuGet package provides VB.NET developers with C#-like features including safe pointer operations, **functional programming utilities**, **tuple deconstruction**, and **workarounds for VB.NET limitations**.

The library aims to bridge the gap between VB.NET and C# features, enabling VB.NET developers to make full use of **modern programming features** while maintaining the safety and simplicity of the VB.NET language.

## Requirements

- **[.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)**, **[.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)** or later
- **VB.NET** projects
- **Unsafe code** permission (automatically enabled)

## Features

### 🔧 Pointer Operations
- **Safe pointer management** for VB.NET developers
- **Enhanced MemoryBlock class** with zero-initialization, span support, and array copying
- **Typed pointers** with bounds checking
- **Array pinning** to prevent GC movement
- **Memory block creation** from existing pointers

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
- **MakeAsyncEnumerable** for converting iterator functions to async enumerables
- **Safe array slicing** with bounds checking
- **Loop signal control** for fine-grained iteration control

### 📦 Tuple Deconstruction
- **Value tuple extensions** for VB.NET
- **Deconstruction helpers** for 2-5 element tuples
- **Type-safe tuple operations**

### 📐 Type Size Constants
- **`SizeOf` static class** provides compile-time constants for common CLI types
- **Cross-platform type size** calculations
- **Memory layout planning** and buffer calculations

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

' Loop control with LoopSignal
Dim numbers() As Integer = {1, 2, 3, 4, 5}
Dim asyncNums = Await Workarounds.MakeAsyncEnumerable(
    Iterator Function()
        For Each n In numbers
            Yield n
        Next
    End Function)

Await asyncNums.ForEachAsync(Function(num)
    If num = 3 Then Return LoopSignal.Continue ' Skip 3
    If num = 5 Then Return LoopSignal.Break     ' Stop at 5
    Console.WriteLine(num)
    Return LoopSignal.Normal
End Function)
```

### Enhanced MemoryBlock Features

```vb
Imports PointersAndWorkaroundsVB

' Create memory block with zero initialization
Using memoryBlock As New MemoryBlock(1024, zeroMemory:=True)
    ' Copy data from managed array
    Dim sourceData() As Integer = {1, 2, 3, 4, 5}
    memoryBlock.CopyFrom(sourceData)
    
    ' Work with spans for safe access
    Dim span = memoryBlock.AsSpan(Of Integer)(5)
    For i As Integer = 0 To span.Length - 1
        span(i) *= 2 ' Modify data through span
    Next i
    
    ' Copy data back to managed array
    Dim resultData(4) As Integer
    memoryBlock.CopyTo(resultData)
End Using

' Create memory block from existing pointer (advanced usage)
Using existingPtr As Pointer(Of Integer) = GetSomePointer()
    Dim referencedBlock = MemoryBlock.FromPointer(existingPtr, 256)
    ' ... then work with the referenced block
End Using
```

### Async Enumerable Support

```vb
Imports PointersAndWorkaroundsVB

' Convert iterator function to async enumerable (for .NET 8.0 & .NET 9.0)
' NOTE: For .NET 10.0, DO NOT USE the `Await` keyword.
Dim asyncNumbers = Await Workarounds.MakeAsyncEnumerable(
    Iterator Function()
        For i = 1 To 10 : Yield i : Next i
    End Function)

' Use with async iteration
Await asyncNumbers.ForEachAsync(Function(num)
    Console.WriteLine($"Processing number: {num}")
    Return LoopSignal.Normal
End Function)
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

### Type Size Constants

```vb
Imports PointersAndWorkaroundsVB

' Calculate buffer sizes for data structures
Dim bufferSize As Integer = SizeOf.Integer + (SizeOf.Double * 10) + SizeOf.Boolean

' Calculate memory layout
Dim structSize As Integer = SizeOf.Integer + SizeOf.Long + SizeOf.Char

' Use in unsafe code scenarios
Using memory As New MemoryBlock(SizeOf.Integer * 100)
    ' Allocate space for 100 integers
End Using

' Binary serialization helpers
Dim floatCount As Integer = totalBytes \ SizeOf.Single
Dim doubleCount As Integer = totalBytes \ SizeOf.Double
```

### Loop Signal Control

```vb
Imports PointersAndWorkaroundsVB

' LoopSignal provides fine-grained control over async iterations:
' - Normal: Continue with normal iteration
' - Continue: Skip to next iteration
' - Break: Exit the loop immediately

Await someAsyncEnumerable.ForEachAsync(Function(item)
    If ShouldSkip(item) Then Return LoopSignal.Continue
    If ShouldStop(item) Then Return LoopSignal.Break
    Process(item)
    Return LoopSignal.Normal
End Function)
```

## API Reference

### LoopSignal Enum
- `LoopSignal.Normal` - Continue with normal iteration behavior
- `LoopSignal.Continue` - Skip to next iteration immediately
- `LoopSignal.Break` - Exit the loop immediately

### SizeOf Class
Static class providing compile-time constants for CLI type sizes (in bytes):

| Constant | Type | Size (bytes) |
|----------|------|--------------|
| `SizeOf.Integer` | Int32 | 4 |
| `SizeOf.UInteger` | UInt32 | 4 |
| `SizeOf.Long` | Int64 | 8 |
| `SizeOf.ULong` | UInt64 | 8 |
| `SizeOf.Short` | Int16 | 2 |
| `SizeOf.UShort` | UInt16 | 2 |
| `SizeOf.Byte` | Byte | 1 |
| `SizeOf.SByte` | SByte | 1 |
| `SizeOf.Single` | Single (float) | 4 |
| `SizeOf.Double` | Double | 8 |
| `SizeOf.Decimal` | Decimal | 16 |
| `SizeOf.Char` | Char | 2 |
| `SizeOf.Boolean` | Boolean | 1 |

### Pointer Class
- `Pointer.Create(array)` - Create a typed pointer from an array
- `Pointer.UBound(ptr)` - Get the upper bound of a pointer
- `Pointer<T>` - Generic pointer class with indexer access

### MemoryBlock Class
- `MemoryBlock(sizeInBytes, zeroMemory)` - Allocate unmanaged memory with optional zero initialization
- `MemoryBlock.Allocate<T>(elementCount)` - Type-safe allocation
- `MemoryBlock.FromPointer<T>(pointer, sizeInBytes)` - Create memory block from existing Pointer(Of T)
- `AsPointer<T>(elementCount)` - Create typed pointer from memory block
- `Fill(byte)` / `Fill<T>(T)` - Fill memory with byte or typed value
- `CopyFrom<T>(array)` - Copy data from managed array to memory block
- `CopyTo<T>(array)` - Copy data from memory block to managed array
- `AsSpan<T>(elementCount)` - Create span for safe memory access
- `AsReadOnlySpan<T>(elementCount)` - Create read-only span for safe memory access

### Workarounds Module
- `Swap(ref T, ref T)` - Swap two values of any type
- `Assign(ref T, T)` - Assign with return value (enables chained assignments)
- `TryMatchType(object, out T)` - Type-safe pattern matching
- `MakeAsyncEnumerable(enumerableFunc)` - Convert iterator function to async enumerable
- `ForEachAsync` - Async iteration with loop control signals
- `Slice(array, start, end)` - Safe array slicing with bounds checking
- `Slice(list, start, end)` - Safe list slicing with bounds checking

### FunctionalExtensions Module
- `PatternMatch` - Pattern matching expressions with predicate-result pairs
- `Compose` - Function composition for chaining transformations
- `Memoize` - Function memoization for caching expensive computations
- `Curry` - Partial function application (currying)
- `Pipe` - Pipeline operator simulation for fluent method chaining

### ValueTupleExtensions Module
- `Deconstruct` - Deconstruction methods for 2-5 element tuples

## Advanced Usage

### Combining Functional Extensions with Workarounds

```vb
Imports PointersAndWorkaroundsVB

' Compose multiple operations with pattern matching
Dim result = 42.5F _
    .Pipe(Function(x) x * 2) _
    .PatternMatch(
        (Function(x) x < 0, "Negative"),
        (Function(x) x >= 0 AndAlso x < 100, "Small"),
        (Function(x) x >= 100, "Large")
    )
```

### Using SizeOf for Binary Protocol Implementation

```vb
Imports PointersAndWorkaroundsVB

' Calculate exact message size for a protocol
Dim messageSize As Integer =
    SizeOf.Integer +          ' Message ID
    SizeOf.Integer +          ' Timestamp
    SizeOf.Integer +          ' Payload length
    payload.Length * SizeOf.Byte  ' Actual payload

' Allocate appropriate buffer
Using buffer As New MemoryBlock(messageSize, zeroMemory:=True)
    ' ... write message fields
End Using
```

### Advanced Async Iteration Patterns

```vb
Imports PointersAndWorkaroundsVB

' Batch processing with early termination
Await largeDataset.ToAsyncEnumerable().ForEachAsync(Function(item)
    If Not IsValid(item) Then Return LoopSignal.Continue
    ProcessBatch(item)
    batchCount += 1
    If batchCount >= maxBatchSize Then Return LoopSignal.Break
    Return LoopSignal.Normal
End Function)
```

### Tuple Deconstruction in Complex Scenarios

```vb
Option Strict On
Option Infer On
Imports PointersAndWorkaroundsVB

' Multiple tuple deconstruction
Dim record = ("Alice", "Johnson", 28, "NYC", 50000D)
Dim firstName, lastName, city As String
Dim age As Integer
Dim salary As Double

record.Deconstruct(firstName, lastName, age, city, salary)

' Useful for swapping tuple values
Dim pair = (100, 200)
Dim a, b As Integer
pair.Deconstruct(a, b)
Workarounds.Swap(a, b) ' Now a=200, b=100
```

## Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

## Support

If you encounter any issues or have questions, please open an issue on the [GitHub repository](https://github.com/Pac-Dessert1436/PointersAndWorkaroundsVB).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.