namespace PointersAndWorkaroundsVB;

/// <summary>
/// Functional programming extensions for VB.NET - provides workarounds for functional
/// programming features not natively supported in VB.NET.
/// </summary>
/// <remarks>
/// This module focuses on functional programming constructs that VB.NET lacks:
/// <para>• Pattern matching expressions</para>
/// <para>• Function composition and method chaining</para>
/// <para>• Memoization for expensive computations</para>
/// <para>• Currying and partial function application</para>
/// <para>• Pipeline operators for fluent APIs</para>
/// </remarks>
public static class FunctionalExtensions
{
    /// <summary>
    /// Provides pattern matching functionality similar to C#'s switch expressions.
    /// VB.NET lacks native pattern matching support.
    /// </summary>
    /// <typeparam name="TInput">The input type.</typeparam>
    /// <typeparam name="TOutput">The output type.</typeparam>
    /// <param name="input">The value to match.</param>
    /// <param name="patterns">Array of pattern functions and their corresponding outputs.</param>
    /// <returns>The matched output value.</returns>
    public static TOutput PatternMatch<TInput, TOutput>(this TInput input, params (Func<TInput, bool> pattern, TOutput result)[] patterns)
    {
        foreach (var (pattern, result) in patterns)
        {
            if (pattern(input)) return result;
        }
        throw new InvalidOperationException("No pattern matched the input value.");
    }

    /// <summary>
    /// Functional composition for method chaining.
    /// VB.NET lacks native composition operators.
    /// </summary>
    /// <typeparam name="T">The input and output type.</typeparam>
    /// <param name="value">The initial value.</param>
    /// <param name="functions">Functions to apply in sequence.</param>
    /// <returns>The transformed value.</returns>
    public static T Compose<T>(this T value, params Func<T, T>[] functions)
    {
        foreach (var func in functions)
        {
            value = func(value);
        }
        return value;
    }

    /// <summary>
    /// Memoization helper for expensive function calls.
    /// VB.NET lacks native memoization support.
    /// </summary>
    /// <typeparam name="TInput">The input type.</typeparam>
    /// <typeparam name="TOutput">The output type.</typeparam>
    /// <param name="func">The function to memoize.</param>
    /// <returns>A memoized version of the function.</returns>
    public static Func<TInput, TOutput> Memoize<TInput, TOutput>(this Func<TInput, TOutput> func)
        where TInput : notnull
    {
        var cache = new Dictionary<TInput, TOutput>();
        return input =>
        {
            if (!cache.TryGetValue(input, out var result))
            {
                result = func(input);
                cache[input] = result;
            }
            return result;
        };
    }

    /// <summary>
    /// Currying helper for partial function application.
    /// VB.NET lacks native currying support.
    /// </summary>
    /// <typeparam name="T1">Type of the first parameter.</typeparam>
    /// <typeparam name="T2">Type of the second parameter.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="func">The function to curry.</param>
    /// <param name="arg1">The first argument.</param>
    /// <returns>A function that takes the second argument.</returns>
    public static Func<T2, TResult> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1)
    {
        return arg2 => func(arg1, arg2);
    }

    /// <summary>
    /// Pipeline operator simulation for method chaining.
    /// VB.NET lacks native pipeline operators.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="value">The value to pipe.</param>
    /// <param name="func">The function to apply.</param>
    /// <returns>The result of applying the function.</returns>
    public static TResult Pipe<T, TResult>(this T value, Func<T, TResult> func) => func(value);
}

/// <summary>
/// Maybe type implementation for safer null handling, equivalent to F#'s Option type.
/// VB.NET lacks native functional programming types like Option/Maybe.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public readonly struct Maybe<T>(T value)
{
    private readonly T _value = value;
    private readonly bool _hasValue = value is not null;

    /// <summary>
    /// Returns true if the Maybe has a value, false otherwise.
    /// </summary>
    /// <returns>True if the Maybe has a value, false otherwise.</returns>
    public bool HasValue => _hasValue;

    /// <summary>
    /// Returns the value if the Maybe has a value, or throws an NullReferenceException otherwise.
    /// </summary>
    public T Value => _hasValue ? _value : throw new NullReferenceException("Maybe has no value");

    /// <summary>
    /// Returns the value if the Maybe has a value, or the default value otherwise.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the Maybe is empty.</param>
    /// <returns>The value if the Maybe has a value, or the default value otherwise.</returns>
    public T GetValueOrDefault(T defaultValue = default!) => _hasValue ? _value : defaultValue;

    /// <summary>
    /// Applies a function to the value if the Maybe has a value, or returns an empty 
    /// Maybe otherwise.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="mapper">The function to apply.</param>
    /// <returns>The result of applying the function.</returns>
    public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return _hasValue ? new Maybe<TResult>(mapper(_value)) : default;
    }

    /// <summary>
    /// Implicitly converts a value to a Maybe.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A Maybe containing the value.</returns>
    public static implicit operator Maybe<T>(T value) => new(value);

    /// <summary>
    /// Creates a Maybe from a nullable value.
    /// </summary>
    /// <param name="value">The nullable value.</param>
    /// <returns>A Maybe containing the value if not null.</returns>
    public static Maybe<T> Some(T value) => new(value);

    /// <summary>
    /// Creates an empty Maybe.
    /// </summary>
    /// <returns>An empty Maybe.</returns>
    public static Maybe<T> None() => default;
}

/// <summary>
/// Either type for representing success or failure, equivalent to F#'s Result type but
/// has the <b>opposite</b> order of the types.
/// </summary>
/// <typeparam name="TLeft">The left (error) type.</typeparam>
/// <typeparam name="TRight">The right (success) type.</typeparam>
public readonly struct Either<TLeft, TRight>
{
    private readonly TLeft _left;
    private readonly TRight _right;
    private readonly bool _isRight;

    /// <summary>
    /// Creates an Either from a left (error) value.
    /// </summary>
    /// <param name="left">The left (error) value.</param>
    public Either(TLeft left)
    {
        _left = left;
        _right = default!;
        _isRight = false;
    }

    /// <summary>
    /// Creates an Either from a right (success) value.
    /// </summary>
    /// <param name="right">The right (success) value.</param>
    public Either(TRight right)
    {
        _left = default!;
        _right = right;
        _isRight = true;
    }

    /// <summary>
    /// Whether the Either is a right (success) value.
    /// </summary>
    public bool IsRight => _isRight;

    /// <summary>
    /// Whether the Either is a left (error) value.
    /// </summary>
    public bool IsLeft => !_isRight;

    /// <summary>
    /// The left (error) value if the Either is a left (error) value.
    /// Throws an InvalidOperationException if the Either is a right (success) value.
    /// </summary>
    public TLeft Left => _isRight ? throw new InvalidOperationException("Either is right") : _left;

    /// <summary>
    /// The right (success) value if the Either is a right (success) value.
    /// Throws an InvalidOperationException if the Either is a left (error) value.
    /// </summary>
    public TRight Right => _isRight ? _right : throw new InvalidOperationException("Either is left");

    /// <summary>
    /// Applies a function to the left (error) value if the Either is a left (error) value,
    /// or applies a function to the right (success) value if the Either is a right (success) 
    /// value. Returns the result of the function application.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="left">The function to apply to the left (error) value.</param>
    /// <param name="right">The function to apply to the right (success) value.</param>
    /// <returns>The result of the function application.</returns>
    public TResult Match<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right) =>
        _isRight ? right(_right) : left(_left);

    /// <summary>
    /// Creates an Either from a left (error) value.
    /// </summary>
    /// <param name="left">The left (error) value.</param>
    public static implicit operator Either<TLeft, TRight>(TLeft left) => new(left);

    /// <summary>
    /// Creates an Either from a right (success) value.
    /// </summary>
    /// <param name="right">The right (success) value.</param>
    public static implicit operator Either<TLeft, TRight>(TRight right) => new(right);
}