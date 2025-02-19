﻿using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LoGiC.NET.Utils;

namespace LoGiC.NET.Protections
{
    public class IntEncoding : Randomizer
    {
        /// <summary>
        /// The amount of encoded ints.
        /// </summary>
        private static int Amount { get; set; }

        /// <summary>
        /// Execution of the 'IntEncoding' method. It'll encodes the integers within different methods.
        /// Absolute : This method will add Math.Abs(int) before each integer.
        /// StringLen : This method will replace each integer by their string equivalent.
        /// </summary>
        public static void Execute()
        {
            foreach (TypeDef type in Program.Module.Types)
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody)
                        continue;

                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                        if (method.Body.Instructions[i].IsLdcI4())
                        {
                            int operand = method.Body.Instructions[i].GetLdcI4Value();
                            if (operand <= 0) // Prevents errors.
                                continue;

                            // The Absolute method.
                            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(
                                Program.Module.Import(typeof(Math).GetMethod("Abs", new Type[] { typeof(int) }))));

                            // The String Length method.
                            // To fix
                            /*method.Body.Instructions[i].OpCode = OpCodes.Ldstr;
                            method.Body.Instructions[i].Operand = GenerateRandomString(operand);
                            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(
                                Program.Module.Import(typeof(string).GetMethod("get_Length"))));*/

                            // The Negative method.
                            for (var j = 0; j < 8; j++)
                                method.Body.Instructions.Insert(i + j + 1, Instruction.Create(OpCodes.Neg));

                            // The Max method.
                            /*if (operand > 1)
                            {
                                method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(1));
                                method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(Program.Module.Import(typeof(Math).GetMethod("Max", new Type[] { typeof(int), typeof(int) }))));
                            }*/

                            // The Min method.
                            if (operand < int.MaxValue)
                            {
                                method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(int.MaxValue));
                                method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(Program.Module.Import(typeof(Math).GetMethod("Min", new Type[] { typeof(int), typeof(int) }))));
                            }

                            ++Amount;
                        }
                }

            Console.WriteLine($"  Encoded {Amount} ints.");
        }
    }
}
