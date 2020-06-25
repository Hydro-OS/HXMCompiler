using CommandLine;
using HXMCompiler.Exceptions;
using System;
using System.IO;
using System.Reflection;

/*
 * If you are using this project as a library for your own project, you can safely delete this file.
 * This file only serves as the CLI for the compiler.
 */

namespace HXMCompiler
{
    /// <summary>
    /// The main program entry point.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The CLI's options, used by the command line parser.
        /// </summary>
        private sealed class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Sets the compiler verbose mode.")]
            public bool Verbose { get; set; }

            [Option('c', "comment_prefix", Required = false, Default = "//", HelpText = "Sets the comment prefix.")]
            public string CommentPrefix { get; set; }

            [Option('m', "metadata_prefix", Required = false, Default = "#", HelpText = "Sets the metadata entry prefix.")]
            public string MetadataPrefix { get; set; }

            [Option('i', "input", Required = true, HelpText = "The input file.")]
            public string Input { get; set; }

            [Option('o', "output", Required = true, HelpText = "The output file.")]
            public string Output { get; set; }
        }

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">All of the arguments provided by the user.</param>
        /// <returns>A result code; 0 if everything went right, and -1 if an error occured.</returns>
        static int Main(string[] args)
        {
            ConsoleExt.WriteLine($"HydroOS Executable Managed Assembly Compiler v{Assembly.GetEntryAssembly().GetName().Version}", ConsoleColor.Cyan);

            Compiler compiler = new Compiler();
            int result = 0;

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(options =>
                   {
                       ConsoleExt.WriteLine($"  Input: {options.Input}");
                       ConsoleExt.WriteLine($"  Output: {options.Output}");

                       if (compiler.verbose)
                       {
                           ConsoleExt.WriteLine($"  Verbose mode turned on.");
                           ConsoleExt.WriteLine($"  Comment prefix: {options.CommentPrefix}");
                           ConsoleExt.WriteLine($"  Metadata prefix: {options.MetadataPrefix}");
                       }

                       compiler.CommentPrefix = options.CommentPrefix;
                       compiler.MetadataPrefix = options.MetadataPrefix;
                       compiler.verbose = options.Verbose;

                       HxmProgram program = null;

                       try
                       {
                           program = compiler.Compile(File.ReadAllLines(options.Input));
                           File.WriteAllBytes(options.Output, program.Build(compiler.verbose));
                       }
                       catch(CompilerException ex)
                       {
                           ConsoleExt.WriteLine($"error: {ex.Message}", ConsoleColor.Red);
                           result = -1;
                           return;
                       }

                       ConsoleExt.WriteLine("Successfully compiled program.", ConsoleColor.Green);
                   });

            return result;
        }
    }
}
