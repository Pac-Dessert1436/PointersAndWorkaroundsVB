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
    /// </summary>
    /// <remarks>
    /// For variables, fields, properties etc. of type <c>IEnumerable(Of T)</c>,
    /// simply use the built-in <c>ToAsyncEnumerable</c> extension method.
    /// </remarks>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="enumerableFunc">The function that returns the enumerable.</param>
    /// <returns>The async enumerable of type <typeparamref name="T"/>.</returns>
    public static IAsyncEnumerable<T> MakeAsyncEnumerable<T>(Func<IEnumerable<T>> enumerableFunc)
    {
        return enumerableFunc().ToAsyncEnumerable();
    }

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