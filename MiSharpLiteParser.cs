using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicromiumDOS
{
    static class MiSharpLiteParser
    {
        static Dictionary<string, string> variables;
        public static void Parse(string code)
        {
            variables = new Dictionary<string, string>();
            Random rnd = new Random();
            bool multilineComment = false;
            string[] functions = code.Split(new[] { "\r\n", "\r", "\n", ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string func in functions)
            {
                if (func.StartsWith("//")) continue;
                if (func.Contains("*/"))
                {
                    multilineComment = false;
                    continue;
                }
                if (func.Contains("/*"))
                {
                    multilineComment = true;
                    continue;
                }
                if (multilineComment) continue;
                if (string.IsNullOrWhiteSpace(func)) continue;
                object[] args = ParseArgs(func);
                switch (func.Split('(')[0])
                {
                    case "console.write":
                        Utils.WriteLine(args[0]);
                        break;
                    case "console.alertbox":
                        Utils.DrawAlertBox((string)args[0], (int)ArrayCheckNull(args, 1, 0));
                        break;
                    case "console.settitle":
                        Utils.DrawTitleBar((string)args[0]);
                        break;
                    case "wait":
                        System.Threading.Thread.Sleep(Convert.ToInt32(args[0]));
                        break;
                    case "random":
                        OutputFunction(rnd.Next(0, Convert.ToInt32(args[0])).ToString(), func, variables);
                        break;
                    case "value":
                        OutputFunction((string)args[0], func, variables);
                        break;
                    case "input":
                        Utils.PrintSystemText(Convert.ToString(args[0]), Utils.SystemInfoType.InputNeeded, false);
                        OutputFunction(Console.ReadLine(), func, variables);
                        break;
                    default:
                        break;
                }
            }
        }

        static void OutputFunction(string output, string func, Dictionary<string, string> variables)
        {
            String[] asSplit = func.Split(new[] { "as " }, StringSplitOptions.None);
            if (asSplit.Length == 2)
            {
                string variableName = asSplit[1].Split(' ')[1];
                switch (asSplit[1].Split(' ')[0])
                {
                    case "int":
                        Utils.PrintSystemText($"Saving {output} to variable {variableName} as int", Utils.SystemInfoType.Debug);
                        variables.Add(variableName, output);
                        return;
                    case "string":
                        Utils.PrintSystemText($"Saving {output} to variable {variableName} as string", Utils.SystemInfoType.Debug);
                        variables.Add(variableName, output);
                        return;
                    default:
                        Utils.PrintSystemText("Program tried to save value into unknown data type: " + asSplit[1].Split(' ')[0], Utils.SystemInfoType.Warning);
                        break;
                }
            }

            Utils.WriteLine("Func output: " + output);
        }


        static void IfExecute()
        {

        }

        static void GetFunctionName(string func)
        {

        }

        static object ArrayCheckNull(object[] arr, int index, object defaultValue)
        {
            if (arr.Length - 1 < index)
            {
                Utils.PrintSystemText($"Returning default value: {defaultValue}", Utils.SystemInfoType.Debug);
                return defaultValue;
            }
            else
            {
                Utils.PrintSystemText($"Returning value from array: {arr[index]}", Utils.SystemInfoType.Debug);
                return arr[index];
            }
        }

        static object[] ParseArgs(string function)
        {
            List<Object> almostParsedArgs = new List<Object>();
            /*string[] args = function.Split('(', ')')[1].Split(',');
            foreach (string arg in args)
            {
                int resultInt;
                if (arg.Contains('"'))
                {
                    //Utils.PrintSystemText($"Regexing string, input: {arg} , output: {reg.Match(arg).Value}", Utils.SystemInfoType.Debug);
                    string parsedString = ParseString(arg);
                    parsedArgs.Add(parsedString);
                }
                else if (int.TryParse(arg, out resultInt))
                {
                    parsedArgs.Add(resultInt);
                }
                else
                {
                    //parsedVariables.Add(arg);
                    if (variables.ContainsKey(arg))
                    {
                        string variableValue;
                        bool gotValue = variables.TryGetValue(arg, out variableValue);
                        if (gotValue)
                            parsedArgs.Add(variableValue);
                    }
                }
                
            }*/
            char[] args = function.Split('(', ')')[1].ToCharArray();
            bool isInString = false;
            string tempArg = "";
            for (int i = 0; i < args.Length; i++)
            {
                char ch = args[i];
                if (ch == '"')
                {
                    isInString = !isInString;
                }
                if ((ch == ',' && !isInString) || i == args.Length - 1)
                {
                    if (i == args.Length - 1) tempArg += ch;
                    almostParsedArgs.Add(tempArg.Trim());
                    tempArg = "";
                    continue;
                }
                tempArg += ch;
            }
            List<Object> parsedArgs = new List<object>();
            foreach (string arg in almostParsedArgs)
            {
                int resultInt;
                if (arg.Contains('"'))
                {
                    //Utils.PrintSystemText($"Regexing string, input: {arg} , output: {reg.Match(arg).Value}", Utils.SystemInfoType.Debug);
                    string parsedString = ParseString(arg);
                    parsedArgs.Add(parsedString);
                }
                else if (int.TryParse(arg, out resultInt))
                {
                    parsedArgs.Add(resultInt);
                }
                else
                {
                    //parsedVariables.Add(arg);
                    if (variables.ContainsKey(arg))
                    {
                        string variableValue;
                        bool gotValue = variables.TryGetValue(arg, out variableValue);
                        if (gotValue)
                            parsedArgs.Add(variableValue);
                    }
                }

            }
            return parsedArgs.ToArray();
        }

        static string ParseString(string input)
        {
            input += " ";
            string parsedString = "";
            bool escapeCharacter = false;
            bool isInQuotes = false;
            string tmpString = "";
            foreach (char ch in input)
            {
                if (!isInQuotes && ch == '"')
                {
                    isInQuotes = true;
                    continue;
                }
                else if (isInQuotes && !escapeCharacter && ch == '"')
                {
                    isInQuotes = false;
                    continue;
                }

                if (ch == '\\')
                {
                    escapeCharacter = true;
                    continue;
                }

                if (isInQuotes)
                {
                    parsedString += ch;
                }
                else if (!isInQuotes)
                {
                    if (ch == ' ' && tmpString != "")
                    {
                        if (variables.ContainsKey(tmpString))
                        {
                            bool gotValue = variables.TryGetValue(tmpString, out string str);
                            if (gotValue)
                            {
                                parsedString += str;
                            }
                            else
                            {
                                Utils.PrintSystemText($"Variable {tmpString} does not exist", Utils.SystemInfoType.Error);
                            }
                            tmpString = "";
                            continue;
                        }
                    }
                    if (ch == ' ') continue;
                    if (ch == '+') continue;
                    tmpString += ch;
                }
            }
            return parsedString;
        }

        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }
}
