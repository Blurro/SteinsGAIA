using SteinsGAIA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

public class Program
{
    public class GloDat
    {
        public static List<string> Events = new List<string>();
        public static List<string> EventCauses = new List<string>();
        public static List<string> TTEntities = new List<string>();
        public static List<string> AttFields = new List<string>();
        public static List<string> DateOrd = new List<string>();
        public static List<string> RSUsers = new List<string>();
        public static List<string> WLBegin = new List<string>();
        public static List<string> BTTDates = new List<string>();
        public static List<string> ListTags = new List<string> { "###events", "###eventcauses", "###tt-entities", "###AttractorFields", "###dateOrder", "###rs-users", "###Worldline1", "###btt-arrival-dates" };
        public static List<List<string>> allLists = new List<List<string>> { Events, EventCauses, TTEntities, AttFields, DateOrd, RSUsers, WLBegin, BTTDates };
        public static string evDate = null;
        public static string evBttDate = null;
        public static string evType = null;
        public static string evLabel = null;
        public static string evColor = null;
        public static List<int> evCauses = new List<int>();
        public static List<int> evPreventatives = new List<int>();
        public static string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static List<string> EventsSortedByDate = new List<string>();
        public static List<string> EventCausesSortedByDate = new List<string>();
        public static string removeChars = Regex.Escape(@"#/><|()\*");
        public static string removeCharsSpace = removeChars + " ";
        public static string random = null;
        public static Random randomVal = new Random();
        public static int randomValue = randomVal.Next(0, 100);
        public static string cancelCode = "/c";
    }

    public static void PrintConsole(bool logo)
    {
        //for (int i = 0; i < 200; i++)
        //{
        //    Console.WriteLine(i + " anhahahhufrefauhfwnajiodifwuflwuihbsfjkngiuerjw");
        //}
        //Console.ReadKey();

        Console.Clear(); // Clear the visible portion

        //Console.ReadKey();

        string preMsgCode = "\u001b[34m";
        if (logo)
        {
            preMsgCode = "\u001b[26A\u001b[34m";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SteinsGAIA.logo.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                int lineNumber = 1;
                Console.WriteLine("\n\n\n");
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine("\u001b[30m..................................................\u001b[34m" + line);
                    lineNumber++;
                }
            }
        }
        Console.WriteLine(preMsgCode + "SteinsGAIA : the world line chronology calculator\u001b[37m");
        Console.WriteLine("Early build V0.2, generations will appear below here\n");
    }

    [STAThread]
    static void Main(string[] args)
    {
        bool logo = true;
        PrintConsole(logo);
        //string filePathInput = null;
        string filePathInput = Path.Combine(GloDat.exeDirectory, "worldline.txt");
        for (int i = 0; i < args.Length; i++)
        {
            if (File.Exists(args[i]))
            {
                filePathInput = args[i];
            }
        }
        if (filePathInput == null)
        {
            Console.WriteLine("Drag n drop a file yo");
            Console.ReadKey(true);
            Environment.Exit(0);
        } else if (!File.Exists(filePathInput))
        {
            Console.WriteLine("No " + Path.GetFileName(filePathInput) + " file found");
            Console.ReadKey(true);
            Environment.Exit(0);
        }

        // start of cool coding
        string[] lines = File.ReadAllLines(filePathInput);

        for (int i = 0; i < GloDat.allLists.Count; i++)
        {
            List<string> newList = GetList(lines, GloDat.ListTags[i]);
            GloDat.allLists[i].Clear();
            GloDat.allLists[i].AddRange(newList);
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        CoolForm CoolForm = new CoolForm();
        
        for (int i = 0; i < GloDat.allLists.Count; i++)
        {
            CoolForm.LoadAllLists(GloDat.allLists[i], i);
        }
        CoolForm.UpdText(TextUpdateToTXTwl());

        Application.Run(CoolForm);

        //CoolForm.Close();
    }

    public static List<string> GetList(string[] lines, string label)
    {
        List<string> result = new List<string>();
        bool lineStart = false;

        foreach (string line in lines)
        {
            if (line.Contains(label))
            {
                lineStart = true;
                continue;
            }
            if (lineStart)
            {
                if (line == "-")
                {
                    break;
                }
                result.Add(line);
            }
        }
        return result;
    }

    public static string TextUpdateToTXTwl()
    {
        List<int> matchingIndices = new List<int>();
        foreach (string item in GloDat.WLBegin)
        {
            int index = GloDat.Events.IndexOf(item);
            if (index == -1)
            {
                Console.WriteLine("error");
                Console.ReadKey(true);
                Environment.Exit(0);
            }
            matchingIndices.Add(index + 1);
        }
        return string.Join(" ", matchingIndices);
    }

    public static string ParseEvent(string ev, bool colors)
    {
        char[] separators = { '<', '>', '(', ')', '#', '\\', '/', '*' };
        string[] parts = ev.Split(new [] {'|'}, StringSplitOptions.RemoveEmptyEntries);
        List<string> influences = new List<string>();
        if (parts.Length > 0)
        {
            for (int i = 1; i < parts.Length; i++) //add all skipping first
            {
                influences.Add(parts[i]);
            }
        }
        string BTTInfluenceAddon = string.Join(", ", influences);
        parts = parts[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
        string evText = null;
        GloDat.evLabel = parts[0];
        GloDat.evDate = parts[1];

        int[] rgb = GenerateColorFromText($"{GloDat.evLabel}{GloDat.randomValue}"); //epic colors wooooo

        if (ev.Contains("<"))
        {
            if (ev.Contains("\\"))
            {
                GloDat.evBttDate = parts[2];
                GloDat.evType = "bttdep";
                evText = "BTT-" + parts[0] + " leaves from date " + parts[1] + " to date " + GloDat.BTTDates[int.Parse(GloDat.evBttDate) - 1];
            }
            if (ev.Contains("/"))
            {
                GloDat.evType = "fttdep";
                evText = $"FTT-{parts[0]} leaves from date {parts[1]} to date {parts[2]}";
            }
        } else if (ev.Contains(">"))
        {
            string tt = "BTT-";
            if (ev.Contains("\\"))
            {
                GloDat.evType = "bttarv";
                GloDat.evDate = GloDat.BTTDates[int.Parse(parts[1]) - 1];
                parts[1] = GloDat.evDate;
            }
            if (ev.Contains("/"))
            {
                tt = "FTT-";
                GloDat.evType = "fttarv";
            }
            evText = $"{tt}{parts[0]} arrives on date {parts[1]}";
            if (colors) {
                int r2 = rgb[0] / 2;
                int g2 = rgb[1] / 2;
                int b2 = rgb[2] / 2;
                if (r2 > g2 && r2 > b2)
                {
                    g2 += (r2 - g2) / 2;
                    b2 += (r2 - b2) / 2;
                }
                else if (g2 > r2 && g2 > b2)
                {
                    r2 += (g2 - r2) / 2;
                    b2 += (g2 - b2) / 2;
                }
                else
                {
                    g2 += (b2 - g2) / 2;
                    r2 += (b2 - r2) / 2;
                }
                evText += $"\u001b[38;2;{r2};{g2};{b2}m"; // convoluted lil thing for simple detail lol
            };
            evText += $"{(BTTInfluenceAddon.Length > 0 ? $" - Influenced by event{(BTTInfluenceAddon.Length > 2 ? "s" : "")} {BTTInfluenceAddon}" : "")}";
        } else
        {
            GloDat.evType = "norm";
            evText = parts[0] + " on date " + parts[1];
        }
        GloDat.evColor = $"\u001b[38;2;{rgb[0]};{rgb[1]};{rgb[2]}m";
        if (colors)
        {
            return $"{GloDat.evColor}{evText}\u001b[37m";
        }
        return evText;
    }

    public static int[] GenerateColorFromText(string input)
    {
        byte[] hashBytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

        // Extract three numbers from the hash
        int r = hashBytes[0];
        int g = hashBytes[1];
        int b = hashBytes[2];

        // Ensure at least one of the numbers has a min brightness
        int minim = 200;
        if (r < minim && g < minim && b < minim)
        {
            if (r > g && r > b)
                r = minim;
            else if (g > r && g > b)
                g = minim;
            else
                b = minim;
        }

        // Ensure that no number exceeds 255 after adjustment
        r = Math.Min(r, 255);
        g = Math.Min(g, 255);
        b = Math.Min(b, 255);

        return new int[] { r, g, b };
    }
}