using Mpv.Core.Enums.Client;

namespace Mpv.Core;

/// <summary>
/// Represents an exception thrown by the MPV library.
/// </summary>
#pragma warning disable RCS1194 // Implement exception constructors
public class MpvException : Exception
#pragma warning restore RCS1194 // Implement exception constructors
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MpvException"/> class.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="code">Error code.</param>
    public MpvException(string? message = null, MpvError? code = null)
        : base(message)
    {
        Code = code;
    }

    public MpvError? Code { get; }
}
