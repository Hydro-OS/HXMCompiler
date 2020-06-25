using System;

/*
 * All exceptions here can be thrown by the compiler.
 * To catch every exception related to the compiler, simply catch CompilerException.
 */

namespace HXMCompiler.Exceptions
{
    /// <summary>
    /// Defines a compiler exception.
    /// </summary>
    [Serializable]
    public class CompilerException : Exception
    {
        public CompilerException() { }
        public CompilerException(string message) : base(message) { }
        public CompilerException(string message, Exception inner) : base(message, inner) { }
        protected CompilerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Specifies an exception that is thrown when a metadata entry is invalid.
    /// </summary>
    [Serializable]
    public class InvalidMetadataEntryException : CompilerException
    {
        public InvalidMetadataEntryException() { }
        public InvalidMetadataEntryException(string message) : base(message) { }
        public InvalidMetadataEntryException(string message, Exception inner) : base(message, inner) { }
        protected InvalidMetadataEntryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Specifies an exception that is thrown when an opcode is unrecognized.
    /// </summary>
    [Serializable]
    public class InvalidOpcodeException : CompilerException
    {
        public InvalidOpcodeException() { }
        public InvalidOpcodeException(string message) : base(message) { }
        public InvalidOpcodeException(string message, Exception inner) : base(message, inner) { }
        protected InvalidOpcodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an embedded resource decleration is invalid.
    /// </summary>
    [Serializable]
    public class InvalidResourceException : CompilerException
    {
        public InvalidResourceException() { }
        public InvalidResourceException(string message) : base(message) { }
        public InvalidResourceException(string message, Exception inner) : base(message, inner) { }
        protected InvalidResourceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when trying to call and opcode, while providing the wrong parameter count.
    /// </summary>
    [Serializable]
    public class InvalidParameterCountException : CompilerException
    {
        public InvalidParameterCountException() { }
        public InvalidParameterCountException(string message) : base(message) { }
        public InvalidParameterCountException(string message, Exception inner) : base(message, inner) { }
        protected InvalidParameterCountException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
