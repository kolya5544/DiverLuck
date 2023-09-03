using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiverLuckCore.Helpers
{
    public class ParseHelper
    {
        public static string ParseFunction(string program, int indexBegin)
        {
            string funBegin = program.Substring(indexBegin + 1);
            int funDepth = 0;
            int endIndex = -1;

            for (int i = 0; i < funBegin.Length; i++)
            {
                char p = funBegin[i];
                if (p == '&')
                {
                    funDepth += 1;
                } else if (p == '\\')
                {
                    funDepth -= 1;
                }

                if (funDepth < 0)
                {
                    endIndex = i;
                    break;
                }
            }

            if (endIndex == -1) throw new Exception("Function end statement not found.");
            return funBegin.Substring(0, endIndex);
        }

        public static int RunInfiniteLoop(DiverLuck diver, string program, int indexBegin)
        {
            string loopBegin = program.Substring(indexBegin + 1);
            int loopDepth = 0;
            int endIndex = -1;

            for (int i = 0; i < loopBegin.Length; i++)
            {
                char p = loopBegin[i];
                if (p == '[')
                {
                    loopDepth += 1;
                }
                else if (p == ']')
                {
                    loopDepth -= 1;
                }

                if (loopDepth < 0)
                {
                    endIndex = i;
                    break;
                }
            }

            if (endIndex == -1) throw new Exception("Loop end statement not found.");

            string loopCode = loopBegin.Substring(0, endIndex);

            Console.ResetColor();

            while (diver.ExecuteProgram(loopCode));
            return endIndex;
        }

        public static Tuple<string, bool, int> ParseIFblock(DiverLuck diver, string program, int indexBegin)
        {
            string ifCondBegin = program.Substring(indexBegin + 1);
            int endCondIndex = -1;

            for (int i = 0; i < ifCondBegin.Length; i++)
            {
                char p = ifCondBegin[i];

                if (p == 'f')
                {
                    endCondIndex = i;
                    break;
                }
            }

            if (endCondIndex == -1) throw new Exception("Condition block end statement not found.");

            string conditionBlock = ifCondBegin.Substring(0, endCondIndex);
            // parse condition block
            char condType = conditionBlock[0];
            int number = int.Parse(conditionBlock.Substring(1));
            bool result = false;

            switch (condType)
            {
                case 'e':
                    result = number == diver.GetCell().value; break;
                case 'n':
                    result = number != diver.GetCell().value; break;
                case 'm':
                    result = number > diver.GetCell().value; break;
                case 'l':
                    result = number < diver.GetCell().value; break;
            }

            // parse code block
            string codeBlockBegin = ifCondBegin.Substring(endCondIndex + 1); // skip the { under assumption it exists
            if (codeBlockBegin.First() != '{') throw new Exception("If code block statement not found.");
            codeBlockBegin = codeBlockBegin.Substring(1);

            int endIndex = -1;
            int codeDepth = 0;

            for (int i = 0; i < codeBlockBegin.Length; i++)
            {
                char p = codeBlockBegin[i];

                if (p == '{')
                {
                    codeDepth += 1;
                }
                else if (p == '}')
                {
                    codeDepth -= 1;
                }

                if (codeDepth < 0)
                {
                    endIndex = i;
                    break;
                }
            }

            if (endIndex == -1) throw new Exception("If code block end statement not found.");

            string ifCode = codeBlockBegin.Substring(0, endIndex);

            return new Tuple<string, bool, int>(ifCode, result, endIndex + endCondIndex + 2);
        }

        public static Tuple<string, int> ParseUserDefinedFunction(DiverLuck diver, string program, int indexBegin)
        {
            string funBegin = program.Substring(indexBegin + 1);
            int funDepth = 0;
            int endIndex = -1;

            for (int i = 0; i < funBegin.Length; i++)
            {
                char p = funBegin[i];

                if (p == '$')
                {
                    endIndex = i;
                    break;
                }
            }

            if (endIndex == -1) throw new Exception("Function call end statement not found.");

            var funId = int.Parse(funBegin.Substring(0, endIndex));

            diver.debug.UserFunctionExec(funId);

            return new Tuple<string, int>(diver.functions[funId], endIndex);
        }
    }
}
