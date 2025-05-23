using System.Numerics;

namespace Abstractions.Domain.Exceptions;

public sealed class NegativeOrZeroException : Exception
{
    private NegativeOrZeroException(string message) : base(message)
    {
    }

    /// <summary>
    /// Throw an exception
    /// </summary>
    /// <param name="value">Comparable value</param>
    /// <param name="parameterName">Name of parameter</param>
    /// <param name="message">Additional message</param>
    /// <typeparam name="T">Numeric type</typeparam>
    public static void ThrowIfNegativeOrZero<T>(T value, string? parameterName = null, string? message = null)
        where T : INumber<T>
    {
        if (value <= T.Zero)
        {
            throw new NegativeOrZeroException($"{
                message
                ?? $"Parameter {(parameterName is not null ? $"'{parameterName}'" : parameterName)} cannot be less than or equal to {T.Zero}."
            }");
        }
    }

    /// <summary>
    /// Throw an exception
    /// </summary>
    /// <param name="value">Comparable value</param>
    /// <param name="parameterName">Name of parameter</param>
    /// <param name="message">Additional message</param>
    /// <typeparam name="T">Numeric type</typeparam>
    public static void ThrowIfNegative<T>(T? value, string? parameterName = null, string? message = null)
        where T : INumber<T>
    {
        if (value < T.Zero)
        {
            throw new NegativeOrZeroException($"{
                message
                ?? $"Parameter {(parameterName is not null ? $"'{parameterName}'" : parameterName)} cannot be less than or equal to {T.Zero}."
            }");
        }
    }
}
