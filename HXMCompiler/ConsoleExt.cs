using System;
using System.Collections.Generic;
using System.Text;

namespace HXMCompiler
{
    /// <summary>
    /// An extended version of the console, used by the CLI.
    /// </summary>
    internal static class ConsoleExt
    {
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream with the specified color.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="color">The color of the line.</param>
        internal static void WriteLine(string value, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ResetColor();
        }
    }
}
