using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace MicromiumDOS
{
    class Program
    {
        public static bool debugMode = true;
        static void Main(string[] args)
        {
            /*Console.WriteLine("Test");
            Utils.DrawAlertBox("Zkouškaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");*/
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(exeDir);

            Utils.CreateListMenu("Test", new string[] { "Load", "Compile", "Very long test, that will...." });
            Utils.ShowDialogBox("Pozor vyvolávači! Blablablablablablabla", Utils.DialogBoxType.RetryAbortCancel);

            /*if (Utils.HasResource("script"))
            {
                //string metadata = Utils.ReadEmbeddedResource("metadata");
                string code = Utils.ReadEmbeddedResource("script");

                RunScript(code);
            }
            else
            {
                ConsoleMode();
            }

            Console.ReadKey();*/
        }

        static void ConsoleMode()
        {
            Utils.DrawTitleBar("MicromiumDOS - Language Parser");
            Utils.PrintSystemText("Vítejte", Utils.SystemInfoType.Info);
            Utils.PrintSystemText("Co chcete spusit za program? CZS nebo MIS", Utils.SystemInfoType.InputNeeded);
            System.Threading.Thread.Sleep(500);
            string fileName = Console.ReadLine();
            Utils.PrintSystemText($"Spouštím soubor {fileName}", Utils.SystemInfoType.Info);
            System.Threading.Thread.Sleep(1000);
            Utils.Clear();
            Utils.DrawTitleBar($"MicromiumDOS - {fileName}");
            string fileExt = fileName.Split('.')[1];
            string code = File.ReadAllText($"{fileName}");
            if (fileExt.ToLower() == "mis")
            {
                //MiSharpLiteParser.Parse(code);
                RunScript(code);
            }
            /*else if (fileExt.ToLower() == "czs")
            {
                CzechSharpParser.Parse(code);
            }
            else if (fileExt.ToLower() == "nya")
            {
                NyaSharpParser.Parse(code);
            }*/
            else if (fileExt.ToLower() == "exe")
            {
                Utils.PrintSystemText("Nemůžu spustit EXE soubory! Copak vypadám jako Windows?", Utils.SystemInfoType.Error);
            }
            else
            {
                Utils.PrintSystemText("Nemůžu spustit tento typ souboru!", Utils.SystemInfoType.Error);
            }
        }

        // Loads script and executes it using compatible parser
        static void RunScript(string code)
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                MiSharpLiteParser.Parse(code);
                watch.Stop();
                Utils.PrintSystemText("Konec programu! Čas běhu: " + watch.ElapsedMilliseconds / 1000f + "s", Utils.SystemInfoType.Info);
            }
            catch (FileNotFoundException)
            {
                Utils.PrintSystemText("Soubor neexistuje!", Utils.SystemInfoType.Error);
            }
            catch (IndexOutOfRangeException e)
            {
                Utils.PrintSystemText("Nastala chyba!", Utils.SystemInfoType.Error);
                Utils.PrintSystemText("*já si říkal že na tomhle radši nemám dělat*", Utils.SystemInfoType.Error);
                Utils.PrintSystemText("Popis chyby: " + e.Message, Utils.SystemInfoType.Error);
                Utils.PrintSystemText("Možná jste zadali špatný počet argumentů do funkce?", Utils.SystemInfoType.Error);
            }
            catch (Exception e)
            {
                Utils.PrintSystemText("Nastala chyba!", Utils.SystemInfoType.Error);
                Utils.PrintSystemText("*já si říkal že na tomhle radši nemám dělat*", Utils.SystemInfoType.Error);
                Utils.PrintSystemText("Popis chyby: " + e.Message, Utils.SystemInfoType.Error);
                //Utils.PrintSystemText("Lokace chyby: " + e.Source, Utils.SystemInfoType.Error);
            }
        }
    }
}
