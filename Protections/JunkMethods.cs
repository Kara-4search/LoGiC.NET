﻿using System;
using System.Linq;
using LoGiC.NET.Utils;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace LoGiC.NET.Protections
{
    public class JunkMethods : Randomizer
    {
        /// <summary>
        /// The amount of added junk methods.
        /// </summary>
        private static int Amount = 1;
        
        /// <summary>
        /// This obfuscation will add random junk methods to make the code harder to decrypt to people if they think the junk methods are actually used.
        /// </summary>
        public static void Execute()
        {
            foreach (TypeDef type in Program.Module.Types)
            {
                if (type.IsGlobalModuleType)
                    continue;

                foreach (MethodDef _ in type.Methods.ToArray())
                {
                    MethodDef strings = CreateReturnMethodDef(Generated, Program.Module);
                    MethodDef ints = CreateReturnMethodDef(MemberRenamer.StringLength(), Program.Module);
                    
                    type.Methods.Add(strings);
                    type.Methods.Add(ints);

                    Amount += 2;
                }
            }
            Console.WriteLine($"  Added {Amount} junk methods.");
        }

        /// <summary>
		/// The return value for the randomly generated method. It can be an integer or a string.
		/// </summary>
		private static MethodDef CreateReturnMethodDef(object value, ModuleDefMD module)
        {
            CorLibTypeSig corlib = null;
            if (value is int)
                corlib = module.CorLibTypes.Int32;
            else if (value is string)
                corlib = module.CorLibTypes.String;

            MethodDef newMethod = new MethodDefUser(Generated, MethodSig.CreateStatic(corlib, corlib),
                    MethodImplAttributes.IL | MethodImplAttributes.Managed,
                    MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig)
            {
                Body = new CilBody()
            };

            if (value is int)
                newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, Convert.ToInt32(value)));
            else if (value is string)
                newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, value.ToString()));
            newMethod.Body.Instructions.Add(OpCodes.Ret.ToInstruction());

            return newMethod;
        }
    }
}
