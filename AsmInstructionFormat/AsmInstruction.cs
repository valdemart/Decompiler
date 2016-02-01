using System;
using System.Collections.Generic;

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
        /// Is this instruction opcode invalid in specified context
        /// </summary>
        public bool IsInstructionInvalid { get; set; }
        /// <summary>
        /// Does this instruction have undefined behavior in specified context
        /// </summary>
        public bool UndefinedBehavior { get; set; }
        /// <summary>
        /// Is this instruction treated like NOP in specified context
        /// </summary>
        public bool NOPLike { get; set; }
        /// <summary>
        /// If this instruction is only an alias to other instruction, this property specifies reference to instruction it references to
        /// </summary>
        public string AliasTo { get; set; }
        /// <summary>
        /// Is this instruction only partially an alias (similar but with slightly different meaning)
        /// </summary>
        public bool PartialAlias { get; set; }
        /// <summary>
        /// Instruction operands information
        /// </summary>
        public List<OperandInformation> Operands { get; set; }
        /// <summary>
        /// Instruction set extension this instruction was added in
        /// </summary>
        public string ExtensionGroup { get; set; }
        /// <summary>
        /// Common instruction classification
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Flags tested by this instruction
        /// </summary>
        public string TestedFlags { get; set; }
        /// <summary>
        /// Flags modified by this instruction
        /// </summary>
        public string ModifiedFlags { get; set; }
        /// <summary>
        /// Flags, which state after execution of this instruction is somehow defined 
        /// </summary>
        public string DefinedFlags { get; set; }
        /// <summary>
        /// Flags, which state after execution of this instruction is undefined
        /// </summary>
        public string UndefinedFlags { get; set; }
        /// <summary>
        /// Constant values set into flags during execution of this instruction
        /// </summary>
        public string SetFlagsValues { get; set; }
        /// <summary>
        /// Instruction description
        /// </summary>
        public string Description { get; set; }
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
        /// <summary>
        /// Operand format
        /// </summary>
        public OperandFormats OperandFormat { get; set; }
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

    public enum OperandFormats
    {
        //a - two word operands or two dword operands, in memory, depending on operand size prefix
        /// <summary>
        /// Two 16-bit values or two 32-bit values in memory, depending on operand size prefix
        /// </summary>
        TwoWordDWord,
        //b - byte operand
        /// <summary>
        /// 8-bit valu
        /// </summary>
        Byte,
        //bcd - packed-BCD operand
        /// <summary>
        /// 80-bit packed BCD value used by FPU
        /// </summary>
        FPUPackedBCD,
        //bs - byte, sign-extended to destination operand size
        /// <summary>
        /// 8-bit value, sign-extended to destination size
        /// </summary>
        ByteSignExtended,
        //bsq - like bs, extended to 64-bits
        /// <summary>
        /// 8-bit value, sign-extended to 64-bits
        /// </summary>
        ByteToQWordSignExtended,
        //bss - like bs, extended to size of stack pointer (SP, ESP or RSP)
        /// <summary>
        /// 8-bit value, sign-extended to size of stack frame
        /// </summary>
        ByteToStackFrameSignExtended,
        //c - byte or word, depending on operand size prefix
        /// <summary>
        /// 8-bit or 16-bit value, depending on operand size prefix
        /// </summary>
        ByteWordSizeDependent,
        //d - dword operand, regardless of operand size prefix
        /// <summary>
        /// 32-bit value
        /// </summary>
        DWord,
        //da - dword depending on address size prefix
        /// <summary>
        /// 32-bit value, depending on address size prefix
        /// </summary>
        DWordAddrDependent,
        //di - dword integer (used by FPU)
        /// <summary>
        /// 32-bit integer value used by FPU
        /// </summary>
        FPUDWordInteger,
        //do - dword depending on current operand size
        /// <summary>
        /// 32-bit value, depending on current operand size
        /// </summary>
        DWordCurrDependent,
        //dq - dqword, regardless of operand size prefix
        /// <summary>
        /// 128-bit value
        /// </summary>
        DQWord,
        //dqa - dword or qword depending on address size prefix
        /// <summary>
        /// 32-bit or 64-bit value, depending on address size prefix
        /// </summary>
        DWordQWordAddrDependent,
        //dqp - dword or qword (qword when REX.W=1)
        /// <summary>
        /// 32-bit or 64-bit value, depending on REX.W bit (64-bit when REX.W=1)
        /// </summary>
        DWordQWordREXWDependent,
        //dr - qword real (c#: double)
        /// <summary>
        /// 64-bit floating-point value (C#: double)
        /// </summary>
        Double,
        //ds - dword sign-extended to 64-bits
        /// <summary>
        /// 32-bit value, sign-extended to 64-bits
        /// </summary>
        DWordSignExtended,
        //e - FPU environment (14 or 28 bits)
        /// <summary>
        /// 14- or 28-bit FPU environment state data
        /// </summary>
        FPUEnvironment,
        //er - extended real (80-bits real)
        /// <summary>
        /// 80-bit floating-point value
        /// </summary>
        Extended,
        //p - 16:16 or 16:32 pointer operand (depending on operand size prefix)
        /// <summary>
        /// 16:16 or 16:32 pointer, depending on operand size prefix
        /// </summary>
        Pointer,
        //pi - MMX qword
        /// <summary>
        /// MMX 64-bit value
        /// </summary>
        MMXQWord,
        //pd - packed into 128-bits, double-precision real numbers
        /// <summary>
        /// Two 64-bit floating-point values, packed into 128-bit pack
        /// </summary>
        PackedDouble,
        //ps - packed into 128-bits, single-precision real numbers
        /// <summary>
        /// Four 32-bit floating-point values, packed into 128-bit pack
        /// </summary>
        PackedSingle,
        //psq - packed into 64-bits, single-precision real numbers
        /// <summary>
        /// Two 32-bit floating-point values, packed into 64-bit pack
        /// </summary>
        PackedTwoSingle,
        //pt - 16:64 pointer operand
        /// <summary>
        /// 16:64 pointer
        /// </summary>
        LongPointer,
        //ptp - 16:16 or 16:32 or 16:64 (with REX.W=1) pointer operand
        /// <summary>
        /// 16:16, 16:32 or 16:64 (with REX.W=1) pointer
        /// </summary>
        GeneralPointer,
        //q - qword operand, regardless of operand size prefix
        /// <summary>
        /// 64-bit value
        /// </summary>
        QWord,
        //qa - qword depending on address size prefix
        /// <summary>
        /// 64-bit value, depending on address size prefix
        /// </summary>
        QWordAddrDependent,
        //qi - qword integer (used by FPU)
        /// <summary>
        /// 64-bit integer value used by FPU
        /// </summary>
        FPUQWordInteger,
        //qp - qword with REX.W=1
        /// <summary>
        /// 64-bit value with REX.W=1
        /// </summary>
        QWordREXW,
        //qs - qword depending on current stack size
        /// <summary>
        /// 64-bit value, depending on current operand size
        /// </summary>
        QWordCurrDependent,
        //s - 6- or 10-byte (in 64-bit mode) pseudodescriptor
        /// <summary>
        /// 48- or 80-bit (in 64-bit mode) pseudodescriptor
        /// </summary>
        Pseudodescriptor,
        //sd - scalar element of 128-bit packed double-precision data
        /// <summary>
        /// Scalar element of two 64-bit floating-point values, packed into 128-bit pack
        /// </summary>
        ScalarDouble,
        //si - dword integer register
        /// <summary>
        /// 32-bit integer register (e.g. EAX)
        /// </summary>
        DWordRegister,
        //sr - dword real (c#: float)
        /// <summary>
        /// 32-bit floating-point value (C#: float)
        /// </summary>
        Single,
        //ss - scalar element of 128-bit packed single-precision data
        /// <summary>
        /// Scalar element of four 32-bit floating-point values, packed into 128-bit pack
        /// </summary>
        ScalarSingle,
        //st - 94- or 108-bit FPU state
        /// <summary>
        /// 94- or 108-bit FPU state
        /// </summary>
        FPUState,
        //stx - 512-bit FPU and SIMD state
        /// <summary>
        /// 512-bit FPU and SIMD state
        /// </summary>
        FPUSIMDState,
        //t - 10-byte far pointer operand
        /// <summary>
        /// 80-bit far pointer
        /// </summary>
        FarPointer,
        //v - word or dword operand, depending on operand size prefix
        /// <summary>
        /// 16- or 32-bit value, depending on operand size prefix
        /// </summary>
        WordDWordSizeDependent,
        //va - word or dword depending on address size prefix
        /// <summary>
        /// 16- or 32-bit value, depending on address size prefix
        /// </summary>
        WordDWordAddrDependent,
        //vds - word or dword depending on operand size prefix or dword sign-extended to 64-bits with 64-bit operand size prefix
        /// <summary>
        /// 16-, 32- or 64-bit (32-bit sign-extended to 64-bit) value, depending on operamd size prefix
        /// </summary>
        WordDWord64SizeDependent,
        //vq - qword (default) or word depending on operand size prefix
        /// <summary>
        /// 64- or 16-bit value, depending on operand size prefix (64-bit is default)
        /// </summary>
        QWordWordSizeDependent,
        //vqp - word or dword depending on operand size prefix or qword with REX.W=1
        /// <summary>
        /// 16- or 32-bit value, depending on operand size prefix, or 64-bit value with REX.W=1
        /// </summary>
        WordDWordQWord,
        //vs - word or dword sign-extended to stack pointer
        /// <summary>
        /// 16- or 32-bit value, sign extended to stack frame size
        /// </summary>
        WordDWordToStackFrameSignExtended,
        //w - word regardless of operand size prefix
        /// <summary>
        /// 16-bit value
        /// </summary>
        Word,
        //wa - word depending on address size prefix
        /// <summary>
        /// 16-bit value, depending on address size prefix
        /// </summary>
        WordAddrDependent,
        //wi - word integer (used by FPU)
        /// <summary>
        /// 16-bit integer value used by FPU
        /// </summary>
        FPUWordInteger,
        //wo - word depending on current operand size
        /// <summary>
        /// 16-bit value, depending on current operand size
        /// </summary>
        WordCurrDependent,
        //ws - word depending on current stack size
        /// <summary>
        /// 16-bit value, depending on current stack frame size
        /// </summary>
        WordStackFrameDependent,
    }

    [Flags]
    public enum FlagsStates
    {
        OF_SET = 0x00000001, OF_CLEAR = 0x00000002, OF = OF_SET | OF_CLEAR,
        DF_SET = 0x00000004, DF_CLEAR = 0x00000008, DF = DF_SET | DF_CLEAR,
        IF_SET = 0x00000010, IF_CLEAR = 0x00000020, IF = IF_SET | IF_CLEAR,
        SF_SET = 0x00000040, SF_CLEAR = 0x00000080, SF = SF_SET | SF_CLEAR,
        ZF_SET = 0x00000100, ZF_CLEAR = 0x00000200, ZF = ZF_SET | ZF_CLEAR,
        AF_SET = 0x00000400, AF_CLEAR = 0x00000800, AF = AF_SET | AF_CLEAR,
        PF_SET = 0x00001000, PF_CLEAR = 0x00002000, PF = PF_SET | PF_CLEAR,
        CF_SET = 0x00004000, CF_CLEAR = 0x00008000, CF = CF_SET | CF_CLEAR,
        FPU1_SET = 0x00010000, FPU1_CLEAR = 0x00020000, FPU1 = FPU1_SET | FPU1_CLEAR,
        FPU2_SET = 0x00040000, FPU2_CLEAR = 0x00080000, FPU2 = FPU2_SET | FPU2_CLEAR,
        FPU3_SET = 0x00100000, FPU3_CLEAR = 0x00200000, FPU3 = FPU3_SET | FPU3_CLEAR,
        FPU4_SET = 0x00400000, FPU4_CLEAR = 0x00800000, FPU4 = FPU4_SET | FPU4_CLEAR,
    }

    public class ModRM
    {
        public int ModField { get; private set; }
        public int RegField { get; private set; }
        public int RMField { get; private set; }

        public bool SIBPresent { get; private set; }
        public Displacements Displacement { get; private set; }

        public Registers RMRegister { get; private set; }

        public Registers RegRegister { get; private set; }

        public Addresses32_64 Address32_64 { get; private set; }
        public Addresses32 Address32 { get; private set; }
        public Addresses16 Address16 { get; private set; }

        public ModRM(byte modRM, bool rexR, bool rexW, bool rexB, bool rexX)
        {
            ModField = (modRM >> 6) & 0x03;
            RegField = (modRM >> 3) & 0x07;
            RMField = modRM & 0x07;


        }
    }

    public enum Displacements
    {
        None,
        Disp8,
        Disp16,
        Disp32,
    }

    public enum Registers
    {
        Invalid = -1,
        AL = 0, AH = 4, AX = 0, EAX = 0, RAX = 0,
        CL = 1, CH = 5, CX = 1, ECX = 1, RCX = 1,
        DL = 2, DH = 6, DX = 2, EDX = 2, RDX = 2,
        BL = 3, BH = 7, BX = 3, EBX = 3, RBX = 3,
        SPL = 4, SP = 4, ESP = 4, RSP = 4,
        BPL = 5, BP = 5, EBP = 5, RBP = 5,
        SIL = 6, SI = 6, ESI = 6, RSI = 6,
        DIL = 7, DI = 7, EDI = 7, RDI = 7,
        ES = 0, CS = 1, SS = 2, DS = 3, FS = 4, GS = 5,
        R8B = 0, R8W = 0, R8D = 0, R8 = 0,
        R9B = 1, R9W = 1, R9D = 1, R9 = 1,
        R10B = 2, R10W = 2, R10D = 2, R10 = 2,
        R11B = 3, R11W = 3, R11D = 3, R11 = 3,
        R12B = 4, R12W = 4, R12D = 4, R12 = 4,
        R13B = 5, R13W = 5, R13D = 5, R13 = 5,
        R14B = 6, R14W = 6, R14D = 6, R14 = 6,
        R15B = 7, R15W = 7, R15D = 7, R15 = 7,
        ST0 = 0, ST1 = 1, ST2 = 2, ST3 = 3, ST4 = 4, ST5 = 5, ST6 = 6, ST7 = 7,
        MM0 = 0, MM1 = 1, MM2 = 2, MM3 = 3, MM4 = 4, MM5 = 5, MM6 = 6, MM7 = 7,
        XMM0 = 0, XMM1 = 1, XMM2 = 2, XMM3 = 3, XMM4 = 4, XMM5 = 5, XMM6 = 6, XMM7 = 7,
        XMM8 = 0, XMM9 = 1, XMM10 = 2, XMM11 = 3, XMM12 = 4, XMM13 = 5, XMM14 = 6, XMM15 = 7,
        CR0 = 0, CR2 = 2, CR3 = 3, CR4 = 4, CR8 = 0,
        DR0 = 0, DR1 = 1, DR2 = 2, DR3 = 3, DR4 = 4, DR5 = 5, DR6 = 6, DR7 = 7,
    }

    public enum Addresses32_64
    {
        Invalid = -1,
        RAX = 0, EAX = 0,
        RCX = 1, ECX = 1,
        RDX = 2, EDX = 2,
        RBX = 3, EBX = 3,
        SIB = 4,
        RIP = 0xC5, EIP = 0xC5,
        RBP = 5, EBP = 5,
        RSI = 6, ESI = 6,
        RDI = 7, EDI = 7,
    }

    public enum Addresses32
    {
        Invalid = -1,
        EAX = 0,
        ECX = 1,
        EDX = 2,
        EBX = 3,
        SIB = 4,
        /// <summary>
        /// No register is used in address. Displacement only.
        /// </summary>
        None = 0xC5,
        EBP = 5,
        ESI = 6,
        EDI = 7,
    }

    public enum Addresses16
    {
        Invalid = -1,
        BX_PLUS_SI = 0,
        BX_PLUS_DI = 1,
        BP_PLUS_SI = 2,
        BP_PLUS_DI = 3,
        SI = 4,
        DI = 5,
        /// <summary>
        /// No register is used in address. Displacement only.
        /// </summary>
        None = 0xC6,
        BP = 6,
        BX = 7,
    }
}
