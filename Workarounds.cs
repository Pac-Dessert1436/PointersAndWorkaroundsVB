namespace PointersAndWorkaroundsVB;

/// <summary>
/// Represents a signal to control the flow of a loop (especially in <c>ForEachAsync</c> loops).
/// </summary>
public enum LoopSignal : byte
{
    /// <summary>
    /// Normal loop behavior.
    /// </summary>
    Normal = 0,
    /// <summary>
    /// Continue to the next iteration of the loop.
    /// </summary>
    Continue = 1,
    /// <summary>
    /// Break out of the loop.
    /// </summary>
    Break = 2
}

/// <summary>
/// Provides compile-time constant values for the sizes (in bytes) of common CLI types.
/// </summary>
/// <remarks>
/// This class offers a convenient way to access type sizes without using the <c>sizeof</c> operator,
/// which can be useful for cross-platform code, memory calculations, or generating binary data.
/// All values are constants determined at compile time.
/// </remarks>
public static class SizeOf
{
    /// <summary>
    /// The size of a signed 32-bit integer (<c>System.Int32</c>), in bytes.
    /// </summary>
    /// <value>Typically 4 bytes on most CLI platforms.</value>
    public const int Integer = sizeof(int);

    /// <summary>
    /// The size of an unsigned 32-bit integer (<c>System.UInt32</c>), in bytes.
    /// </summary>
    /// <value>Typically 4 bytes on most CLI platforms.</value>
    public const int UInteger = sizeof(uint);

    /// <summary>
    /// The size of a signed 64-bit integer (<c>System.Int64</c>), in bytes.
    /// </summary>
    /// <value>Typically 8 bytes on most CLI platforms.</value>
    public const int Long = sizeof(long);

    /// <summary>
    /// The size of an unsigned 64-bit integer (<c>System.UInt64</c>), in bytes.
    /// </summary>
    /// <value>Typically 8 bytes on most CLI platforms.</value>
    public const int ULong = sizeof(ulong);

    /// <summary>
    /// The size of a signed 16-bit integer (<c>System.Int16</c>), in bytes.
    /// </summary>
    /// <value>Typically 2 bytes on most CLI platforms.</value>
    public const int Short = sizeof(short);

    /// <summary>
    /// The size of an unsigned 16-bit integer (<c>System.UInt16</c>), in bytes.
    /// </summary>
    /// <value>Typically 2 bytes on most CLI platforms.</value>
    public const int UShort = sizeof(ushort);

    /// <summary>
    /// The size of an unsigned 8-bit integer (<c>System.Byte</c>), in bytes.
    /// </summary>
    /// <value>Always 1 byte on all CLI platforms.</value>
    public const int Byte = sizeof(byte);

    /// <summary>
    /// The size of a signed 8-bit integer (<c>System.SByte</c>), in bytes.
    /// </summary>
    /// <value>Always 1 byte on all CLI platforms.</value>
    public const int SByte = sizeof(sbyte);

    /// <summary>
    /// The size of a single-precision floating-point number (<c>System.Single</c> / <c>float</c>), in bytes.
    /// </summary>
    /// <value>Typically 4 bytes on most CLI platforms, conforming to IEEE 754 single-precision standard.</value>
    public const int Single = sizeof(float);

    /// <summary>
    /// The size of a double-precision floating-point number (<c>System.Double</c> / <c>double</c>), in bytes.
    /// </summary>
    /// <value>Typically 8 bytes on most CLI platforms, conforming to IEEE 754 double-precision standard.</value>
    public const int Double = sizeof(double);

    /// <summary>
    /// The size of a decimal type (<c>System.Decimal</c>), in bytes.
    /// </summary>
    /// <value>Typically 16 bytes on most CLI platforms. Note that decimal is a high-precision type often used for financial calculations.</value>
    public const int Decimal = sizeof(decimal);

    /// <summary>
    /// The size of a Unicode character (<c>System.Char</c>), in bytes.
    /// </summary>
    /// <value>Typically 2 bytes on most CLI platforms, representing a UTF-16 code unit.</value>
    public const int Char = sizeof(char);

    /// <summary>
    /// The size of a boolean value (<c>System.Boolean</c>), in bytes.
    /// </summary>
    /// <value>Typically 1 byte on most CLI platforms, though the CLR may optimize storage in arrays.</value>
    public const int Boolean = sizeof(bool);
}

/// <summary>
/// Basic workarounds module for VB.NET limitations - value swapping, assignments with return 
/// values, type matching, async iteration, and safe array/list slicing.
/// </summary>
/// <remarks>
/// This module aims at feature parity with VB.NET, inspired by C# 9.0+ features.
/// </remarks>
[Microsoft.VisualBasic.CompilerServices.StandardModule]
public static class Workarounds
{
    /// <summary>
    /// Swaps the values of two variables of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the variables to swap.</typeparam>
    /// <param name="left">The first variable to swap.</param>
    /// <param name="right">The second variable to swap.</param>
    public static void Swap<T>(ref T left, ref T right) => (right, left) = (left, right);

    /// <summary>
    /// Assigns a value to a variable of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the variable to assign to.</typeparam>
    /// <param name="target">The variable to assign to.</param>
    /// <param name="value">The value to assign.</param>
    /// <returns>The value assigned to the variable.</returns>
    public static T Assign<T>(ref T target, T value) => target = value;

    /// <summary>
    /// Attempts to match the type of a value to a variable of type <typeparamref name="T"/>.
    /// If the value is of the correct type, it assigns the value to the variable and returns 
    /// True; otherwise, it assigns the default value of <typeparamref name="T"/> to the
    /// variable and returns False.
    /// </summary>
    /// <typeparam name="T">The type of the variable to assign to.</typeparam>
    /// <param name="value">The value to match.</param>
    /// <param name="result">The variable to assign to.</param>
    /// <returns>True if the value is of the correct type, False otherwise.</returns>
    public static bool TryMatchType<T>(object value, out T result)
    {
        if (value is T target)
        {
            result = target;
            return true;
        }
        else
        {
            result = default!;
            return false;
        }
    }

    /// <summary>
    /// Converts an iterator function (which returns an enumerable) to an async enumerable.
    /// <c>Await</c> keyword required for .NET 8.0 and .NET 9.0.
    /// </summary>
    /// <remarks>
    /// In .NET SDK 10.0, use the built-in <c>ToAsyncEnumerable</c> extension method for 
    /// variables, fields, properties etc. of type <c>IEnumerable(Of T)</c>.
    /// That extension method is not supported in .NET 8.0 and .NET 9.0.
    /// </remarks>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="enumerableFunc">The function that returns the enumerable.</param>
    /// <returns>The async enumerable of type <typeparamref name="T"/>.</returns>
#if NET10_0
    public static IAsyncEnumerable<T> MakeAsyncEnumerable<T>(Func<IEnumerable<T>> enumerableFunc)
    {
        return enumerableFunc().ToAsyncEnumerable();
    }
#else
    public static async IAsyncEnumerable<T> MakeAsyncEnumerable<T>(Func<IEnumerable<T>> enumerableFunc)
    {
        foreach (T item in enumerableFunc())
        {
            yield return item;
        }
    }
#endif

    /// <summary>
    /// Asynchronously iterates over each element in the enumerable and performs the 
    /// specified loop action on it.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to iterate over.</param>
    /// <param name="loopAction">The loop action to perform on each element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, LoopSignal> loopAction)
    {
        await foreach (T item in enumerable)
        {
            LoopSignal signal = loopAction(item);
            if (signal == LoopSignal.Continue) continue;
            else if (signal == LoopSignal.Break) break;
        }
    }

    /// <summary>
    /// Safe array slicing with bounds checking. VB.NET arrays lack built-in safe slicing.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="array">The source array.</param>
    /// <param name="startIndex">The starting index.</param>
    /// <param name="endIndex">The ending index.</param>
    /// <returns>A new array containing the sliced elements.</returns>
    public static T[] Slice<T>(this T[] array, int startIndex, int endIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (startIndex < 0 || startIndex >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (endIndex < startIndex || endIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(endIndex));

        var result = new T[endIndex - startIndex];
        Array.Copy(array, startIndex, result, 0, endIndex - startIndex);
        return result;
    }

    /// <summary>
    /// Safe list slicing with bounds checking. VB.NET lists lack built-in safe slicing.
    /// </summary>
    /// <typeparam name="T">The list element type.</typeparam>
    /// <param name="list">The source list.</param>
    /// <param name="startIndex">The starting index.</param>
    /// <param name="endIndex">The ending index.</param>
    /// <returns>A new list containing the sliced elements.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static List<T> Slice<T>(this List<T> list, int startIndex, int endIndex)
    {
        ArgumentNullException.ThrowIfNull(list);
        if (startIndex < 0 || startIndex >= list.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (endIndex < startIndex || endIndex > list.Count)
            throw new ArgumentOutOfRangeException(nameof(endIndex));

        return list.GetRange(startIndex, endIndex - startIndex);
    }
}