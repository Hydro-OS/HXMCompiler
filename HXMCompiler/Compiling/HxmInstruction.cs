using System;
using System.Collections.Generic;
using System.Text;

namespace HXMCompiler
{
    /// <summary>
    /// Represents a HXM instruction.
    /// </summary>
    public class HxmInstruction
    {
        /// <summary>
        /// The op-code for this instruction.
        /// </summary>
        public OpCode opCode;

        /// <summary>
        /// All of the arguments for the op-code.
        /// </summary>
        public byte[] arguments;

        /// <summary>
        /// Defines an instruction that has no input.
        /// </summary>
        public static readonly Type[] noType = new Type[0];

        /// <summary>
        /// Creates a new HXM instruction.
        /// </summary>
        /// <param name="opCode">The op-code.</param>
        /// <param name="args">The arguments, in byte array form.</param>
        public HxmInstruction(OpCode opCode, byte[] args)
        {
            this.opCode = opCode;
            arguments = args;
        }

        public byte[] Build()
        {
            List<byte> builder = new List<byte>
            {
                // Add starting byte
                (byte)opCode
            };

            builder.AddRange(BitConverter.GetBytes(arguments.Length));
            builder.AddRange(arguments);

            return builder.ToArray();
        }

        /// <summary>
        /// Gets the byte size of a type.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The size, in bytes, of the specified type.</returns>
        public static ushort GetByteSize(Type type)
        {
            switch (type)
            {
                case Type.Float:
                case Type.Int:
                case Type.UInt:
                    return 4;
                case Type.Short:
                case Type.UShort:
                case Type.WideChar:
                    return 2;
                case Type.Byte:
                case Type.Char:
                    return 1;
                case Type.ULong:
                case Type.Long:
                case Type.Double:
                    return 8;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Represents an op-code.
        /// </summary>
        public enum OpCode : byte
        {
            syscall,
            var,
            subvar,
            addvar,
            mulvar,
            divvar,
            setvar,
            setacc,
            subacc,
            addacc,
            mulacc,
            divacc,
            jmp,
            jmpvar,
            delvar,
            jmpequ,
            jmpneq,
            jmpg,
            jmpge,
            jmpl,
            jmple,
            jmpequvar,
            jmpneqvar,
            jmpgvar,
            jmpgevar,
            jmplvar,
            jmplevar,
            inc,
            incvar,
            dec,
            decvar,
            varcpy,
            noop,
            andvar,
            and,
            andaccvar,
            orvar,
            or,
            oraccvar,
            xorvar,
            xor,
            xoraccvar,
            lshiftvar,
            lshift,
            lshiftaccvar,
            rshiftvar,
            rshift,
            rshiftaccvar,
            notvar,
            not,
            notaccvar,
            syscallret,
            syscallacc,
            wait,
            waitvar,
            end,
            endequ,
            endneq,
            endg,
            endge,
            endl,
            endle,
            endequvar,
            endneqvar,
            endgvar,
            endgevar,
            endlvar,
            endlevar,
            mod,
            modvar,
            modacc,
            modacco,
            syscallvar,
            scaappend,
            scaappendacc,
            scaclear,
            varappend,
            changeacc,
            changevar,
            varappendb,
            appendacc,
            appendaccvar,
            varappendacc,
            popstack,
            pushstack,
            popstackvar,
            pushstackvar,
            initstack,
            unloadstack,
            delayjmp,
            delayjmpvar,
            lipk,
            lipkvar,
            prev,
            back,
            backvar,
            setaccbyte,
            setaccbytevar,
            setvarbyte,
            setvarbytevar,
            setbyteconstpos,
            setbyteconstval,
            resetacc,
            trimacc,
            trimaccvar,
            skipacc,
            skipaccvar,
            trimvar,
            trimvarvar,
            skipvar,
            skipvarvar,
            loadres,
            loadresvar,
            f2xm1,
            f2xm1var,
            f2xm1acc,
            fabs,
            fabsvar,
            fabsacc,
            frndint,
            frndintvar,
            frndintacc,
            fadd,
            fsub,
            fmul,
            fdiv,
            fsqrt,
            fchs,
            fchscpy,
            ftan,
            fpatan,
            fjmpequ,
            fjmpneq,
            fjmpg,
            fjmpge,
            fjmpl,
            fjmple,
            fjmpequvar,
            fjmpneqvar,
            fjmpgvar,
            fjmpgevar,
            fjmplvar,
            fjmplevar




        }

        /// <summary>
        /// Defines the type of a parameter or a variable, which size can be defined.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Unsigned integer. (uint32)
            /// </summary>
            UInt,
            /// <summary>
            /// Unsigned short. (uint16)
            /// </summary>
            UShort,
            /// <summary>
            /// A byte. (uint8)
            /// </summary>
            Byte,
            /// <summary>
            /// Signed integer. (int32)
            /// </summary>
            Int,
            /// <summary>
            /// Signed short. (int16)
            /// </summary>
            Short,
            /// <summary>
            /// ASCII Character. (char8_t)
            /// </summary>
            Char,
            /// <summary>
            /// Unicode character. (char16_t)
            /// </summary>
            WideChar,
            /// <summary>
            /// Single floating-point value. (float32)
            /// </summary>
            Float,
            /// <summary>
            /// Double floating-point value (float64)
            /// </summary>
            Double,
            /// <summary>
            /// A long integer. (int64)
            /// </summary>
            Long,
            /// <summary>
            /// An unsigned long integer. (uint64)
            /// </summary>
            ULong
        }

        private static readonly Type[] oneVarId = new Type[] { Type.UInt };
        private static readonly Type[] twoVarIds = new Type[] { Type.UInt, Type.UInt };
        private static readonly Type[] jmpParams = new Type[] { Type.UInt, Type.UInt, Type.UInt };

        /// <summary>
        /// The list of all required arguments. Op-codes with no parameters are not listed.
        /// </summary>
        public static readonly Dictionary<OpCode, Type[]> opCodeArguments = new Dictionary<OpCode, Type[]>()
        {
            { OpCode.syscall, new Type[]{ Type.UShort } },
            { OpCode.var, oneVarId},
            { OpCode.subvar, oneVarId },
            { OpCode.addvar, oneVarId },
            { OpCode.mulvar, oneVarId },
            { OpCode.divvar, oneVarId },
            { OpCode.setvar, oneVarId },
            { OpCode.setacc, oneVarId },
            { OpCode.subacc, oneVarId },
            { OpCode.addacc, oneVarId },
            { OpCode.mulacc, oneVarId },
            { OpCode.divacc, oneVarId },
            { OpCode.jmpvar, oneVarId },
            { OpCode.delvar, oneVarId },

            { OpCode.jmpequ, twoVarIds },
            { OpCode.jmpneq, twoVarIds },
            { OpCode.jmpg, twoVarIds },
            { OpCode.jmpge, twoVarIds },
            { OpCode.jmpl, twoVarIds },
            { OpCode.jmple, twoVarIds },

            { OpCode.jmpequvar, jmpParams },
            { OpCode.jmpneqvar, jmpParams },
            { OpCode.jmpgvar, jmpParams },
            { OpCode.jmpgevar, jmpParams },
            { OpCode.jmplvar, jmpParams },
            { OpCode.jmplevar, jmpParams },

            { OpCode.incvar, oneVarId },
            { OpCode.decvar, oneVarId },
            { OpCode.varcpy, twoVarIds },

            { OpCode.andvar, twoVarIds },
            { OpCode.and, oneVarId },
            { OpCode.orvar, twoVarIds },
            { OpCode.or, oneVarId },
            { OpCode.xorvar, twoVarIds },
            { OpCode.xor, oneVarId },

            { OpCode.lshiftvar, twoVarIds },
            { OpCode.lshift, oneVarId },
            { OpCode.lshiftaccvar, oneVarId },

            { OpCode.rshiftvar, twoVarIds },
            { OpCode.rshift, oneVarId },
            { OpCode.rshiftaccvar, oneVarId },

            { OpCode.notvar, twoVarIds },
            { OpCode.not, oneVarId },
            { OpCode.notaccvar, oneVarId },

            { OpCode.syscallret, new Type[] { Type.UShort, Type.UInt } },
            { OpCode.syscallacc, new Type[] { Type.UShort } },

            { OpCode.waitvar, oneVarId },

            { OpCode.endequ, oneVarId },
            { OpCode.endneq, oneVarId },
            { OpCode.endg, oneVarId },
            { OpCode.endge, oneVarId },
            { OpCode.endl, oneVarId },
            { OpCode.endle, oneVarId },

            { OpCode.endequvar, twoVarIds },
            { OpCode.endneqvar, twoVarIds },
            { OpCode.endgvar, twoVarIds },
            { OpCode.endgevar, twoVarIds },
            { OpCode.endlvar, twoVarIds },
            { OpCode.endlevar, twoVarIds },

            { OpCode.mod, twoVarIds },
            { OpCode.modvar, new Type[]{ Type.UInt, Type.UInt, Type.UInt } },
            { OpCode.modacc, twoVarIds },
            { OpCode.modacco, oneVarId },

            { OpCode.syscallvar, oneVarId },
            { OpCode.scaappend, oneVarId },

            { OpCode.varappend, oneVarId },
            { OpCode.changeacc, new Type[] { Type.Byte } },
            { OpCode.changevar, new Type[] { Type.Byte } },
            { OpCode.varappendb, new Type[] { Type.Byte } },

            { OpCode.appendacc, oneVarId },
            { OpCode.appendaccvar, oneVarId },
            { OpCode.varappendacc, oneVarId },

            { OpCode.popstackvar, oneVarId },
            { OpCode.pushstackvar, oneVarId },

            { OpCode.delayjmpvar, oneVarId },

            { OpCode.lipkvar, oneVarId },

            { OpCode.backvar, oneVarId },
            { OpCode.setaccbyte, new Type[] { Type.UShort, Type.Byte } },
            { OpCode.setaccbytevar, twoVarIds },

            { OpCode.setvarbyte, new Type[] { Type.UShort, Type.Byte } },
            { OpCode.setvarbytevar, twoVarIds },

            { OpCode.setbyteconstval, new Type[] { Type.UInt, Type.Byte } },
            { OpCode.setbyteconstpos, new Type[] { Type.UShort, Type.UInt } },

            { OpCode.trimacc, new Type[]{ Type.UShort } },
            { OpCode.trimaccvar, oneVarId },
            { OpCode.skipacc, new Type[] {Type.UShort} },
            { OpCode.skipaccvar, oneVarId },

            { OpCode.trimvar, new Type[] {Type.UShort} },
            { OpCode.trimvarvar, oneVarId },

            { OpCode.skipvar, new Type[] { Type.UShort} },
            { OpCode.skipvarvar, oneVarId },

            { OpCode.loadres, new Type[] { Type.UShort} },
            { OpCode.loadresvar, new Type[] {Type.UShort, Type.UInt} },

            // FLOATING POINT OPCODES
            { OpCode.f2xm1, oneVarId },
            { OpCode.f2xm1var, twoVarIds },
            { OpCode.f2xm1acc, oneVarId },

            { OpCode.fabs, oneVarId },
            { OpCode.fabsvar, twoVarIds },
            { OpCode.fabsacc, oneVarId },

            { OpCode.frndint, oneVarId },
            { OpCode.frndintvar, twoVarIds },
            { OpCode.frndintacc, oneVarId },

            { OpCode.fadd, oneVarId },
            { OpCode.fsub, oneVarId },
            { OpCode.fmul, oneVarId },
            { OpCode.fdiv, oneVarId },

            { OpCode.fsqrt, oneVarId },
            { OpCode.fchs, oneVarId },
            { OpCode.fchscpy, oneVarId },

            { OpCode.ftan, oneVarId },
            { OpCode.fpatan, oneVarId },

            { OpCode.fjmpequ, jmpParams  },
            { OpCode.fjmpneq, jmpParams  },
            { OpCode.fjmpg, jmpParams },
            { OpCode.fjmpge, jmpParams  },
            { OpCode.fjmpl, jmpParams },
            { OpCode.fjmple, jmpParams  },

            { OpCode.fjmpequvar, jmpParams  },
            { OpCode.fjmpneqvar, jmpParams  },
            { OpCode.fjmpgvar, jmpParams  },
            { OpCode.fjmpgevar, jmpParams  },
            { OpCode.fjmplvar, jmpParams },
            { OpCode.fjmplevar, jmpParams }
        };
    }
}
