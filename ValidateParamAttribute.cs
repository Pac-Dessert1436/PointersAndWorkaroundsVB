using System.Reflection;
using System.Text.RegularExpressions;

namespace PointersAndWorkaroundsVB;

/// <summary>
/// The validation rule for a parameter, eliminating the need for verbose code.
/// </summary>
[Flags]
public enum ValidationRule
{
    /// <summary>
    /// The parameter must not be null.
    /// </summary>
    NotNull = 1,
    /// <summary>
    /// The parameter must not be equal to zero.
    /// </summary>
    NotEqualToZero = 2,
    /// <summary>
    /// The parameter must be non-negative (greater than or equal to zero).
    /// </summary>
    NonNegative = 4,
    /// <summary>
    /// The parameter must be non-positive (less than or equal to zero).
    /// </summary>
    NonPositive = 8,
    /// <summary>
    /// The parameter must be always positive.
    /// </summary>
    AlwaysPositive = NotEqualToZero | NonNegative,
    /// <summary>
    /// The parameter must be always negative.
    /// </summary>
    AlwaysNegative = NotEqualToZero | NonPositive,
    /// <summary>
    /// The parameter must not be empty (for strings and collections).
    /// </summary>
    NotEmpty = 16,
    /// <summary>
    /// The parameter must not be null or empty.
    /// </summary>
    NotNullOrEmpty = NotNull | NotEmpty,
    /// <summary>
    /// The parameter must not be whitespace (for strings).
    /// </summary>
    NotWhitespace = 32,
    /// <summary>
    /// The parameter must not be null, empty, or whitespace.
    /// </summary>
    NotNullOrWhitespace = NotNull | NotEmpty | NotWhitespace,
    /// <summary>
    /// The parameter must not be the default value for its type.
    /// </summary>
    NotDefault = 64,
    /// <summary>
    /// The parameter must not be NaN (for floating-point types).
    /// </summary>
    NotNaN = 128,
    /// <summary>
    /// The parameter must be finite (not NaN or infinity).
    /// </summary>
    Finite = NotNaN | 256,
    /// <summary>
    /// The parameter must be a valid email address.
    /// </summary>
    Email = 512,
    /// <summary>
    /// The parameter must be a valid URL.
    /// </summary>
    Url = 1024
}

/// <summary>
/// Specifies the validation rules for a parameter. Traditional VB.NET needs verbose code to validate
/// parameters. This attribute simplifies the process by allowing you to specify the validation
/// rules in a single line of code.
/// </summary>
/// <param name="validation">The validation rules to apply (default is NotNull).</param>
/// <param name="minLength">Minimum length for strings and collections (optional).</param>
/// <param name="maxLength">Maximum length for strings and collections (optional).</param>
/// <param name="minValue">Minimum value for numeric types (optional).</param>
/// <param name="maxValue">Maximum value for numeric types (optional).</param>
/// <param name="regexPattern">Regular expression pattern for string validation (optional).</param>
[AttributeUsage(AttributeTargets.Parameter)]
public partial class ValidateParamAttribute(
    ValidationRule validation = ValidationRule.NotNull,
    int minLength = -1,
    int maxLength = -1,
    double minValue = double.MinValue,
    double maxValue = double.MaxValue,
    string? regexPattern = null) : Attribute
{
    internal ValidationRule Validation { get; } = validation;
    internal int MinLength { get; } = minLength;
    internal int MaxLength { get; } = maxLength;
    internal double MinValue { get; } = minValue;
    internal double MaxValue { get; } = maxValue;
    internal string? RegexPattern { get; } = regexPattern;

    /// <summary>
    /// Validates a parameter value against the specified validation rules.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The name of the parameter (for error messages).</param>
    /// <exception cref="ArgumentNullException">Thrown when null validation fails.</exception>
    /// <exception cref="ArgumentException">Thrown when other validation rules fail.</exception>
    public void Validate(object? value, string parameterName)
    {
        if ((Validation & ValidationRule.NotNull) != 0 && value is null)
        {
            throw new ArgumentNullException(parameterName, $"Parameter '{parameterName}' cannot be null.");
        }

        if (value is null)
        {
            return;
        }

        if ((Validation & ValidationRule.NotDefault) != 0)
        {
            if (Equals(value, GetDefaultValue(value.GetType())))
            {
                throw new ArgumentException($"Parameter '{parameterName}' cannot be the default value for its type.", parameterName);
            }
        }

        if ((Validation & ValidationRule.NotEqualToZero) != 0)
        {
            if (IsZero(value))
            {
                throw new ArgumentException($"Parameter '{parameterName}' cannot be zero.", parameterName);
            }
        }

        if ((Validation & ValidationRule.NonNegative) != 0)
        {
            if (IsNegative(value))
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be non-negative.", parameterName);
            }
        }

        if ((Validation & ValidationRule.NonPositive) != 0)
        {
            if (IsPositive(value))
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be non-positive.", parameterName);
            }
        }

        if ((Validation & ValidationRule.AlwaysPositive) != 0)
        {
            if (!IsPositive(value))
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be positive.", parameterName);
            }
        }

        if ((Validation & ValidationRule.AlwaysNegative) != 0)
        {
            if (!IsNegative(value))
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be negative.", parameterName);
            }
        }

        if ((Validation & ValidationRule.NotEmpty) != 0)
        {
            if (IsEmpty(value))
            {
                throw new ArgumentException($"Parameter '{parameterName}' cannot be empty.", parameterName);
            }
        }

        if ((Validation & ValidationRule.NotWhitespace) != 0 && value is string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException($"Parameter '{parameterName}' cannot be whitespace.", parameterName);
            }
        }

        if ((Validation & ValidationRule.NotNaN) != 0)
        {
            if (IsNaN(value))
            {
                throw new ArgumentException($"Parameter '{parameterName}' cannot be NaN.", parameterName);
            }
        }

        if ((Validation & ValidationRule.Finite) != 0)
        {
            if (!IsFinite(value))
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be finite.", parameterName);
            }
        }

        if ((Validation & ValidationRule.Email) != 0 && value is string email)
        {
            if (!IsValidEmail(email))
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be a valid email address.", parameterName);
            }
        }

        if ((Validation & ValidationRule.Url) != 0 && value is string url)
        {
            if (!IsValidUrl(url))
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be a valid URL.", parameterName);
            }
        }

        if (MinLength >= 0)
        {
            int length = GetLength(value);
            if (length < MinLength)
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be at least {MinLength} characters/elements long.", parameterName);
            }
        }

        if (MaxLength >= 0)
        {
            int length = GetLength(value);
            if (length > MaxLength)
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be at most {MaxLength} characters/elements long.", parameterName);
            }
        }

        if (MinValue != double.MinValue || MaxValue != double.MaxValue)
        {
            if (!TryGetNumericValue(value, out double numericValue))
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be a numeric type for range validation.", parameterName);
            }

            if (MinValue != double.MinValue && numericValue < MinValue)
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be at least {MinValue}.", parameterName);
            }

            if (MaxValue != double.MaxValue && numericValue > MaxValue)
            {
                throw new ArgumentException($"Parameter '{parameterName}' must be at most {MaxValue}.", parameterName);
            }
        }

        if (!string.IsNullOrEmpty(RegexPattern) && value is string strValue)
        {
            if (!Regex.IsMatch(strValue, RegexPattern))
            {
                throw new ArgumentException($"Parameter '{parameterName}' does not match the required pattern.", parameterName);
            }
        }
    }

    private static object GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type)! : null!;
    }

    private static bool IsZero(object value)
    {
        return value switch
        {
            int i => i == 0,
            long l => l == 0,
            short s => s == 0,
            byte b => b == 0,
            float f => f == 0,
            double d => d == 0,
            decimal m => m == 0,
            _ => false
        };
    }

    private static bool IsNegative(object value)
    {
        return value switch
        {
            int i => i < 0,
            long l => l < 0,
            short s => s < 0,
            float f => f < 0,
            double d => d < 0,
            decimal m => m < 0,
            _ => false
        };
    }

    private static bool IsPositive(object value)
    {
        return value switch
        {
            int i => i > 0,
            long l => l > 0,
            short s => s > 0,
            byte b => b > 0,
            float f => f > 0,
            double d => d > 0,
            decimal m => m > 0,
            _ => false
        };
    }

    private static bool IsEmpty(object value)
    {
        return value switch
        {
            string s => string.IsNullOrEmpty(s),
            System.Collections.ICollection c => c.Count == 0,
            System.Collections.IEnumerable e when value is not string => !e.GetEnumerator().MoveNext(),
            _ => false
        };
    }

    private static bool IsNaN(object value)
    {
        return value switch
        {
            float f => float.IsNaN(f),
            double d => double.IsNaN(d),
            _ => false
        };
    }

    private static bool IsFinite(object value)
    {
        return value switch
        {
            float f => float.IsFinite(f),
            double d => double.IsFinite(d),
            _ => true
        };
    }

    private static int GetLength(object value)
    {
        return value switch
        {
            string s => s.Length,
            System.Collections.ICollection c => c.Count,
            System.Collections.IEnumerable e when value is not string => e.Cast<object>().Count(),
            _ => 0
        };
    }

    private static bool TryGetNumericValue(object value, out double numericValue)
    {
        numericValue = 0;
        return value switch
        {
            int i => (numericValue = i) >= 0 || true,
            long l => (numericValue = l) >= long.MinValue || true,
            short s => (numericValue = s) >= short.MinValue || true,
            byte b => (numericValue = b) >= 0 || true,
            float f => (numericValue = f) >= float.MinValue || true,
            double d => (numericValue = d) >= double.MinValue || true,
            decimal m => (numericValue = (double)m) >= (double)decimal.MinValue || true,
            _ => false
        };
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return ValidEmailRegex().IsMatch(email);
    }

    private static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex ValidEmailRegex();
}

/// <summary>
/// Provides subroutines for validating parameters using reflection.
/// </summary>
[Microsoft.VisualBasic.CompilerServices.StandardModule]
public static class ParameterValidation
{
    /// <summary>
    /// Validates all parameters of a method that have ValidateParamAttribute.
    /// </summary>
    /// <param name="method">The method to validate parameters for.</param>
    /// <param name="parameterValues">The parameter values to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when null validation fails.</exception>
    /// <exception cref="ArgumentException">Thrown when other validation rules fail.</exception>
    public static void ValidateParameters(MethodBase method, params object?[] parameterValues)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(parameterValues);

        ParameterInfo[] parameters = method.GetParameters();

        for (int i = 0; i < parameters.Length; i++)
        {
            ParameterInfo parameter = parameters[i];
            object? value = i < parameterValues.Length ? parameterValues[i] : null;

            ValidateParamAttribute? attribute = parameter.GetCustomAttribute<ValidateParamAttribute>();
            attribute?.Validate(value, parameter.Name!);
        }
    }

    /// <summary>
    /// Validates all parameters of a method that have ValidateParamAttribute.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type.</typeparam>
    /// <param name="method">The delegate method to validate parameters for.</param>
    /// <param name="parameterValues">The parameter values to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when null validation fails.</exception>
    /// <exception cref="ArgumentException">Thrown when other validation rules fail.</exception>
    public static void ValidateParameters<TDelegate>(TDelegate method, params object?[] parameterValues)
        where TDelegate : Delegate
    {
        ArgumentNullException.ThrowIfNull(method);
        ValidateParameters(method.Method, parameterValues);
    }

    /// <summary>
    /// Validates a single parameter value against a ValidateParamAttribute.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validation">The validation rules to apply.</param>
    /// <param name="parameterName">The name of the parameter (for error messages).</param>
    /// <param name="minLength">Minimum length for strings and collections (optional).</param>
    /// <param name="maxLength">Maximum length for strings and collections (optional).</param>
    /// <param name="minValue">Minimum value for numeric types (optional).</param>
    /// <param name="maxValue">Maximum value for numeric types (optional).</param>
    /// <param name="regexPattern">Regular expression pattern for string validation (optional).</param>
    /// <exception cref="ArgumentNullException">Thrown when null validation fails.</exception>
    /// <exception cref="ArgumentException">Thrown when other validation rules fail.</exception>
    public static void Validate(
        object? value,
        ValidationRule validation = ValidationRule.NotNull,
        string parameterName = "value",
        int minLength = -1,
        int maxLength = -1,
        double minValue = double.MinValue,
        double maxValue = double.MaxValue,
        string? regexPattern = null)
    {
        var attribute = new ValidateParamAttribute(validation, minLength, maxLength, minValue, maxValue, regexPattern);
        attribute.Validate(value, parameterName);
    }
}