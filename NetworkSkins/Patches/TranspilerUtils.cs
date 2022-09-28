namespace NetworkSkins.Patches {
    using HarmonyLib;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    public class HarmonyPatch2 : HarmonyLib.HarmonyPatch {
        public HarmonyPatch2(Type delcaringType, Type delegateType, bool instance = false){
            info.declaringType = delcaringType;
            info.methodName = delegateType.Name;
            var args = delegateType.GetMethod("Invoke").GetParameters().Select(p => p.ParameterType);
            if (instance) args = args.Skip(1); // skip arg0 because its instance method.
            info.argumentTypes = args.ToArray();
        }
    }

    public delegate bool CodePredicate(CodeInstruction code);
    public delegate bool CodesPredicate(List<CodeInstruction> codes, int index);

    public static class TranspilerUtils {


        public const BindingFlags ALL = BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Instance
            | BindingFlags.Static
            | BindingFlags.GetField
            | BindingFlags.SetField
            | BindingFlags.GetProperty
            | BindingFlags.SetProperty;

        public const BindingFlags ALL_Declared = ALL | BindingFlags.DeclaredOnly;

        public static bool VERBOSE = false;

        public static void Log(string m) => UnityEngine.Debug.Log(m);

        /// <summary>
        /// like DeclaredMethod but throws suitable exception if method not found.
        /// </summary>
        internal static MethodInfo Method(this Type type, string name) =>
            type.GetMethod(name, ALL)
            ?? throw new Exception($"Method not found: {type.Name}.{name}");

        internal static FieldInfo Field(this Type type, string name) =>
            type.GetField(name, ALL)
            ?? throw new Exception($"Field not found: {type.Name}.{name}");

        public static CodeInstruction GetLDArg(MethodBase method, string argName, bool throwOnError = true) {
            if (!throwOnError && !HasParameter(method, argName))
                return null;
            byte idx = (byte)GetArgLoc(method, argName);
            if (idx == 0) {
                return new CodeInstruction(OpCodes.Ldarg_0);
            } else if (idx == 1) {
                return new CodeInstruction(OpCodes.Ldarg_1);
            } else if (idx == 2) {
                return new CodeInstruction(OpCodes.Ldarg_2);
            } else if (idx == 3) {
                return new CodeInstruction(OpCodes.Ldarg_3);
            } else {
                return new CodeInstruction(OpCodes.Ldarg_S, idx);
            }
        }

        /// <returns>
        /// returns the argument location to be used in LdArg instruction.
        /// </returns>
        public static byte GetArgLoc(this MethodBase method, string argName) {
            byte idx = (byte)GetParameterLoc(method, argName);
            if (!method.IsStatic)
                idx++; // first argument is object instance.
            return idx;
        }

        public static bool IsLdarg(this CodeInstruction code, MethodBase method, string argName) {
            byte loc = method.GetArgLoc(argName);
            return code.IsLdarg(loc);
        }

        /// <summary>
        /// Post condition: for instance method add one to get argument location
        /// </summary>
        public static byte GetParameterLoc(MethodBase method, string name) {
            var parameters = method.GetParameters();
            for (byte i = 0; i < parameters.Length; ++i) {
                if (parameters[i].Name == name) {
                    return i;
                }
            }
            throw new Exception($"did not found parameter with name:<{name}>");
        }

        public static bool HasParameter(MethodBase method, string name) {
            foreach (var param in method.GetParameters()) {
                if (param.Name == name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// shortcut for a.opcode == b.opcode && a.operand == b.operand
        /// </summary>
        public static bool IsSameInstruction(this CodeInstruction a, CodeInstruction b) {
            if (a.opcode == b.opcode) {
                if (a.operand == b.operand) {
                    return true;
                }

                // This special code is needed for some reason because the == operator doesn't work on System.Byte
                return (a.operand is byte aByte && b.operand is byte bByte && aByte == bByte);
            } else {
                return false;
            }
        }


        [Obsolete("use harmony extension instead")]
        public static bool IsLdLoc(CodeInstruction instruction) {
            return (instruction.opcode == OpCodes.Ldloc_0 || instruction.opcode == OpCodes.Ldloc_1 ||
                    instruction.opcode == OpCodes.Ldloc_2 || instruction.opcode == OpCodes.Ldloc_3
                    || instruction.opcode == OpCodes.Ldloc_S || instruction.opcode == OpCodes.Ldloc
                );
        }

        [Obsolete("use harmony extension instead")]
        public static bool IsStLoc(CodeInstruction instruction) {
            return (instruction.opcode == OpCodes.Stloc_0 || instruction.opcode == OpCodes.Stloc_1 ||
                    instruction.opcode == OpCodes.Stloc_2 || instruction.opcode == OpCodes.Stloc_3
                    || instruction.opcode == OpCodes.Stloc_S || instruction.opcode == OpCodes.Stloc
                );
        }

        /// <summary>
        /// Get the instruction to load the variable which is stored here.
        /// </summary>
        public static CodeInstruction BuildLdLocFromStLoc(this CodeInstruction instruction) {
            if (instruction.opcode == OpCodes.Stloc_0) {
                return new CodeInstruction(OpCodes.Ldloc_0);
            } else if (instruction.opcode == OpCodes.Stloc_1) {
                return new CodeInstruction(OpCodes.Ldloc_1);
            } else if (instruction.opcode == OpCodes.Stloc_2) {
                return new CodeInstruction(OpCodes.Ldloc_2);
            } else if (instruction.opcode == OpCodes.Stloc_3) {
                return new CodeInstruction(OpCodes.Ldloc_3);
            } else if (instruction.opcode == OpCodes.Stloc_S) {
                return new CodeInstruction(OpCodes.Ldloc_S, instruction.operand);
            } else if (instruction.opcode == OpCodes.Stloc) {
                return new CodeInstruction(OpCodes.Ldloc, instruction.operand);
            } else {
                throw new Exception("instruction is not stloc! : " + instruction);
            }
        }

        public static CodeInstruction BuildStLocFromLdLoc(this CodeInstruction instruction) {
            if (instruction.opcode == OpCodes.Ldloc_0) {
                return new CodeInstruction(OpCodes.Stloc_0);
            } else if (instruction.opcode == OpCodes.Ldloc_1) {
                return new CodeInstruction(OpCodes.Stloc_1);
            } else if (instruction.opcode == OpCodes.Ldloc_2) {
                return new CodeInstruction(OpCodes.Stloc_2);
            } else if (instruction.opcode == OpCodes.Ldloc_3) {
                return new CodeInstruction(OpCodes.Stloc_3);
            } else if (instruction.opcode == OpCodes.Ldloc_S) {
                return new CodeInstruction(OpCodes.Stloc_S, instruction.operand);
            } else if (instruction.opcode == OpCodes.Ldloc) {
                return new CodeInstruction(OpCodes.Stloc, instruction.operand);
            } else {
                throw new Exception("instruction is not ldloc! : " + instruction);
            }
        }

        internal static string IL2STR(this IEnumerable<CodeInstruction> instructions) {
            string ret = "";
            foreach (var code in instructions) {
                ret += code + "\n";
            }
            return ret;
        }

        public class InstructionNotFoundException : Exception {
            public InstructionNotFoundException() : base() { }
            public InstructionNotFoundException(string m) : base(m) { }
        }

        /// <param name="count">Number of occurrences. Negative count searches backward</param>
        public static int Search(
            this List<CodeInstruction> codes,
            CodePredicate predicate,
            int startIndex = 0, int count = 1, bool throwOnError = true) {
            return codes.Search(
                (c, i) => predicate(codes[i]),
                startIndex: startIndex,
                count: count,
                throwOnError: throwOnError);

        }

        /// <param name="count">negative count searches backward</param>
        public static int Search(
            this List<CodeInstruction> codes,
            CodesPredicate predicate,
            int startIndex = 0, int count = 1, bool throwOnError = true) {
            if (codes == null) throw new ArgumentNullException("codes");
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (count == 0) throw new ArgumentOutOfRangeException("count can't be zero");
            int dir = count > 0 ? 1 : -1;
            int counter = Math.Abs(count);
            int n = 0;
            int index = startIndex;

            for (; 0 <= index && index < codes.Count; index += dir) {
                if (predicate(codes, index)) {
                    if (++n == counter)
                        return index;
                }
            }

            if (throwOnError == true) {
                throw new InstructionNotFoundException($"count: found={n} requested={count}");
            } else {
                if (VERBOSE)
                    Log("Did not found instruction[s].\n" + Environment.StackTrace);
                return -1;
            }
        }



        public static void MoveLabels(CodeInstruction source, CodeInstruction target) {
            // move labels
            var labels = source.labels;
            target.labels.AddRange((IEnumerable<Label>)labels);
            labels.Clear();
        }

        public static int PeekBefore = 10;
        public static int PeekAfter = 15;

        /// <summary>
        /// replaces one instruction at the given index with multiple instructions
        /// </summary>
        public static void ReplaceInstructions(List<CodeInstruction> codes, CodeInstruction[] insertion, int index) {
            foreach (var code in insertion)
                if (code == null)
                    throw new Exception("Bad Instructions:\n" + insertion.IL2STR());
            if (VERBOSE)
                Log($"replacing <{codes[index]}>\nInsert between: <{codes[index - 1]}>  and  <{codes[index + 1]}>");

            MoveLabels(codes[index], insertion[0]);
            codes.RemoveAt(index);
            codes.InsertRange(index, insertion);

            if (VERBOSE) {
                Log("Replacing with\n" + insertion.IL2STR());
                string message = "PEEK\n";
                for (int i = index - PeekBefore; i <= index + PeekAfter && i < codes.Count; ++i) {
                    if (i == index)
                        message += " *** REPLACEMENT START ***\n";
                    message += codes[i] + "\n";
                    if (i == index + insertion.Length - 1)
                        message += " *** REPLACEMENT END ***\n";
                }
                Log(message);
            }
        }

        public static void InsertInstructions(List<CodeInstruction> codes, CodeInstruction[] insertion, int index, bool moveLabels = true) {
            foreach (var code in insertion)
                if (code == null)
                    throw new Exception("Bad Instructions:\n" + insertion.IL2STR());
            if (VERBOSE)
                Log($"Insert point:\n between: <{codes[index - 1]}>  and  <{codes[index]}>");

            MoveLabels(codes[index], insertion[0]);
            codes.InsertRange(index, insertion);

            if (VERBOSE) {
                Log("Insertion is:\n" + insertion.IL2STR());
                string message = "PEEK\n";
                for(int i=index-PeekBefore; i <= index + PeekAfter && i<codes.Count; ++i) {
                    if (i == index)
                        message += " *** INJECTION START ***\n";
                    message += codes[i] + "\n";
                    if (i == index + insertion.Length - 1)
                        message += " *** INJECTION END ***\n";
                }
                Log(message);
            }
        }
    }

    internal static class TranspilerExtensions {
        public static void InsertInstructions(
            this List<CodeInstruction> codes, int index, IEnumerable<CodeInstruction> insertion, bool moveLabels = true) {
            TranspilerUtils.InsertInstructions(codes, insertion.ToArray(), index, moveLabels);
        }

        public static void InsertInstruction(
            this List<CodeInstruction> codes, int index, CodeInstruction insertion, bool moveLabels = true) {
            TranspilerUtils.InsertInstructions(codes, new[] { insertion }, index, moveLabels);
        }


        /// <summary>
        /// replaces one instruction at the given index with multiple instructions
        /// </summary>
        public static void ReplaceInstructions(
            this List<CodeInstruction> codes, int index, IEnumerable<CodeInstruction> insertion) {
            TranspilerUtils.ReplaceInstructions(codes, insertion.ToArray(), index);
        }

        public static void ReplaceInstruction(
            this List<CodeInstruction> codes, int index, CodeInstruction insertion) {
            TranspilerUtils.ReplaceInstructions(codes, new[] { insertion }, index);
        }

        public static bool IsLdLoc(this CodeInstruction code, out int loc) {
            if (code.opcode == OpCodes.Ldloc_0) {
                loc = 0;
            } else if (code.opcode == OpCodes.Ldloc_1) {
                loc = 1;
            } else if (code.opcode == OpCodes.Ldloc_2) {
                loc = 2;
            } else if (code.opcode == OpCodes.Ldloc_3) {
                loc = 3;
            } else if (code.opcode == OpCodes.Ldloc_S || code.opcode == OpCodes.Ldloc) {
                if (code.operand is LocalBuilder lb)
                    loc = lb.LocalIndex;
                else
                    loc = (int)code.operand;
            } else {
                loc = -1;
                return false;
            }
            return true;
        }

        public static bool IsLdLoc(this CodeInstruction code, int loc) {
            if (!code.IsLdLoc(out int loc0))
                return false;
            return loc == loc0;
        }

        public static bool IsStLoc(this CodeInstruction code, out int loc) {
            if (code.opcode == OpCodes.Stloc_0) {
                loc = 0;
            } else if (code.opcode == OpCodes.Stloc_1) {
                loc = 1;
            } else if (code.opcode == OpCodes.Stloc_2) {
                loc = 2;
            } else if (code.opcode == OpCodes.Stloc_3) {
                loc = 3;
            } else if (code.opcode == OpCodes.Stloc_S || code.opcode == OpCodes.Stloc) {
                if (code.operand is LocalBuilder lb)
                    loc = lb.LocalIndex;
                else
                    loc = (int)code.operand;
            } else {
                loc = -1;
                return false;
            }
            return true;
        }

        public static bool IsStLoc(this CodeInstruction code, int loc) {
            if (!code.IsStLoc(out int loc0))
                return false;
            return loc == loc0;
        }

        /// <summary>
        /// loads a field of the given type or its address (ldfld/ldflda and their variants).
        /// </summary>
        /// <param name="type">type of variable being loaded</param>
        /// <param name="method">method containing the local variable</param>
        public static bool IsLdLoc(this CodeInstruction code, Type type, MethodBase method) {
            if (!code.IsLdloc())
                return false;
            if (code.opcode == OpCodes.Ldloc_0) {
                return method.GetMethodBody().LocalVariables[0].LocalType == type;
            } else if (code.opcode == OpCodes.Ldloc_1) {
                return method.GetMethodBody().LocalVariables[1].LocalType == type;
            } else if (code.opcode == OpCodes.Ldloc_2) {
                return method.GetMethodBody().LocalVariables[2].LocalType == type;
            } else if (code.opcode == OpCodes.Ldloc_3) {
                return method.GetMethodBody().LocalVariables[3].LocalType == type;
            } else {
                return code.operand is LocalBuilder lb && lb.LocalType == type;
            }
            
        }

        /// <summary>
        /// ldloca[.s] for a certain variable type
        /// </summary>
        /// <param name="type">type of variable being loaded</param>
        /// <param name="method">method containing the local variable</param>
        public static bool IsLdLocA(this CodeInstruction code, Type type, out int loc) {
            bool isldloca =
                code.opcode == OpCodes.Ldloca ||
                code.opcode == OpCodes.Ldloca_S;
            if (isldloca && code.operand is LocalBuilder lb && lb.LocalType == type) {
                loc = lb.LocalIndex;
                return true;
            }
            loc = -1;
            return false;

        }

        public static bool IsStLoc(this CodeInstruction code, Type type, MethodBase method) {
            if (!code.IsStloc())
                return false;
            if (code.opcode == OpCodes.Stloc_0) {
                return method.GetMethodBody().LocalVariables[0].LocalType == type;
            } else if (code.opcode == OpCodes.Stloc_1) {
                return method.GetMethodBody().LocalVariables[1].LocalType == type;
            } else if (code.opcode == OpCodes.Stloc_2) {
                return method.GetMethodBody().LocalVariables[2].LocalType == type;
            } else if (code.opcode == OpCodes.Stloc_3) {
                return method.GetMethodBody().LocalVariables[3].LocalType == type;
            } else {
                return code.operand is LocalBuilder lb && lb.LocalType == type;
            }
        }

        public static int GetLoc(this CodeInstruction code) {
            int loc = -1;
            if (code.opcode == OpCodes.Ldloc_0 || code.opcode == OpCodes.Stloc_0) {
                loc = 0;
            } else if (code.opcode == OpCodes.Ldloc_1 || code.opcode == OpCodes.Stloc_1) {
                loc = 1;
            } else if (code.opcode == OpCodes.Ldloc_2 || code.opcode == OpCodes.Stloc_2) {
                loc = 2;
            } else if (code.opcode == OpCodes.Ldloc_3 || code.opcode == OpCodes.Stloc_3) {
                loc = 3;
            } else if (
                code.opcode == OpCodes.Ldloc_S || code.opcode == OpCodes.Ldloc ||
                code.opcode == OpCodes.Ldloca_S || code.opcode == OpCodes.Ldloca ||
                code.opcode == OpCodes.Stloc_S || code.opcode == OpCodes.Stloc) {
                if (code.operand is LocalBuilder lb)
                    loc = lb.LocalIndex;
                else
                    loc = (int)code.operand;
            } else {
                throw new Exception($"{code} is not stloc, ldloc or ldlocA");
            }
            return loc;
        }
    }
}
