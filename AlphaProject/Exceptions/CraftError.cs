using System;

namespace AlphaProject.Exceptions;

/// <summary>
/// Error thrown when an error occurs inside a command.
/// </summary>
internal class CraftError : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CraftError"/> class.
    /// </summary>
    /// <param name="message">Message to show.</param>
    public CraftError(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CraftError"/> class.
    /// </summary>
    /// <param name="message">Message to show.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CraftError(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
