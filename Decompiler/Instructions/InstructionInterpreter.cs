using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompiler.Instructions
{
    [DebuggerDisplay("{Mnemonic}")]
    public abstract class InstructionInterpreter
    {
        public abstract string Mnemonic { get; }
        public abstract bool PrefixInstruction { get; }

        public InstructionInterpreter()
        { 
        }

        /// <summary>
        /// Should return consumed bytes count or zero if not recognized. If not recognized also should put back partially consumed bytes.
        /// </summary>
        /// <param name="byteStream">Source byte stream</param>
        /// <param name="prefixes">Collection of accumulated prefix instructions</param>
        /// <param name="interpretationText">Text form of consumed bytes, interpreted into instruction. Should be null when interpretation error</param>
        /// <returns></returns>
        public abstract int Interpret(StreamReader byteStream, List<InstructionInterpreter> prefixes, out string interpretationText);
    }
}
