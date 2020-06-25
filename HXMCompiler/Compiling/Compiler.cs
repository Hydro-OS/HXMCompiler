using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static HXMCompiler.HxmInstruction;
using HXMCompiler.Exceptions;
using System.Threading;

namespace HXMCompiler
{
    /// <summary>
    /// Main compiler class.
    /// </summary>
    public class Compiler
    {
        /// <summary>
        /// The prefix of all comments.
        /// </summary>
        public string CommentPrefix = "//";

        /// <summary>
        /// The prefix of all metadata entries.
        /// </summary>
        public string MetadataPrefix = "#";

        /// <summary>
        /// Should the compiler ignore all errors, and just skip the lines with errors?
        /// </summary>
        public bool ignoreErrors = false;

        /// <summary>
        /// Is the compiler in verbose mode?
        /// </summary>
        public bool verbose = false;

        /// <summary>
        /// Compiles an HXMASM file.
        /// </summary>
        /// <param name="file">The HXM assembly file, split by new lines.</param>
        /// <returns>The compiled program.</returns>
        /// <exception cref="InvalidMetadataEntryException">An invalid metadata entry exists in the source code.</exception>
        public HxmProgram Compile(string[] file)
        {
            ConsoleExt.WriteLine($"Starting compilng file.", ConsoleColor.Blue);
            ConsoleExt.WriteLine($"Lines: {file.Length}");
            HxmProgram program = new HxmProgram();

            // Iterate through all lines
            for (int i = 0; i < file.Length; i++)
            {
                // Ignore all comments
                int index = file[i].IndexOf(CommentPrefix);
                if (index > 0)
                    file[i] = file[i].Substring(0, index);

                if (string.IsNullOrWhiteSpace(file[i]) || file[i].StartsWith(CommentPrefix)) continue; // Skip only new lines and comment lines

                // Parse metadata
                if (file[i].StartsWith(MetadataPrefix))
                {
                    if (verbose) ConsoleExt.WriteLine("Compiling metadata entry.");
                    try
                    {
                        ParseMetadata(file[i], program);
                    }
                    catch(InvalidMetadataEntryException invalidMetadata)
                    {
                        if (ignoreErrors) {
                            ConsoleExt.WriteLine($"Ignoring error: {invalidMetadata.Message}", ConsoleColor.Yellow);
                            continue; 
                        }
                        else throw new InvalidMetadataEntryException(invalidMetadata.Message + $"(line {i})");
                    }
                }
                else if (file[i].ToUpper().StartsWith("RESOURCES"))
                    // Embed resource block
                    EmbedResources(program, file, ref i);
                else
                {
                    // Compile op-code

                    // Get the op-code in form of a string.
                    string instruction = file[i].Substring(0, file[i].IndexOf(' '));

                    // Verbose messages
                    if (verbose) ConsoleExt.WriteLine($"Compiling opcode on line: {i}, opcode: {instruction}");
                    if (verbose) ConsoleExt.WriteLine($"Instruction: {file[i]}", ConsoleColor.DarkGray);

                    // Split the instruction by commas. This will return an array of arguments.
                    // If no arguments would be detected, it would return an array with one element, that is an empty string.
                    string[] args = file[i].Substring(instruction.Length).Trim().Split(',');

                    // If no arguments are present, change it to an empty array
                    if (args.Length == 1 && string.IsNullOrEmpty(args[0])) args = new string[0];

                    OpCode opCode;

                    // Try to get the opcode
                    try
                    {
                        opCode = (OpCode)Enum.Parse(typeof(OpCode), instruction);
                    }
                    catch (ArgumentException) {
                        // Enum.Parse couldn't parse the string -- that means that the op-code is invalid
                        if (ignoreErrors)
                        {
                            ConsoleExt.WriteLine($"Ignoring error: Op-code \"{instruction}\" not recognized. (line {i})", ConsoleColor.Yellow);
                            continue;
                        }
                        throw new InvalidOpcodeException($"Op-code \"{instruction}\" not recognized. (line {i})");
                    }

                    // This list will contain all of the arguments, in byte form. We will be attaching this directly into the binary.
                    List<byte> byteArgs = new List<byte>();

                    // Get arguments for the specified opcode; if the opcode does not have any arguments, this will be an empty array.
                    HxmInstruction.Type[] argTypes = opCodeArguments.GetValueOrDefault(opCode, noType);

                    // Check if the arguments for the opcode are correct
                    if (args.Length != argTypes.Length)
                    {
                        if (ignoreErrors)
                        {
                            ConsoleExt.WriteLine($"Ignoring error: The opcode {instruction} requires {argTypes.Length} instructions, while {args.Length} were provided (line {i})", ConsoleColor.Yellow);
                            continue;
                        }
                        else throw new InvalidParameterCountException($"The opcode {instruction} requires {argTypes.Length} instructions, while {args.Length} were provided (line {i})");
                    }
                        
                    for (int argi = 0; argi < argTypes.Length; argi++)
                    {
                        // Parse arguments from string to correct type (Parse will automatically convert the correct type to a byte array) and add the bytes to the raw argument list
                        byteArgs.AddRange(Parse(argTypes[argi], args[argi]));
                    }

                    // Create actual instruction
                    HxmInstruction hxmInstruction = new HxmInstruction(opCode, byteArgs.ToArray());

                    // Add to instructions
                    program.instructions.Add(hxmInstruction);
                }
            }

            return program;
        }

        /// <summary>
        /// Parses metadata.
        /// </summary>
        /// <param name="metadataLine">The metadata line (with the prefix)</param>
        /// <param name="program">The target program.</param>
        private void ParseMetadata(string metadataLine, HxmProgram program)
        {
            // A metadata entry can either have no value, or one value.
            // The difference between a metadata entry and a resource entry, is that there can be unlimited resources, and the count of metadata entries are fixed.
            // Metadata entries only inform the OS or the compiler about things like the name of the program, the author, etc.
            // The program can't access it's own metadata using op-codes. (getting metadata thru syscalls can work, though)

            // The first element of this array is the metadata entry name.
            // All other elements of the array are the value.
            // If there isn't any colon in the metadata value, there will only be 2 elements in this array.
            // Still, you should use string.Join(":", ...) to join all other elements that have been split because they had a colon.
            string[] metadata = metadataLine.
                Substring(MetadataPrefix.Length). // Remove prefix
                Split(':'); // Split value and metadata value name

            switch (metadata[0].ToUpper())
            {
                case "TITLE":
                    program.title = string.Join(":", metadata.Skip(1));
                    program.title = program.title.TrimStart();
                    break;
                case "AUTHOR":
                    program.author = string.Join(":", metadata.Skip(1));
                    program.author = program.author.TrimStart();
                    break;
                case "VERSION":
                    program.version = string.Join(":", metadata.Skip(1));
                    program.version = program.version.TrimStart();
                    break;
                case "DESCRIPTION":
                    program.description = string.Join(":", metadata.Skip(1));
                    program.description = program.description.TrimStart();
                    break;
                case "IGNORE_COMPILER_ERRORS":
                case "IGNORECOMPILERERRORS":
                    ConsoleExt.WriteLine("WARNING: Compiler errors will now be ignored.", ConsoleColor.Yellow);
                    ignoreErrors = true;
                    break;
                case "ENABLE_COMPILER_ERRORS":
                case "ENABLECOMPILERERRORS":
                    Console.WriteLine("Compiler errors now are not ignored.");
                    ignoreErrors = false;
                    break;
                default:
                    throw new InvalidMetadataEntryException($"Metadata entry \"{metadata[0]}\" is invalid.");
            }
        }

        /// <summary>
        /// Embeds all resources in a block, beggining at the start of the specified line, in the specified file.
        /// </summary>
        /// <param name="program">The target program.</param>
        /// <param name="file">The file that the block is located in.</param>
        /// <param name="line">The line iterator. Should start where the block starts.</param>
        private void EmbedResources(HxmProgram program, string[] file, ref int line)
        {
            // This will embed all resources in the specified file, on the line.
            // This method is designed for compiling the program line-by-line.

            ConsoleExt.WriteLine($"Embedding resources -- resource block started at line {line}.");
            for (int resource = 0; true; resource++)
            {
                // Check if the resource block has ended
                if (file[line + resource + 1].ToUpper() == "END")
                {
                    ConsoleExt.WriteLine($"Embedding resources finished -- resource block ended at line {line}.");
                    // We've iterated through the whole block -- add the line count of the block to the line count, because we've processed those lines
                    line += resource + 1;
                    break;
                }

                // This will return an array, where the first element is the type, and the 2nd element is the actual resource.
                string[] res = file[line + resource + 1].TrimStart('\t').Split(':');

                // We need to convert from a string to an actual type, and then we need to get the bytes of the object
                switch (res[0].ToUpper())
                {
                    case "STRING":
                    case "STR":
                        if (verbose) ConsoleExt.WriteLine($"Metadata #{resource} from current block embedded. (type: string)");
                        // Ascii string -> bytes
                        program.resources.Add(Encoding.ASCII.GetBytes(string.Join("", res.Skip(1))));
                        break;
                    case "INT":
                    case "INTEGER":
                    case "INT32":
                        if (verbose) ConsoleExt.WriteLine($"Metadata #{resource} from current block embedded. (type: int32)");
                        program.resources.Add(BitConverter.GetBytes(int.Parse(res[1])));
                        break;
                    case "LONG":
                    case "INT64":
                        if (verbose) ConsoleExt.WriteLine($"Metadata #{resource} from current block embedded. (type: int64)");
                        program.resources.Add(BitConverter.GetBytes(long.Parse(res[1])));
                        break;
                    case "SHORT":
                    case "INT16":
                        if (verbose) ConsoleExt.WriteLine($"Metadata #{resource} from current block embedded. (type: int16)");
                        program.resources.Add(BitConverter.GetBytes(short.Parse(res[1])));
                        break;
                    case "UINT":
                    case "UINTEGER":
                    case "UINT32":
                        if (verbose) ConsoleExt.WriteLine($"Metadata #{resource} from current block embedded. (type: uint32)");
                        program.resources.Add(BitConverter.GetBytes(uint.Parse(res[1])));
                        break;
                    case "ULONG":
                    case "UINT64":
                        if (verbose) ConsoleExt.WriteLine($"Metadata #{resource} from current block embedded. (type: uint64)");
                        program.resources.Add(BitConverter.GetBytes(ulong.Parse(res[1])));
                        break;
                    case "USHORT":
                    case "UINT16":
                        if (verbose) ConsoleExt.WriteLine($"Metadata #{resource} from current block embedded. (type: uint16)");
                        program.resources.Add(BitConverter.GetBytes(ushort.Parse(res[1])));
                        break;
                    case "BYTE":
                        if (verbose) ConsoleExt.WriteLine($"Metadata #{resource} from current block embedded. (type: byte)");
                        // This wil parse the binary *representation* of the byte.
                        // It's different from a int8 -- it would look like this: 00000000
                        // It will ignore spaces so you can safely do something like this: 0000 0000
                        program.resources.Add(new byte[] { BinaryRepresentationToByte(res[1]) });
                        break;
                }
            }
        }

        /// <summary>
        /// Parses a data type and it's string representation and returns it bytes.
        /// </summary>
        /// <param name="type">The type of the value.</param>
        /// <param name="value">The actual value.</param>
        /// <returns>The bytes of the parsed value.</returns>
        public static byte[] Parse(HxmInstruction.Type type, string value)
        {
            return type switch
            {
                HxmInstruction.Type.UInt => BitConverter.GetBytes(uint.Parse(value)),
                HxmInstruction.Type.UShort => BitConverter.GetBytes(ushort.Parse(value)),
                HxmInstruction.Type.Byte => new byte[] { byte.Parse(value) },
                HxmInstruction.Type.Int => BitConverter.GetBytes(int.Parse(value)),
                HxmInstruction.Type.Short => BitConverter.GetBytes(short.Parse(value)),
                // The C# char is normally 16-bits wide, but an ASCII char is 8-bits wide.
                HxmInstruction.Type.Char => Encoding.ASCII.GetBytes(new char[] { char.Parse(value) }),
                HxmInstruction.Type.WideChar => BitConverter.GetBytes(char.Parse(value)),
                HxmInstruction.Type.Float => BitConverter.GetBytes(float.Parse(value)),
                HxmInstruction.Type.Double => BitConverter.GetBytes(double.Parse(value)),
                HxmInstruction.Type.Long => BitConverter.GetBytes(long.Parse(value)),
                HxmInstruction.Type.ULong => BitConverter.GetBytes(ulong.Parse(value)),
                // Unknown type, fallback to default string encoding
                _ => Encoding.Default.GetBytes(value),
            };
        }

        /// <summary>
        /// Converts an string binary representation of a byte to an <see cref="byte"/> object.
        /// </summary>
        /// <param name="input">The string binary representation of the byte, in the format of 00000000. Will ignore spaces.</param>
        /// <returns>The binary string, converted to a byte.</returns>
        private static byte BinaryRepresentationToByte(string input)
        {
            return Convert.ToByte(
                input.
                Replace(" ", ""). // Ignore all spaces
                Substring(0, 8), // Make the string exactly 8 characters
            fromBase: 2); // Convert to byte
        }
    }
}
