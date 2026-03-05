using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BaseLib.Utils.Patching;

public class InstructionPatcher(IEnumerable<CodeInstruction> instructions)
{
    //Placeholder-ish, other existing tools may be easier to use. Will need time to see.
    private readonly List<CodeInstruction> code = [.. instructions];
    private int index = -1, lastMatchStart = -1;

    public readonly List<string> Log = [];

    public static implicit operator List<CodeInstruction>(InstructionPatcher locator) => locator.code;

    public InstructionPatcher ResetPosition()
    {
        index = -1;
        lastMatchStart = -1;
        return this;
    }

    /// <summary>
    /// Iterates over given matchers and attempts to match each in order.
    /// After matching is complete, position is on the code instruction following the last match.
    /// If a match is not found, an exception will be thrown.
    /// </summary>
    /// <param name="matchers"></param>
    /// <returns></returns>
    public InstructionPatcher Match(params IMatcher[] matchers)
    {
        return Match(DefaultMatchFailure, matchers);
    }
    /// <summary>
    /// Iterates over given matchers and attempts to match each in order.
    /// After matching is complete, position is on the code instruction following the last match.
    /// If a match is not found, onFailMatch is called. By default, this will throw an exception.
    /// </summary>
    /// <param name="onFailMatch"></param>
    /// <param name="matchers"></param>
    /// <returns></returns>
    public InstructionPatcher Match(Action<IMatcher[]> onFailMatch, params IMatcher[] matchers)
    {
        index = 0;
        foreach (IMatcher matcher in matchers)
        {
            if (!matcher.Match(Log, code, index, out lastMatchStart, out index))
            {
                onFailMatch(matchers);
                return this;
            }
        }

        Log.Add("Found end of match at " + index + "; last match starts at " + lastMatchStart);

        return this;
    }

    /// <summary>
    /// Adjust current position in code instructions.
    /// Should only be called after <seealso cref="Match(Action{IMatcher[]}, IMatcher[])"/> is called at least once.
    /// Avoid doing large steps into unmatched code, as this may result in issues if the code you are patching has already been modified.
    /// </summary>
    /// <param name="amt"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public InstructionPatcher Step(int amt = 1)
    {
        if (index < 0) throw new Exception("Attempted to Step without any match found");

        index += amt;

        Log.Add("Stepped to " + index);

        return this;
    }

    /// <summary>
    /// Gets all labels attached to the current instruction.
    /// </summary>
    /// <param name="labels"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public InstructionPatcher GetLabels(out List<Label> labels)
    {
        if (index < 0) throw new Exception("Attempted to GetLabels without any match found");

        labels = code[index].labels;

        if (labels.Count == 0)
        {
            if (code[index].operand is Label)
            {
                throw new Exception($"Code instruction {code[index].ToString()} has no labels. Did you mean to use GetOperandLabel instead?");
            }
            else
            {
                throw new Exception($"Code instruction {code[index].ToString()} has no labels");
            }
        }

        return this;
    }

    /// <summary>
    /// Gets a label used as the operand of the current instruction.
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public InstructionPatcher GetOperandLabel(out Label label)
    {
        if (index < 0) throw new Exception("Attempted to GetOperandLabel without any match found");
        if (code[index].operand is Label result)
        {
            label = result;
            return this;
        }
        throw new Exception($"Code instruction {code[index].ToString()} does not have a Label parameter");
    }

    public InstructionPatcher GetOperand(out object operand)
    {
        if (index < 0) throw new Exception("Attempted to GetOperand without any match found");
        operand = code[index].operand;
        return this;
    }

    /// <summary>
    /// Replaces a match of CodeInstructions. Note that if this removes a labeled instruction this can cause issues.
    /// Preserving labels must be done manually.
    /// </summary>
    /// <param name="replacement"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public InstructionPatcher ReplaceLastMatch(IEnumerable<CodeInstruction> replacement)
    {
        if (lastMatchStart < 0) throw new Exception("Attempted to ReplaceLastMatch without any match found");

        int i = 0;
        foreach (CodeInstruction instruction in replacement)
        {
            code[lastMatchStart + i] = instruction;
            ++i;
        }
        code.RemoveRange(lastMatchStart + i, index - (lastMatchStart + i));
        index = lastMatchStart + i;

        return this;
    }

    public InstructionPatcher Replace(CodeInstruction replacement, bool keepLabels = true)
    {
        if (index < 0) throw new Exception("Attempted to Replace without any match found");

        if (keepLabels)
        {
            replacement.MoveLabelsFrom(code[index]);
        }

        Log.Add($"{code[index]} => {replacement}");
        code[index] = replacement;
        return this;
    }

    public InstructionPatcher IncrementIntPush()
    {
        if (index < 0) throw new Exception("Attempted to Replace without any match found");

        switch (code[index].opcode.Value)
        {
            case 0x15: //m1, -1
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_0));
            case 0x16: //0
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_1));
            case 0x17: //1
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_2));
            case 0x18: //2
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_3));
            case 0x19: //3
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_4));
            case 0x1a: //4
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_5));
            case 0x1b: //5
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_6));
            case 0x1c: //6
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_7));
            case 0x1d: //7
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_8));
            case 0x1e: //8
                throw new Exception("Instruction " + code[index] + " cannot be incremented"); //maybe later support arbitrary int
            default:
                throw new Exception("Instruction " + code[index] + " is not an int push instruction that can be incremented");
        }
    }
    public InstructionPatcher IncrementIntPush(out CodeInstruction replacedPush)
    {
        if (index < 0) throw new Exception("Attempted to Replace without any match found");

        switch (code[index].opcode.Value)
        {
            case 0x15: //m1, -1
                replacedPush = new CodeInstruction(OpCodes.Ldc_I4_M1);
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_0));
            case 0x16: //0
                replacedPush = new CodeInstruction(OpCodes.Ldc_I4_0);
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_1));
            case 0x17: //1
                replacedPush = new CodeInstruction(OpCodes.Ldc_I4_1);
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_2));
            case 0x18: //2
                replacedPush = new CodeInstruction(OpCodes.Ldc_I4_2);
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_3));
            case 0x19: //3
                replacedPush = new CodeInstruction(OpCodes.Ldc_I4_3);
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_4));
            case 0x1a: //4
                replacedPush = new CodeInstruction(OpCodes.Ldc_I4_4);
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_5));
            case 0x1b: //5
                replacedPush = new CodeInstruction(OpCodes.Ldc_I4_5);
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_6));
            case 0x1c: //6
                replacedPush = new CodeInstruction(OpCodes.Ldc_I4_6);
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_7));
            case 0x1d: //7
                replacedPush = new CodeInstruction(OpCodes.Ldc_I4_7);
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_8));
            case 0x1e: //8
                replacedPush = new CodeInstruction(OpCodes.Ldc_I4_8);
                return Replace(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)9));
            //throw new Exception("Instruction " + code[index] + " cannot be incremented"); //maybe later support arbitrary int
            default:
                throw new Exception("Instruction " + code[index] + " is not an int push instruction that can be incremented");
        }
    }

    /// <summary>
    /// Inserts a single CodeInstruction before the current instruction.
    /// </summary>
    /// <param name="instruction"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public InstructionPatcher Insert(CodeInstruction instruction)
    {
        if (index < 0) throw new Exception("Attempted to Insert without any match found");

        code.Insert(index, instruction);
        ++index;

        return this;
    }

    /// <summary>
    /// Inserts a sequence of CodeInstructions before the current instruction (after the last match).
    /// </summary>
    /// <param name="instruction"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public InstructionPatcher Insert(IEnumerable<CodeInstruction> insert)
    {
        if (index < 0) throw new Exception("Attempted to Insert without any match found");

        code.InsertRange(index, insert);
        index += insert.Count();

        return this;
    }

    /// <summary>
    /// Inserts a copy of existing CodeInstructions determined by offset.
    /// Labels and blocks are not maintained, only opcodes and operands.
    /// </summary>
    /// <param name="startOffset"></param>
    /// <param name="copyLength"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public InstructionPatcher InsertCopy(int startOffset, int copyLength)
    {
        if (index < 0) throw new Exception("Attempted to InsertCopy without any match found");

        int startIndex = index + startOffset;
        if (startIndex < 0) throw new Exception($"startIndex of InsertCopy less than 0 ({startIndex})");

        List<CodeInstruction> copy = [];
        for (int i = 0; i < copyLength; ++i)
        {
            Log.Add("Inserting Copy: " + code[startIndex + i]);
            copy.Add(code[startIndex + i].Clone());
        }

        return Insert(copy);
    }

    public InstructionPatcher PrintLog(Logger logger)
    {
        logger.Info(Log.AsReadable("\n"));
        return this;
    }
    public InstructionPatcher PrintResult(Logger logger)
    {
        logger.Info("----- RESULT -----\n" + ((List<CodeInstruction>)this).NumberedLines());
        return this;
    }

    private void DefaultMatchFailure(IMatcher[] matchers)
    {
        throw new Exception("Failed to find match:\n" + matchers.AsReadable("\n---------\n") + "\nLOG:\n" + Log.AsReadable("\n"));
    }
}
