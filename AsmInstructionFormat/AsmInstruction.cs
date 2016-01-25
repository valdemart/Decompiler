using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsmInstructionFormat
{
    public class AsmInstruction
    {
        /// <summary>
        /// Instruction mnemonic
        /// </summary>
        public string Mnemonic { get; set; }
        /// <summary>
        /// Instruction operation code bytes. Number of valid bytes is specified in OpcodeLength
        /// </summary>
        public long Opcode { get; set; }
        /// <summary>
        /// Number of valid opcode bytes in Opcode
        /// </summary>
        public int OpcodeLength { get; set; }
        /// <summary>
        /// Index within Opcode bytes, at which primary opcode is set
        /// </summary>
        public int PrimaryOpcodePosition { get; set; }
        /// <summary>
        /// Is register number 0-7 added to primary opcode. If so, 3 LSB in primary opcode should be ignored in instruction recognition phase
        /// </summary>
        public bool PrimaryOpcodeIncludesRegister { get; set; }
        /// <summary>
        /// Does Opcode include primary opcode extension value. Primary opcode extemsion value is stored in reg field of ModR/M byte. In Opcode property, opcode extension value is stored in last valid byte
        /// </summary>
        public bool PrimaryOpcodeExtension { get; set; }
        /// <summary>
        /// Information about processors this instruction is valid for
        /// </summary>
        public Processors AvailableProcessors { get; set; }
        /// <summary>
        /// Execution modes this instruction is valid in
        /// </summary>
        public ExecutionModes ExecutionModes { get; set; }
        /// <summary>
        /// Is this instruction valid with LOCK prefix
        /// </summary>
        public bool LOCKValid { get; set; }
        /// <summary>
        /// Changes on FPU stack that this instruction includes
        /// </summary>
        public FPUStackChanges FPUStackChange { get; set; }
        /// <summary>
        /// Documentation level for this instruction
        /// </summary>
        public DocumentationLevels DocumentationLevel { get; set; }
        /// <summary>
        /// Instruction operands information
        /// </summary>
        public List<OperandInformation> Operands { get; set; }
    }

    public class OperandInformation
    {
        /// <summary>
        /// Is this operand implicit (not givien in byte codes)
        /// </summary>
        public bool Implicit { get; set; }
        /// <summary>
        /// Is this operand modified by instruction
        /// </summary>
        public bool Modified { get; set; }
        /// <summary>
        /// Operand type
        /// </summary>
        public OperandTypes OperandType { get; set; }
    }

    [Flags]
    public enum Processors
    {
        _8086 = 0x0001,
        _80186 = 0x0002,
        _80286 = 0x0004,
        _80386 = 0x0008,
        _80486 = 0x0010,
        Pentium = 0x0020,
        PentiumMMX = 0x0040,
        PentiumPro = 0x0080,
        PentiumII = 0x0100,
        PentiumIII = 0x0200,
        Pentium4 = 0x0400,
        Core = 0x0800,
        Core2 = 0x1000,
        Corei7 = 0x2000,
        Itanium = 0x4000,
    }

    [Flags]
    public enum ExecutionModes
    {
        RealMode = 0x01,
        ProtectedMode = 0x02,
        _64bitMode = 0x04,
        SMMOnly = 0x08,
    }

    public enum FPUStackChanges
    {
        None,
        Push,
        Pop,
        PopTwice,
    }

    public enum DocumentationLevels
    {
        Documented,
        MarginallyDocumented,
        Undocumented,
    }

    public enum OperandTypes
    {
        //A - direct addres. no modr/m, direct address encoded after instruction opcodes
        /// <summary>
        /// Direct address encoded after opcodes (no ModR/M)
        /// </summary>
        DirectAddress,
        //BA - memory addressed by DS:EAX (32-bit) or by RAX (64-bit)
        /// <summary>
        /// Memory address given by DS:EAX (32-bit) or by RAX (64-bit)
        /// </summary>
        MemoryAddressEAX,
        //BB - memory addressed by DS:EBX+AL (32-bit) or by RBX+AL (64-bit)
        /// <summary>
        /// Memory address given by DS:EBX+AL (32-bit) or by RBX+AL (64-bit)
        /// </summary>
        MemoryAddressEBXAL,
        //BD - memory addressed by DS:EDI (32-bit) or by RDI (64-bit)
        /// <summary>
        /// Memory address given by DS:EDI (32-bit) or by RDI (64-bit)
        /// </summary>
        MemoryAddressEDI,
        //C - reg field in modr/m specifies CR register
        /// <summary>
        /// Control register selected in reg field of ModR/M byte
        /// </summary>
        ControlRegister,
        //D - reg field in modr/m specifies DR register
        /// <summary>
        /// Debug register selected in reg field of ModR/M byte
        /// </summary>
        DebugRegister,
        //E - modr/m follows opcodes and specifies r/m operand. if operand is memory address, address is given by segment register and displacement and/or SIB byte
        /// <summary>
        /// Register/memory selected in r/m field of ModR/M byte and SIB byte (if available)
        /// </summary>
        RegisterMemoryOperand,
        //ES - modr/m follows opcodes and specifies STi/m (E implied)
        /// <summary>
        /// FPU stack register/memory selected in r/m field of ModR/M byte and SIB byte (if available)
        /// </summary>
        FPUStackMemory,
        //EST - modr/m follows opcode and specifies STi (E implied)
        /// <summary>
        /// FPU stack register selected in r/m field of ModR/M byte
        /// </summary>
        FPUStack,
        //F - RFLAGS register
        /// <summary>
        /// FLAGS register (either FLAGS, EFLAGS or RFLAGS)
        /// </summary>
        FLAGS,
        //G - modr/m follows opcodes and specifies r
        /// <summary>
        /// Register selected in reg field of ModR/M byte
        /// </summary>
        Register,
        //H - modr/m follows opcodes and specifies r in r/m bits (Mod part don't care, implicit: Mod=11b)
        /// <summary>
        /// Register selected in r/m field of ModR/M byte (mod part ignored and assumed to be 11b)
        /// </summary>
        RMRegister,
        //I - immediate value encoded in subsequent bytes
        /// <summary>
        /// Immediate value encoded after opcode bytes
        /// </summary>
        Immediate,
        //J - relative offset value, added to IP
        /// <summary>
        /// Relative jump offset value added to IP, encoded after opcode bytes
        /// </summary>
        JumpOffset,
        //M - modr/m follows opcodes and specifies m (Mod part can't be 11b)
        /// <summary>
        /// Memory address selected by r/m field of ModR/M byte and SIB byte (if available) (mod part can't be 11b)
        /// </summary>
        Memory,
        //N - modr/m follows opcodes and specifies packed quadword MMXn register in r/m
        /// <summary>
        /// MMX register selected in r/m field of ModR/M byte
        /// </summary>
        MMXRMRegister,
        //O - offset operand encoded in subsequent word, dword or qword (depending on address size attribute; Intel: moffs)
        /// <summary>
        /// Memory offset encoded after opcode bytes
        /// </summary>
        MemoryOffset,
        //P - modr/m follows opcodes and specifies packed quadword MMXn register in r
        /// <summary>
        /// MMX register selected in reg field of ModR/M byte
        /// </summary>
        MMXRegister,
        //Q - modr/m follows opcodes and specifies MMXn register or memory address (Intel: m64), possibly using SIB
        /// <summary>
        /// MMX register/memory selected in r/m field of ModR/M byte and SIB byte (if available)
        /// </summary>
        MMXMemory,
        //R - modr/m mod field may be only 11b
        /// <summary>
        /// Register selected in r/m field of ModR/M byte (mod part must be equal to 11b, see: RMRegister for mod part ignored)
        /// </summary>
        ForcedRMRegister,
        //S - modr/m follows opcodes and specifies segment register in reg
        /// <summary>
        /// Segment register selected in reg field of ModR/M byte
        /// </summary>
        SegmentRegister,
        //SC - stack operand. size depends on other operands and/or instruction (f.e. IRET, CALL, LEAVE)
        /// <summary>
        /// Stack data which size depends on instruction and/or other operands (f.e. used in IRET, CALL, LEAVE)
        /// </summary>
        StackData,
        //T - reg field in modr/m specifies TR register
        /// <summary>
        /// Task register selected in reg field of ModR/M byte
        /// </summary>
        TaskRegister,
        //U - modr/m follows opcodes and specifies 128-bit XMM register in r/m
        /// <summary>
        /// XMM register selected in r/m field of ModR/M byte
        /// </summary>
        XMMRMRegister,
        //V - modr/m follows opcodes and specifies 128-bit XMM register in reg
        /// <summary>
        /// XMM register selected in reg field of ModR/M byte
        /// </summary>
        XMMRegister,
        //W - modr/m follows opcodes and specifies XMM register or memory address (Intel: xmm/m128)
        /// <summary>
        /// XMM register/memory selected in r/m field of ModR/M byte and SIB bhte (if available)
        /// </summary>
        XMMMemory,
        //X - memory address specifird by DS:ESI or DS:SI (32-bit) or by DS:ESI or RSI (64-bit)
        /// <summary>
        /// Memory address given by DS:ESI or DS:SI (32-bit) or by DS:ESI or RSI (64-bit)
        /// </summary>
        MemoryAddressESI,
        //Y - memory address specifird by ES:EDI or ES:DI (32-bit) or by ES:EDI or RDI (64-bit). Implicit es segment can't be overridden by segment prefixes
        /// <summary>
        /// Memory address given by ES:EDI or ES:DI (32-bit) or ES:EDI or RDI (64-bit). ES segment can't be overridden by segment prefixes
        /// </summary>
        MemoryAddressESEDI,
        //Z - no modr/m. three LSB in opcode specify reg
        /// <summary>
        /// Register selected in three LSB of primary opcode byte
        /// </summary>
        LSBRegister,
    }
}
