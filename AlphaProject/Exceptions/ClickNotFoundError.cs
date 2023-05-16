using System;

namespace AlphaProject.Exceptions
{
    public class ClickNotFoundError : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClickNotFoundError"/> class.
        /// </summary>
        /// <param name="message">Error message.</param>
        public ClickNotFoundError(string message)
            : base(message)
        {
        }
    }
}
