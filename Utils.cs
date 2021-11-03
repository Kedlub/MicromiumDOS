using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicromiumDOS
{
    static class Utils
    {
        #region Main Features Drawing
        static string lastTitle = "";

        //Draws titlebar
        public static void DrawTitleBar(string title)
        {
            saveCursor();
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            int width = Console.WindowWidth;
            if (width == 0) return;
            width = width - title.Length;
            width = width / 2;
            string beforeTitle = new String(' ', width);
            string afterTitle = new String(' ', width);
            Console.WriteLine($"{beforeTitle}{title}{afterTitle}");
            if (cursorY == 0) cursorY = 1;
            restoreCursor();
            Console.ResetColor();
            lastTitle = title;
        }

        // Draw alert box
        // Alert box is a box with a message inside
        public static void DrawAlertBox(string text, int time = 0, ConsoleColor backgroundColor = ConsoleColor.Blue, ConsoleColor textColor = ConsoleColor.White)
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            width = width / 2;
            height = height / 2;
            int halfText = text.Length / 2;
            string emptyLine = new String(' ', text.Length + 4);
            saveCursor();
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = textColor;
            /*Console.SetCursorPosition(width - halfText - 2, height - 2);
            Console.WriteLine(emptyLine);/*/
            Console.SetCursorPosition(width - halfText - 2, height - 1);
            Console.WriteLine(emptyLine);
            Console.SetCursorPosition(width - halfText - 2, height);
            Console.WriteLine($"  {text}  ");
            Console.SetCursorPosition(width - halfText - 2, height + 1);
            Console.WriteLine(emptyLine);
            /*Console.SetCursorPosition(width - halfText - 2, height + 2);
            Console.WriteLine(emptyLine);*/
            Console.ResetColor();
            if (cursorY == 0)
            {
                cursorY = 2;
            }
            restoreCursor();
            if (time != 0)
            {
                System.Threading.Thread.Sleep(time);
                /*Console.SetCursorPosition(width - halfText - 2, height - 1);
                Console.WriteLine(emptyLine);
                Console.SetCursorPosition(width - halfText - 2, height);
                Console.WriteLine(emptyLine);
                Console.SetCursorPosition(width - halfText - 2, height + 1);
                Console.WriteLine(emptyLine);
                restoreCursor();*/
                RedrawBuffer();
            }
            //Console.SetCursorPosition(width + halfText+1, height);
        }

        public enum DialogBoxType
        {
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel,
            RetryAbortCancel,
            Cancel
        }

        public static void ShowDialogBox(string text, DialogBoxType type)
        {

        }

        public enum SystemInfoType
        {
            Info,
            Error,
            InputNeeded,
            Debug,
            Warning
        }

        // Print system-level text, not available from scripts
        public static void PrintSystemText(string text, SystemInfoType infoType, bool newLine = true)
        {
            ConsoleColor origColor = Console.ForegroundColor;
            switch (infoType)
            {
                case SystemInfoType.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Write("[*] ");
                    Console.ResetColor();
                    break;
                case SystemInfoType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Write("[!] ");
                    Console.ResetColor();
                    break;
                case SystemInfoType.InputNeeded:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Write("[?] ");
                    Console.ResetColor();
                    break;
                case SystemInfoType.Debug:
                    if (!Program.debugMode) return;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Write("[*] ");
                    Console.ResetColor();
                    break;
                case SystemInfoType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Write("[!] ");
                    Console.ResetColor();
                    break;
                default:
                    break;
            }
            Console.ForegroundColor = origColor;
            if (newLine)
                WriteLine(text);
            else
                Write(text);
        }

        /* 
            Buffer of all lines printed to the console
            includes colors in !{FG_COLOR;BG_COLOR} format

            ONLY USE WHILE PRINTING DIRECTLY TO CONSOLE
        */
        static string buffer = "";

        // Prints new line to console
        public static void WriteLine(object text)
        {
            Write(text + "\n");
        }

        // Prints text to the console
        public static void Write(object text)
        {
            ConsoleColor fgColor = Console.ForegroundColor;
            ConsoleColor bgColor = Console.BackgroundColor;
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            System.Console.Write(text);
            buffer += "&!{" + (int)fgColor + ";" + (int)bgColor + ";" + x + ";" + y + "}&" + text;
        }

        // Clears the console
        public static void Clear()
        {
            Console.Clear();
            ClearBuffer();
        }

        // Clears the buffer
        public static void ClearBuffer()
        {
            buffer = "";
        }

        // Redraws the console using buffer
        // Useful after opening dialog or drawing to console
        // without using Write() or WriteLine()
        public static void RedrawBuffer()
        {
            Console.Clear();
            DrawTitleBar(lastTitle);
            Console.SetCursorPosition(0, 1);
            string[] buf = buffer.Split('&');
            foreach (var word in buf)
            {
                if (word.StartsWith("!{"))
                {
                    string[] tmp = word.Replace("!{", "").Replace("}", "").Split(";");
                    Console.ForegroundColor = (ConsoleColor)Convert.ToInt32(tmp[0]);
                    Console.BackgroundColor = (ConsoleColor)Convert.ToInt32(tmp[1]);
                    Console.SetCursorPosition(Convert.ToInt32(tmp[2]), Convert.ToInt32(tmp[3]));
                    continue;
                }

                Console.Write(word);
            }
            PrintSystemText("Redrawn from buffer", SystemInfoType.Debug);
        }


        #endregion

        #region Shape Drawing
        //TODO Shape Drawing
        #endregion

        static int cursorX;
        static int cursorY;

        // Save cursor position
        // Useful when drawing manually to console
        static void saveCursor()
        {
            cursorX = Console.CursorLeft;
            cursorY = Console.CursorTop;
        }

        // Restore cursor position
        // Useful when drawing manually to console
        static void restoreCursor()
        {
            Console.SetCursorPosition(cursorX, cursorY);
        }
    }
}
