using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompiler.Instructions
{
    public sealed class Add : InstructionInterpreter
    {
        public override string Mnemonic { get { return "add"; } }
        public override bool PrefixInstruction { get { return false; } }

        public override int Interpret(StreamReader byteStream, List<InstructionInterpreter> prefixes, out string interpretationText)
        {
            int consumedCount = 0;
            switch (byteStream.Peek())
            {
                case 0x00: //add r/m8, r8, both operands in modr/m, valid with LOCK
                    consumedCount++;
                    break;
            }
            interpretationText = "";
            return 0;
        }
    }
}
