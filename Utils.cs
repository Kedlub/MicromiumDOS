using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        // Create list menu
        // Function listens to key press, so user can highlight item using arrow keys, and press enter to select it
        public static int CreateListMenu(string title, string[] items, ConsoleColor backgroundColor = ConsoleColor.Blue, ConsoleColor textColor = ConsoleColor.White)
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            width = width / 2;
            height = height / 2;

            // hide cursor
            Console.CursorVisible = false;

            // get longest item name
            int longestItem = 0;
            int halfText = longestItem / 2;
            foreach (string item in items)
            {
                if (item.Length > longestItem)
                {
                    longestItem = item.Length;
                }
            }

            // get window position offset, so it is centered
            int offset = longestItem / 2;

            // add spaces to both sides of shorter items, so their name are centered
            // also add other items to the list
            string[] newItems = new string[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                string item = items[i];
                int halfItem = item.Length / 2;
                int spaces = longestItem - item.Length;
                string beforeItem = new String(' ', spaces / 2);
                string afterItem = new String(' ', spaces / 2);
                newItems[i] = $"{beforeItem}{item}{afterItem}";
            }

            // add spaces to both sides of title, so it is centered
            int halfTitle = title.Length / 2;
            string beforeTitle = new String(' ', longestItem - title.Length);
            string afterTitle = new String(' ', longestItem - title.Length);
            string titleNew = $"{beforeTitle}{title}{afterTitle}";


            // calculate height offset from items and title, so entire menu is centered
            int heightOffset = (items.Length + 3) / 2;

            // print out all items
            saveCursor();
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = textColor;
            Console.SetCursorPosition(width - halfText - 2, height - heightOffset);
            Console.WriteLine(new String(' ', longestItem + 4));
            Console.SetCursorPosition(width - halfText - 2, height - heightOffset + 1);
            Console.WriteLine($"  {titleNew}  ");
            Console.SetCursorPosition(width - halfText - 2, height - heightOffset + 2);
            Console.WriteLine(new String(' ', longestItem + 4));
            for (int i = 0; i < items.Length; i++)
            {
                Console.SetCursorPosition(width - halfText - 2, height - heightOffset + 3 + i);
                Console.WriteLine($"  {newItems[i]}  ");
            }
            Console.SetCursorPosition(width - halfText - 2, height - heightOffset + items.Length + 3);
            Console.WriteLine(new String(' ', longestItem + 4));
            Console.ResetColor();

            // listen to key press, and highlight selected item
            int selectedItem = 0;
            int lastSelectedItem = 0;
            // Select first item
            Console.SetCursorPosition(width - halfText - 2, height - heightOffset + 3 + selectedItem);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"  {newItems[selectedItem]}  ");
            Console.ResetColor();
            Console.SetCursorPosition(width - halfText - 2, height - heightOffset + 3 + selectedItem);


            while (true)
            {
                lastSelectedItem = selectedItem;
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (selectedItem > 0)
                    {
                        selectedItem--;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (selectedItem < items.Length - 1)
                    {
                        selectedItem++;
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                Console.SetCursorPosition(width - halfText - 2, height - heightOffset + 3 + selectedItem);
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"  {newItems[selectedItem]}  ");
                Console.ResetColor();
                Console.SetCursorPosition(width - halfText - 2, height - heightOffset + 3 + selectedItem);

                // Reset color of last selected item
                if (lastSelectedItem != selectedItem)
                {
                    Console.SetCursorPosition(width - halfText - 2, height - heightOffset + 3 + lastSelectedItem);
                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = textColor;
                    Console.WriteLine($"  {newItems[lastSelectedItem]}  ");
                    Console.ResetColor();
                    Console.SetCursorPosition(width - halfText - 2, height - heightOffset + 3 + selectedItem);
                }
            }

            // show cursor
            Console.CursorVisible = true;

            restoreCursor();

            RedrawBuffer();

            return selectedItem;
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

        // Create dialog box
        // Function listens to key press, so user can highlight item using arrow keys, and press enter to select it
        // Function returns selected button
        public static int ShowDialogBox(string text, DialogBoxType type)
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            width = width / 2;
            height = height / 2;

            // hide cursor
            Console.CursorVisible = false;

            // calculate window size from text length
            int textLength = text.Length;
            int windowWidth = textLength + 4;
            int windowHeight = 5;

            // calculate width offset from window width, so entire window is centered
            int widthOffset = (width - (windowWidth / 2));

            // calculate height offset from window height, so entire window is centered
            int heightOffset = (height - (windowHeight / 2));

            // print out window
            saveCursor();
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(widthOffset, heightOffset);
            Console.WriteLine(new String(' ', windowWidth));
            Console.SetCursorPosition(widthOffset, heightOffset + 1);
            Console.WriteLine($"  {text}  ");
            Console.SetCursorPosition(widthOffset, heightOffset + 2);
            Console.WriteLine(new String(' ', windowWidth));
            Console.SetCursorPosition(widthOffset, heightOffset + 3);
            Console.WriteLine(new String(' ', windowWidth));
            Console.SetCursorPosition(widthOffset, heightOffset + 4);
            Console.WriteLine(new String(' ', windowWidth));
            Console.ResetColor();

            // render buttons depending on type
            string[] buttons = new string[0];
            switch (type)
            {
                case DialogBoxType.Ok:
                    buttons = new string[] { "Ok" };
                    break;
                case DialogBoxType.OkCancel:
                    buttons = new string[] { "Ok", "Cancel" };
                    break;
                case DialogBoxType.YesNo:
                    buttons = new string[] { "Yes", "No" };
                    break;
                case DialogBoxType.YesNoCancel:
                    buttons = new string[] { "Yes", "No", "Cancel" };
                    break;
                case DialogBoxType.RetryAbortCancel:
                    buttons = new string[] { "Retry", "Abort", "Cancel" };
                    break;
                case DialogBoxType.Cancel:
                    buttons = new string[] { "Cancel" };
                    break;
            }

            // calculate longest button length
            int longestButton = 0;
            foreach (string button in buttons)
            {
                if (button.Length > longestButton)
                {
                    longestButton = button.Length;
                }
            }

            // calculate button start offset, so they are centered
            // depending on number of buttons, and size of longest button
            int buttonStartOffset = (windowWidth - (longestButton * buttons.Length)) / 2;

            // draw buttons, and highlight the first one
            int selectedButton = 0;
            int lastSelectedButton = 0;
            for (int i = 0; i < buttons.Length; i++)
            {
                Console.SetCursorPosition(widthOffset + buttonStartOffset + (i * longestButton), heightOffset + 3);
                Console.WriteLine(buttons[i]);
            }
            Console.SetCursorPosition(widthOffset + buttonStartOffset + (selectedButton * longestButton), heightOffset + 3);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(buttons[selectedButton]);
            Console.ResetColor();

            // listen to key press, and highlight selected button
            while (true)
            {
                lastSelectedButton = selectedButton;
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (selectedButton > 0)
                    {
                        selectedButton--;
                    }
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    if (selectedButton < buttons.Length - 1)
                    {
                        selectedButton++;
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                Console.SetCursorPosition(widthOffset + buttonStartOffset + (selectedButton * longestButton), heightOffset + 3);
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(buttons[selectedButton]);
                Console.ResetColor();
                Console.SetCursorPosition(widthOffset + buttonStartOffset + (selectedButton * longestButton), heightOffset + 3);

                // Reset color of last selected button
                if (lastSelectedButton != selectedButton)
                {
                    Console.SetCursorPosition(widthOffset + buttonStartOffset + (lastSelectedButton * longestButton), heightOffset + 3);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(buttons[lastSelectedButton]);
                    Console.ResetColor();
                    Console.SetCursorPosition(widthOffset + buttonStartOffset + (selectedButton * longestButton), heightOffset + 3);
                }
            }

            // show cursor
            Console.CursorVisible = true;

            restoreCursor();

            RedrawBuffer();

            return selectedButton;
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

        // Check if executable has resource with name
        public static bool HasResource(string name)
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(name);
        }

        // Read embedded resources
        public static string ReadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
            using (var streamReader = new StreamReader(resourceStream))
            {
                return streamReader.ReadToEnd();
            }
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
