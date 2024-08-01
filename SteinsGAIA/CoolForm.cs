using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Program;

namespace SteinsGAIA
{
    public partial class CoolForm : Form
    {
        public CoolForm()
        {
            InitializeComponent();
            textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBox1_KeyPress);
        }

        private void CoolForm_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private ListBox[] listBoxes = new ListBox[8];

        public void LoadAllLists(List<string> lisData, int lisNum)
        {
            listBoxes[0] = listBox1;
            listBoxes[1] = listBox2;
            listBoxes[2] = listBox3;
            listBoxes[3] = listBox4;
            listBoxes[4] = listBox5;
            listBoxes[5] = listBox6;
            listBoxes[6] = listBox7;
            listBoxes[7] = listBox8;
            int i = 0;

            foreach (string s in lisData)
            {
                i++;
                if (new[] { 3, 4, 5 }.Contains(lisNum))
                {
                    listBoxes[lisNum].Items.Add(s);
                } else if (lisNum == 6)
                {
                    listBoxes[lisNum].Items.Add("[" + (GloDat.Events.IndexOf(s) + 1) + "] " + ParseEvent(s));
                }
                else
                {
                    listBoxes[lisNum].Items.Add(i + ") " + s);
                }
            }
        }

        public void UpdText(string text)
        {
            textBox1.Text = text;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = new string(textBox1.Text.Where(c => char.IsDigit(c) || c == ' ').ToArray());
            textBox1.Text = Regex.Replace(textBox1.Text, @"\s+", " ");
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        { //only allows digits, spaces, backspace and ctrl+c
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != ' ' && e.KeyChar != '\b' && !(e.KeyChar == 3 && ModifierKeys.HasFlag(Keys.Control)))
            {
                e.Handled = true; // Prevent the character from being entered
            }
        }

        private void listBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void DateSortEvents()
        {
            for (int d = 0; d < GloDat.DateOrd.Count; d++)
            {
                for (int e = 0; e < GloDat.Events.Count; e++)
                {
                    ParseEvent(GloDat.Events[e]);

                    if (GloDat.evDate != GloDat.DateOrd[d] || GloDat.EventsSortedByDate.Contains(GloDat.Events[e]))
                    {
                        //Console.WriteLine(GloDat.DateOrd[d]); // debug print
                        continue;
                    }

                    GloDat.EventsSortedByDate.Add(GloDat.Events[e]);
                    GloDat.EventCausesSortedByDate.Add(GloDat.EventCauses[e]);
                }
            }
            //foreach (var pair in GloDat.EventsSortedByDate)
            //{
            //    Console.WriteLine(pair);
            //}
            //foreach (var pair in GloDat.EventCausesSortedByDate)
            //{
            //    Console.WriteLine(pair);
            //}
        }

        private void UpdWL_Click(object sender, EventArgs e)
        {
            bool doGen = true;
            List<string> CurrentWorldLine = new List<string>();
            if (string.IsNullOrWhiteSpace(textBox1.Text)) // if the textbox is empty rather than emptying list it restores list to textbox
            {
                string a = "";
                if (listBox7.Items.Count == 0)
                {
                    doGen = false;
                } else
                {
                    foreach (string item in listBox7.Items)
                    {
                        char[] chars = { '[', ']' };
                        a += item.Split(chars, StringSplitOptions.RemoveEmptyEntries)[0] + " ";
                    }
                }
                UpdText(a);
            }
            List<KeyValuePair<string, int>> data = new List<KeyValuePair<string, int>>();
            char[] chars2 = { ' ' };
            foreach (string ev in textBox1.Text.Split(chars2, StringSplitOptions.RemoveEmptyEntries)) //go over every number in txtbox
            {
                if (int.Parse(ev) - 1 < GloDat.Events.Count) //check number is an existing event
                {
                    string i = GloDat.Events[int.Parse(ev) - 1];
                    ParseEvent(i);
                    sortCauses(GloDat.EventCauses[int.Parse(ev) - 1]);
                    if (GloDat.evCauses.Count == 0)
                    {
                        data.Add(new KeyValuePair<string, int>(i, GloDat.DateOrd.IndexOf(GloDat.evDate)));
                    } else
                    {
                        Console.WriteLine("Events with + causes will ONLY happen when a listed cause event exists too.\nIf the cause event exists it'll happen without you havin' to add it anyway.\nNow that's a pro tip! -Mitchie");
                    }
                }
            }

            string result = "";
            listBox7.Items.Clear();
            GloDat.WLBegin.Clear();
            if (data.Count == 0)
            {
                doGen = false;
            }
            else
            {
                data = data.Distinct().ToList();
                data = data.OrderBy(pair => pair.Value).ToList();
                foreach (var pair in data)
                {
                    GloDat.WLBegin.Add(pair.Key);
                    int index = GloDat.Events.IndexOf(pair.Key) + 1;
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += " ";
                    }
                    result += index.ToString();
                    listBox7.Items.Add("[" + (GloDat.Events.IndexOf(pair.Key) + 1) + "] " + ParseEvent(pair.Key));
                }
            }
            UpdText(result);

            int wlCounter = 0;
            if (doGen)
            {
                DateSortEvents();

                bool logo = false;
                PrintConsole(logo);
                string wlStart = GloDat.WLBegin[0];
                List<string> AllWorldLines = new List<string>();

                // WORLDLINE GEN STARTS HERE \/\/\/

                bool endGen = false;
                while (!endGen) // this will loop, one loop = one worldline, all rules are contained within this 'while'
                {
                    endGen = true; // by default the generation of new WLs will end after this one and the future will be allowed to continue, until any type of WL ending event says otherwise.
                    wlCounter++; // before this line, wlCounter is 0, we begin at 1 for WL1
                    AllWorldLines.Add("-"); // every 'WL(num):' is == to a '-' in this code version
                    Console.WriteLine("WL" + wlCounter + ":"); // simply prints "WL1:" increasing the number for every new worldline

                    // formatting - each rule will be one chunk of code, no empty lines between them, and separated by these // RULE comments

                    // RULE ONE: a BTT arriving in the past will arrive before any events that have a higher date.
                    
                    if (wlCounter > 1 && GloDat.evType == "bttarv") // if this is the 2nd+ wl, and last one ended via btt, then this deletes all events happening after the btt arival date, preserving all with a date prior or equal
                    {
                        int j = GloDat.DateOrd.IndexOf(GloDat.evDate);
                        for (int i = CurrentWorldLine.Count - 1; i >= 0; i--)
                        {
                            string ev = CurrentWorldLine[i];
                            ParseEvent(ev);
                            if (GloDat.DateOrd.IndexOf(GloDat.evDate) > j)
                            {
                                CurrentWorldLine.Remove(ev); // remove all events happening after the arriving BTT date. they may return based on causes in later rules.
                            }
                            else
                            {
                                Console.WriteLine(ParseEvent(ev)); // parsing an event will also return a readable sentence version rather than code
                            }
                        }
                        CurrentWorldLine.Add(wlStart); // adding the btt arrival event now
                        Console.WriteLine(ParseEvent(wlStart)); // printing translated readable ver to console
                    }
                    else
                    {
                        CurrentWorldLine.Clear(); // if we got here via convergence breaking instead (no btt, just disobey prev AF) then we will build the WL via other rules.
                    }

                    // RULE TWO: cycle through the list of events in order of date, if their causes exist on the current line, allow its addition

                    for (int i = GloDat.EventsSortedByDate.IndexOf(wlStart); i < GloDat.EventsSortedByDate.Count; i++) // 1 loop = 1 event checked. starting from events with the same date as the event in var 'wlStart', likely the BTT arrival event.
                    {
                        if (CurrentWorldLine.IndexOf(GloDat.EventsSortedByDate[i]) != -1) { continue; } // if the event we are checking already exists on the wl, skip to next event
                        bool addEv = AddEventOrNot(i, CurrentWorldLine); // goes to a function checking if the current worldline contains causes or preventatives for the event in question, returns true (=it should be added) or false
                        string parsed = ParseEvent(GloDat.EventsSortedByDate[i]);
                        if (addEv)
                        {
                            CurrentWorldLine.Add(GloDat.EventsSortedByDate[i]);
                            Console.WriteLine(parsed);
                        }

                        // RULE THREE: if the event being added is a BTT departure then this spells the end of the current worldline, but ONLY IF [its identical arrival exists on the current worldline] OR [its arrival + all current events prior to its arrival do NOT identically match the beginning timeline of ANY prior active worldline, only scanning backwards until the starting worldline of the current AF that was NOT reached via BTT]
                        // tl;dr no change no shift

                        // FIX THIS TO BE, IF THE ARRIVAL + ALL IDENTICAL EVENTS PRIOR ARRIVAL (AND ONLY THESE, NO LESS NO MORE) HAVE EXISTED ON A PRIOR GENERATED WORLDLINE, THEN IT CAN BE SKIPPED
                        if (GloDat.evType == "bttdep")
                        {
                            // if its arrival doesnt exist on wl then end after departure
                            if (CurrentWorldLine.IndexOf(GloDat.Events[GloDat.Events.IndexOf(GloDat.EventsSortedByDate[i]) + 1]) == -1)
                            {
                                string nextStart = GloDat.Events[GloDat.Events.IndexOf(GloDat.EventsSortedByDate[i]) + 1];
                                ParseEvent(nextStart);
                                wlStart = nextStart;
                                endGen = false;
                                Console.WriteLine();
                                break;
                            }
                        }
                    }

                    // RULE FOUR:

                    foreach (string ev in CurrentWorldLine) { AllWorldLines.Add(ev); } //add the fully created current WL to our list of all of them
                    if (wlCounter == 200) // just in case i goof up and make an infinite loop ive put a cap here
                    {
                        endGen = true;
                    }
                    if (endGen)
                    {
                        Console.WriteLine("future continues... Add more events!");
                    }
                }
            }
        }

        private bool AddEventOrNot(int evNum, List<string> WLBuild)
        {
            bool addIt = false;
            sortCauses(GloDat.EventCausesSortedByDate[evNum]); // loads the event's causes (+num+num) and preventatives (-num-num) into lists
            foreach (int p in GloDat.evPreventatives)
            {
                if (WLBuild.IndexOf(GloDat.Events[p - 1]) != -1) // if the preventative in question is present on the worldline, the event has been ruled out from happening, return variable 'addIt' while it is set to 'false'
                {
                    //Console.WriteLine("prevented " + GloDat.EventsSortedByDate[evNum]);
                    return addIt;
                }
            }
            if (GloDat.evCauses.Count > 0)
            {
                foreach (int c in GloDat.evCauses)
                {
                    if (WLBuild.IndexOf(GloDat.Events[c - 1]) != -1) // if the cause event in question is present on the worldline, allow the event to be added
                    {
                        //Console.WriteLine("true 1");
                        addIt = true;
                    }
                }
            } else if (GloDat.WLBegin.IndexOf(GloDat.EventsSortedByDate[evNum]) != -1 && !GloDat.EventCausesSortedByDate[evNum].Contains("/")) // if the event in question has no causes, but was added to the 'starter worldline', assume it happens by default and so we return 'true'
            {
                addIt = true;
            }
            //Console.WriteLine("wgyubhr " + GloDat.EventsSortedByDate[evNum] + "  :  " + GloDat.EventCausesSortedByDate[evNum]);

            return addIt;
        }

        private void sortCauses(string causesList)
        {
            GloDat.evCauses.Clear();
            GloDat.evPreventatives.Clear();

            MatchCollection matches = Regex.Matches(causesList, "-(\\d+)");
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    GloDat.evPreventatives.Add(int.Parse(match.Groups[1].Value));
                }
            }

            matches = Regex.Matches(causesList, "\\+(\\d+)");
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    GloDat.evCauses.Add(int.Parse(match.Groups[1].Value));
                }
            }
        }

        private void EditEv_Click(object sender, EventArgs e)
        {
            foreach (string a in GloDat.WLBegin)
            {
                //Console.WriteLine(ParseEvent(a));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = GloDat.exeDirectory;
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveFileDialog.DefaultExt = "txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = saveFileDialog.FileName;
                    List<string> FileLines = new List<string>();
                    for (int i = 0; i < 8; i++)
                    {
                        FileLines.Add(GloDat.ListTags[i]);
                        FileLines.AddRange(GloDat.allLists[i]);
                        FileLines.Add("-");
                    }
                    File.WriteAllLines(selectedFilePath, FileLines);
                }
            }
        }

        private async void AddEv_Click(object sender, EventArgs e)
        {
            if (panel1.Visible == false)
            {
                panel1.Visible = true;
                bool logo = false;
                PrintConsole(logo);
                Console.WriteLine("For any input, type '/cancel' to escape without modifying the config.\n");
                Console.WriteLine("Add normal or TT event? Enter 'n' or 'tt'.\n(normal = 'okabe walks to the shop', special = btt departure etc)");
                for (int i = 0; i < 1; i+=0)
                {
                    string input = await ReadConsoleInputAsync();
                    if (input == "/cancel") { break; }
                    if (input == "n")
                    {

                    } else if (input == "tt")
                    {

                    } else { continue; }
                    break;
                }
                panel1.Visible = false;
            }
        }

        private Task<string> ReadConsoleInputAsync()
        {
            return Task.Run(() =>
            {
                while (Console.KeyAvailable) { Console.ReadKey(true); } // clears input buffer
                string input = Console.ReadLine();
                if (input == "/cancel") { Console.WriteLine("Cancelled event creation"); }
                return input;
            });
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}