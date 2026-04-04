namespace PointersAndWorkaroundsVB;

/// <summary>
/// ValueTuple extension methods for VB.NET - provides tuple deconstruction helpers
/// for VB.NET, which remains short of native tuple deconstruction support.
/// </summary>
public static class ValueTupleExtensions
{
    /// <summary>
    /// Deconstruction helper for 2-element tuples.
    /// </summary>
    /// <typeparam name="T1">Type of the first element.</typeparam>
    /// <typeparam name="T2">Type of the second element.</typeparam>
    /// <param name="tuple">The tuple to deconstruct.</param>
    /// <param name="item1">The first element.</param>
    /// <param name="item2">The second element.</param>
    public static void Deconstruct<T1, T2>(this (T1, T2) tuple, out T1 item1, out T2 item2)
    {
        item1 = tuple.Item1;
        item2 = tuple.Item2;
    }

    /// <summary>
    /// Deconstruction helper for 3-element tuples.
    /// </summary>
    /// <typeparam name="T1">Type of the first element.</typeparam>
    /// <typeparam name="T2">Type of the second element.</typeparam>
    /// <typeparam name="T3">Type of the third element.</typeparam>
    /// <param name="tuple">The tuple to deconstruct.</param>
    /// <param name="item1">The first element.</param>
    /// <param name="item2">The second element.</param>
    /// <param name="item3">The third element.</param>
    public static void Deconstruct<T1, T2, T3>(this (T1, T2, T3) tuple, out T1 item1, out T2 item2, out T3 item3)
    {
        item1 = tuple.Item1;
        item2 = tuple.Item2;
        item3 = tuple.Item3;
    }

    /// <summary>
    /// Deconstruction helper for 4-element tuples.
    /// </summary>
    /// <typeparam name="T1">Type of the first element.</typeparam>
    /// <typeparam name="T2">Type of the second element.</typeparam>
    /// <typeparam name="T3">Type of the third element.</typeparam>
    /// <typeparam name="T4">Type of the fourth element.</typeparam>
    /// <param name="tuple">The tuple to deconstruct.</param>
    /// <param name="item1">The first element.</param>
    /// <param name="item2">The second element.</param>
    /// <param name="item3">The third element.</param>
    /// <param name="item4">The fourth element.</param>
    public static void Deconstruct<T1, T2, T3, T4>(this (T1, T2, T3, T4) tuple, out T1 item1, out T2 item2, out T3 item3, out T4 item4)
    {
        item1 = tuple.Item1;
        item2 = tuple.Item2;
        item3 = tuple.Item3;
        item4 = tuple.Item4;
    }

    /// <summary>
    /// Deconstruction helper for 5-element tuples.
    /// </summary>
    /// <typeparam name="T1">Type of the first element.</typeparam>
    /// <typeparam name="T2">Type of the second element.</typeparam>
    /// <typeparam name="T3">Type of the third element.</typeparam>
    /// <typeparam name="T4">Type of the fourth element.</typeparam>
    /// <typeparam name="T5">Type of the fifth element.</typeparam>
    /// <param name="tuple">The tuple to deconstruct.</param>
    /// <param name="item1">The first element.</param>
    /// <param name="item2">The second element.</param>
    /// <param name="item3">The third element.</param>
    /// <param name="item4">The fourth element.</param>
    /// <param name="item5">The fifth element.</param>
    public static void Deconstruct<T1, T2, T3, T4, T5>(this (T1, T2, T3, T4, T5) tuple,
        out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5)
    {
        item1 = tuple.Item1;
        item2 = tuple.Item2;
        item3 = tuple.Item3;
        item4 = tuple.Item4;
        item5 = tuple.Item5;
    }
}