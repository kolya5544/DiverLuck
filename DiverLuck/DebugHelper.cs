using System.Reflection;

namespace DiverLuckCore
{
    public class DebugHelper
    {
        public bool debugEnabled = false;
        public int executionPointer = 0;
        public string programCode = "";

        public void OutputDebugInfo()
        {
            string beforeDebug = programCode.Substring(0, executionPointer);
            string debugChar = programCode.Substring(executionPointer, 1);
            string afterDebug = programCode.Substring(executionPointer + 1);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(beforeDebug);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(debugChar);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(afterDebug);
            Console.ResetColor();
        }

        public void InvokeDebug(MethodInfo mi)
        {
            if (!debugEnabled) return;
            Console.WriteLine($"Executed {mi.Name} by {executionPointer}@{programCode}");
        }

        public void PullDebug(object obj)
        {
            if (!debugEnabled) return;
            Console.WriteLine($"Pulled obj {obj.GetType()} by {executionPointer}@{programCode}");
        }

        public void PushDebug(object obj)
        {
            if (!debugEnabled) return;
            Console.WriteLine($"Pushed obj {obj.GetType()} by {executionPointer}@{programCode}");
        }

        public void UserFunctionExec(int id)
        {
            if (!debugEnabled) return;
            Console.WriteLine($"Exec fun#{id} request by {executionPointer}@{programCode}");
        }
    }
}