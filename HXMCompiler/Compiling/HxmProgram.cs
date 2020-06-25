using System;
using System.Collections.Generic;

namespace HXMCompiler
{
    /// <summary>
    /// Defines a HXM program.
    /// </summary>
    public class HxmProgram
    {
        /// <summary>
        /// The programs title. (metadata)
        /// </summary>
        public string title = "";

        /// <summary>
        /// The programs author. (metadata)
        /// </summary>
        public string author = "";

        /// <summary>
        /// The prorgrams description. (metadata)
        /// </summary>
        public string description = "";

        /// <summary>
        /// The programs version. (metadata)
        /// </summary>
        public string version = "";

        /// <summary>
        /// The programs copyright info. (metadata)
        /// </summary>
        public string copyright = "";

        /// <summary>
        /// All of the embedded resources.
        /// </summary>
        public List<byte[]> resources = new List<byte[]>();

        /// <summary>
        /// All of the executable instructions.
        /// </summary>
        public List<HxmInstruction> instructions = new List<HxmInstruction>();

        /// <summary>
        /// The "HXM" signature in ASCII.
        /// </summary>
        private static readonly byte[] signature = { 72, 88, 77 };

        /// <summary>
        /// The "RES" signature in ASCII.
        /// </summary>
        private static readonly byte[] resourceSignature = { 82, 69, 83 };

        /// <summary>
        /// The "EXEC" signature in ASCII.
        /// </summary>
        private static readonly byte[] execSignature = { 69, 88, 69, 67 };

        /// <summary>
        /// Builds this program to binary format.
        /// </summary>
        /// <returns>The program, in binary format.</returns>
        public byte[] Build(bool verbose = false)
        {
            /*
              A header of a HXM file is as follows:
                +------------------------------------------------------------+
                | Chunk Description         | Size (bytes)       | Data type |
                | ------------------------- | ------------------ | --------- |
                | "HXM" in ASCII            | 3                  | char[]    | 
                | Title string length       | 1                  | uint8     |
                | Title                     | As specified above | string    |
                | Author string length      | 1                  | uint8     |
                | Author                    | As specified above | string    |
                | Description string length | 1                  | uint8     |
                | Description               | As specified above | string    |
                | Version string length     | 1                  | uint8     |
                | Version                   | As specified above | string    |
                | Copyright string length   | 1                  | uint8     |
                | Copyright                 | As specified above | string    |
                +------------------------------------------------------------+
             */

            if (verbose) ConsoleExt.WriteLine("Building program to binary format.", ConsoleColor.Blue);
            List<byte> build = new List<byte>(signature);

            // Add metadata
            if (verbose) ConsoleExt.WriteLine("Embedding metadata.");
            build.Add((byte)title.Length);
            build.AddRange(title.ToAsciiBytes());

            build.Add((byte)author.Length);
            build.AddRange(author.ToAsciiBytes());

            build.AddRange(BitConverter.GetBytes((ushort)description.Length));
            build.AddRange(description.ToAsciiBytes());

            build.Add((byte)version.Length);
            build.AddRange(author.ToAsciiBytes());

            build.Add((byte)copyright.Length);
            build.AddRange(copyright.ToAsciiBytes());

            // Add resources

            build.AddRange(resourceSignature); // Add "RES" signature
            build.AddRange(BitConverter.GetBytes((ushort)resources.Count));

            if (verbose) ConsoleExt.WriteLine("Embedding resources.");

            /*
                An embedded resource looks like this:
                +----------------------------------------------------+
                | Chunk Description | Size (bytes)       | Data type |
                | ----------------- | ------------------ | --------- |
                | Resource size     | 2                  | ushort    |
                | Resource data     | As specified above | any       |
                +----------------------------------------------------+
             */

            for (int i = 0; i < resources.Count; i++)
            {
                if (verbose) ConsoleExt.WriteLine($"Embedding resource #{i}.");

                // Add resource size
                build.AddRange(BitConverter.GetBytes((ushort)resources[i].Length));

                // Add the actual resource data
                build.AddRange(resources[i]);
            }

            if (verbose) ConsoleExt.WriteLine("Embedding op-codes.");

            // Add executable code

            build.AddRange(execSignature); // Add "EXEC" signature

            foreach (HxmInstruction instruction in instructions)
            {
                build.Add((byte)instruction.opCode);
                build.AddRange(
                    BitConverter.GetBytes(
                        (ushort)HxmInstruction.opCodeArguments.
                        GetValueOrDefault(
                            instruction.opCode,
                            new HxmInstruction.Type[] { })
                        .Length)
                    );

                build.AddRange(instruction.arguments);
            }

            if (verbose) ConsoleExt.WriteLine("Compiling done.", ConsoleColor.Green);

            return build.ToArray();
        }


    }
}
