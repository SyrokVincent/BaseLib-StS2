using BaseLib.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BaseLib.Utils.Patching;

/// <summary>
/// Matches a consecutive list of instructions, requiring a matching opcode and operand.
/// If the operand to check for is null, it will be ignored and only the opcode will need to match.
/// </summary>
public class InstructionMatcher() : IMatcher
{
    private List<CodeInstruction> target = [];

    public bool Match(List<string> log, List<CodeInstruction> code, int startIndex, out int matchStart, out int matchEnd)
    {
        log.Add("Starting InstructionMatcher");
        matchStart = startIndex;
        matchEnd = matchStart;
        int matchIndex = 0;
        for (int i = startIndex; i < code.Count; ++i)
        {
            if (code[i].opcode == target[matchIndex].opcode)
            {
                if (target[matchIndex].operand == null || Equals(ComparisonOperand(code[i]), target[matchIndex].operand))
                {
                    log.Add($"Instruction match {code[i]}");
                    ++matchIndex;
                    if (matchIndex >= target.Count)
                    {
                        matchEnd = i + 1;
                        matchStart = matchEnd - target.Count;
                        return true;
                    }
                    continue;
                }
                else
                {
                    log.Add($"Opcode match but operand mismatch {code[i].opcode} | [{code[i].operand?.GetType() ?? null}]{code[i].operand} vs {target[matchIndex].operand}");
                }
            }

            if (matchIndex > 0)
            {
                log.Add($"Match ended, opcodes do not match ({code[i].opcode}, {target[matchIndex].opcode})");
                matchIndex = 0;
            }
        }
        return false;
    }

    private object ComparisonOperand(CodeInstruction codeInstruction)
    {
        switch (codeInstruction.opcode.Value)
        {
            case 0x11: //OpCodes.Ldloc_S
            case 0x12: //OpCodes.Ldloca_S
            case 0x13: //OpCodes.Stloc_S
                return ((LocalBuilder)codeInstruction.operand).LocalIndex;
            default:
                return codeInstruction.operand;
        }
    }

    public override string ToString()
    {
        return "InstructionMatcher:\n" + target.AsReadable("\n");
    }





    //Building
    //https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add?view=net-10.0
    public InstructionMatcher nop()
    {
        target.Add(new(opcode: OpCodes.Nop));
        return this;
    }
    public InstructionMatcher Break()
    {
        target.Add(new(opcode: OpCodes.Break));
        return this;
    }
    public InstructionMatcher ldarg_0()
    {
        target.Add(new(opcode: OpCodes.Ldarg_0));
        return this;
    }
    public InstructionMatcher ldarg_1()
    {
        target.Add(new(opcode: OpCodes.Ldarg_1));
        return this;
    }
    public InstructionMatcher ldarg_2()
    {
        target.Add(new(opcode: OpCodes.Ldarg_2));
        return this;
    }
    public InstructionMatcher ldarg_3()
    {
        target.Add(new(opcode: OpCodes.Ldarg_3));
        return this;
    }
    public InstructionMatcher ldloc_0()
    {
        target.Add(new(opcode: OpCodes.Ldloc_0));
        return this;
    }
    public InstructionMatcher ldloc_1()
    {
        target.Add(new(opcode: OpCodes.Ldloc_1));
        return this;
    }
    public InstructionMatcher ldloc_2()
    {
        target.Add(new(opcode: OpCodes.Ldloc_2));
        return this;
    }
    public InstructionMatcher ldloc_3()
    {
        target.Add(new(opcode: OpCodes.Ldloc_3));
        return this;
    }
    public InstructionMatcher stloc_0()
    {
        target.Add(new(opcode: OpCodes.Stloc_0));
        return this;
    }
    public InstructionMatcher stloc_1()
    {
        target.Add(new(opcode: OpCodes.Stloc_1));
        return this;
    }
    public InstructionMatcher stloc_2()
    {
        target.Add(new(opcode: OpCodes.Stloc_2));
        return this;
    }
    public InstructionMatcher stloc_3()
    {
        target.Add(new(opcode: OpCodes.Stloc_3));
        return this;
    }
    /*  
        Ldarg_S = 0x0e,
        Ldarga_S = 0x0f,
        Starg_S = 0x10,*/
    public InstructionMatcher ldloc_s(int index) //0x11
    {
        target.Add(new(opcode: OpCodes.Ldloc_S, index));
        return this;
    }
    public InstructionMatcher ldloca_s(int index) //0x12
    {
        target.Add(new(opcode: OpCodes.Ldloca_S, index));
        return this;
    }
    public InstructionMatcher stloc_s(int index)
    {
        target.Add(new(opcode: OpCodes.Stloc_S, index));
        return this;
    }
    public InstructionMatcher stloc_s()
    {
        target.Add(new(opcode: OpCodes.Stloc_S));
        return this;
    }
    public InstructionMatcher ldnull()
    {
        target.Add(new(opcode: OpCodes.Ldnull));
        return this;
    }
    public InstructionMatcher ldc_i4_m1() //-1
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_M1));
        return this;
    }
    public InstructionMatcher ldc_i4_0()
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_0));
        return this;
    }
    public InstructionMatcher ldc_i4_1()
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_1));
        return this;
    }
    public InstructionMatcher ldc_i4_2()
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_2));
        return this;
    }
    public InstructionMatcher ldc_i4_3()
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_3));
        return this;
    }
    public InstructionMatcher ldc_i4_4()
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_4));
        return this;
    }
    public InstructionMatcher ldc_i4_5()
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_5));
        return this;
    }
    public InstructionMatcher ldc_i4_6()
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_6));
        return this;
    }
    public InstructionMatcher ldc_i4_7()
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_7));
        return this;
    }
    public InstructionMatcher ldc_i4_8()
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_8));
        return this;
    }
    /*public InstructionMatcher ldc_i4_s()
    {
        target.Add(new(opcode: OpCodes.Ldc_I4_S));
        return this;
    }*/
    /*
    Ldc_I4 = 0x20,
    Ldc_I8 = 0x21,
    Ldc_R4 = 0x22,
    Ldc_R8 = 0x23,*/
    public InstructionMatcher dup()
    {
        target.Add(new(opcode: OpCodes.Dup));
        return this;
    }
    public InstructionMatcher pop()
    {
        target.Add(new(opcode: OpCodes.Pop));
        return this;
    }
    //Jmp = 0x27,
    public InstructionMatcher call(Type declaringType, string methodName, Type[] parameters = null, Type[] generics = null)
    {
        return call(AccessTools.Method(declaringType, methodName, parameters, generics));
    }
    public InstructionMatcher call(MethodInfo method)
    {
        target.Add(new(opcode: OpCodes.Call, operand: method));
        return this;
    }
    /*Calli = 0x29,*/
    public InstructionMatcher ret()
    {
        target.Add(new(opcode: OpCodes.Ret));
        return this;
    }
    public InstructionMatcher br_s(Label label)
    {
        target.Add(new(opcode: OpCodes.Br_S, label));
        return this;
    }
    public InstructionMatcher br_s()
    {
        target.Add(new(opcode: OpCodes.Br_S));
        return this;
    }
    public InstructionMatcher brfalse_s(Label label)
    {
        target.Add(new(opcode: OpCodes.Brfalse_S, label));
        return this;
    }
    public InstructionMatcher brfalse_s()
    {
        target.Add(new(opcode: OpCodes.Brfalse_S));
        return this;
    }
    public InstructionMatcher brtrue_s(Label label)
    {
        target.Add(new(opcode: OpCodes.Brtrue_S, label));
        return this;
    }
    public InstructionMatcher brtrue_s()
    {
        target.Add(new(opcode: OpCodes.Brtrue_S));
        return this;
    }
    public InstructionMatcher beq_s(Label label)
    {
        target.Add(new(opcode: OpCodes.Beq_S, label));
        return this;
    }
    public InstructionMatcher beq_s()
    {
        target.Add(new(opcode: OpCodes.Beq_S));
        return this;
    }
    /*Bge_S = 0x2f,
    Bgt_S = 0x30,
    Ble_S = 0x31,
    Blt_S = 0x32,
    Bne_Un_S = 0x33,
    Bge_Un_S = 0x34,
    Bgt_Un_S = 0x35,*/
    public InstructionMatcher ble_un_s(Label label)
    {
        target.Add(new(opcode: OpCodes.Ble_Un_S, label));
        return this;
    }
    public InstructionMatcher ble_un_s()
    {
        target.Add(new(opcode: OpCodes.Ble_Un_S));
        return this;
    }
    //Blt_Un_S = 0x37,
    public InstructionMatcher br(Label label) //0x38
    {
        target.Add(new(opcode: OpCodes.Br, label));
        return this;
    }
    public InstructionMatcher br()
    {
        target.Add(new(opcode: OpCodes.Br));
        return this;
    }
    /*Brfalse = 0x39,
    Brtrue = 0x3a,
    Beq = 0x3b,
    Bge = 0x3c,
    Bgt = 0x3d,
    Ble = 0x3e,
    Blt = 0x3f,
    Bne_Un = 0x40,
    Bge_Un = 0x41,
    Bgt_Un = 0x42,
    Ble_Un = 0x43,
    Blt_Un = 0x44,*/
    public InstructionMatcher switch_()
    {
        target.Add(new(opcode: OpCodes.Switch));
        return this;
    }
    /*Ldind_I1 = 0x46,
    Ldind_U1 = 0x47,
    Ldind_I2 = 0x48,
    Ldind_U2 = 0x49,
    Ldind_I4 = 0x4a,
    Ldind_U4 = 0x4b,
    Ldind_I8 = 0x4c,
    Ldind_I = 0x4d,
    Ldind_R4 = 0x4e,
    Ldind_R8 = 0x4f,
    Ldind_Ref = 0x50,
    Stind_Ref = 0x51,
    Stind_I1 = 0x52,
    Stind_I2 = 0x53,
    Stind_I4 = 0x54,
    Stind_I8 = 0x55,
    Stind_R4 = 0x56,
    Stind_R8 = 0x57,*/
    public InstructionMatcher add()
    {
        target.Add(new(opcode: OpCodes.Add));
        return this;
    }
    public InstructionMatcher sub()
    {
        target.Add(new(opcode: OpCodes.Sub));
        return this;
    }
    public InstructionMatcher mul()
    {
        target.Add(new(opcode: OpCodes.Mul));
        return this;
    }
    public InstructionMatcher div()
    {
        target.Add(new(opcode: OpCodes.Div));
        return this;
    }
    /*Div_Un = 0x5c,
    Rem = 0x5d,
    Rem_Un = 0x5e,
    And = 0x5f,
    Or = 0x60,
    Xor = 0x61,
    Shl = 0x62,
    Shr = 0x63,
    Shr_Un = 0x64,
    Neg = 0x65,
    Not = 0x66,
    Conv_I1 = 0x67,
    Conv_I2 = 0x68,
    Conv_I4 = 0x69,
    Conv_I8 = 0x6a,
    Conv_R4 = 0x6b,
    Conv_R8 = 0x6c,
    Conv_U4 = 0x6d,
    Conv_U8 = 0x6e,*/
    public InstructionMatcher callvirt(Type declaringType, string methodName, Type[] parameters = null, Type[] generics = null)
    {
        return callvirt(AccessTools.Method(declaringType, methodName, parameters, generics));
    }
    public InstructionMatcher callvirt(MethodInfo method)
    {
        target.Add(new(opcode: OpCodes.Callvirt, operand: method));
        return this;
    }
    /*Cpobj = 0x70,
    Ldobj = 0x71,
    Ldstr = 0x72,
    Newobj = 0x73,
    Castclass = 0x74,
    Isinst = 0x75,
    Conv_R_Un = 0x76,
    Unbox = 0x79,
    Throw = 0x7a,*/
    public InstructionMatcher ldfld(Type declaringType, string fieldName)
    {
        return ldfld(AccessTools.Field(declaringType, fieldName));
    }
    public InstructionMatcher ldfld(FieldInfo field)
    {
        target.Add(new(opcode: OpCodes.Ldfld, operand: field));
        return this;
    }

    /*Ldflda = 0x7c,*/
    public InstructionMatcher stfld(Type declaringType, string fieldName)
    {
        return stfld(AccessTools.Field(declaringType, fieldName));
    }
    public InstructionMatcher stfld(FieldInfo field)
    {
        target.Add(new(opcode: OpCodes.Stfld, operand: field));
        return this;
    }
/*Ldsfld = 0x7e,
Ldsflda = 0x7f,
Stsfld = 0x80,
Stobj = 0x81,
Conv_Ovf_I1_Un = 0x82,
Conv_Ovf_I2_Un = 0x83,
Conv_Ovf_I4_Un = 0x84,
Conv_Ovf_I8_Un = 0x85,
Conv_Ovf_U1_Un = 0x86,
Conv_Ovf_U2_Un = 0x87,
Conv_Ovf_U4_Un = 0x88,
Conv_Ovf_U8_Un = 0x89,
Conv_Ovf_I_Un = 0x8a,
Conv_Ovf_U_Un = 0x8b,
Box = 0x8c,*/
    public InstructionMatcher newarr(Type type)
    {
        target.Add(new(opcode: OpCodes.Newarr, operand: type));
        return this;
    }
    /*Ldlen = 0x8e,
    Ldelema = 0x8f,
    Ldelem_I1 = 0x90,
    Ldelem_U1 = 0x91,
    Ldelem_I2 = 0x92,
    Ldelem_U2 = 0x93,
    Ldelem_I4 = 0x94,
    Ldelem_U4 = 0x95,
    Ldelem_I8 = 0x96,
    Ldelem_I = 0x97,
    Ldelem_R4 = 0x98,
    Ldelem_R8 = 0x99,
    Ldelem_Ref = 0x9a,
    Stelem_I = 0x9b,
    Stelem_I1 = 0x9c,
    Stelem_I2 = 0x9d,
    Stelem_I4 = 0x9e,
    Stelem_I8 = 0x9f,
    Stelem_R4 = 0xa0,
    Stelem_R8 = 0xa1,
    */
    public InstructionMatcher stelem_ref()
    {
        target.Add(new(opcode: OpCodes.Stelem_Ref));
        return this;
    }
/*Ldelem = 0xa3,
Stelem = 0xa4,
Unbox_Any = 0xa5,
Conv_Ovf_I1 = 0xb3,
Conv_Ovf_U1 = 0xb4,
Conv_Ovf_I2 = 0xb5,
Conv_Ovf_U2 = 0xb6,
Conv_Ovf_I4 = 0xb7,
Conv_Ovf_U4 = 0xb8,
Conv_Ovf_I8 = 0xb9,
Conv_Ovf_U8 = 0xba,
Refanyval = 0xc2,
Ckfinite = 0xc3,
Mkrefany = 0xc6,
Ldtoken = 0xd0,
Conv_U2 = 0xd1,
Conv_U1 = 0xd2,
Conv_I = 0xd3,
Conv_Ovf_I = 0xd4,
Conv_Ovf_U = 0xd5,
Add_Ovf = 0xd6,
Add_Ovf_Un = 0xd7,
Mul_Ovf = 0xd8,
Mul_Ovf_Un = 0xd9,
Sub_Ovf = 0xda,
Sub_Ovf_Un = 0xdb,
Endfinally = 0xdc,
Leave = 0xdd,
Leave_S = 0xde,
Stind_I = 0xdf,
Conv_U = 0xe0,
Prefix7 = 0xf8,
Prefix6 = 0xf9,
Prefix5 = 0xfa,
Prefix4 = 0xfb,
Prefix3 = 0xfc,
Prefix2 = 0xfd,
Prefix1 = 0xfe,
Prefixref = 0xff,
Arglist = 0xfe00,
Ceq = 0xfe01,
Cgt = 0xfe02,
Cgt_Un = 0xfe03,
Clt = 0xfe04,
Clt_Un = 0xfe05,
Ldftn = 0xfe06,
Ldvirtftn = 0xfe07,
Ldarg = 0xfe09,
Ldarga = 0xfe0a,
Starg = 0xfe0b,
Ldloc = 0xfe0c,
Ldloca = 0xfe0d,
Stloc = 0xfe0e,
Localloc = 0xfe0f,
Endfilter = 0xfe11,
Unaligned_ = 0xfe12,
Volatile_ = 0xfe13,
Tail_ = 0xfe14,
Initobj = 0xfe15,
Constrained_ = 0xfe16,
Cpblk = 0xfe17,
Initblk = 0xfe18,
Rethrow = 0xfe1a,
Sizeof = 0xfe1c,
Refanytype = 0xfe1d,
Readonly_ = 0xfe1e,*/
}
