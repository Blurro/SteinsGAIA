using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            List<KeyValuePair<string, int>> eventsSorting = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> eventCausesSorting = new List<KeyValuePair<string, int>>();
            for (int e = 0; e < GloDat.Events.Count; e++)
            {
                ParseEvent(GloDat.Events[e]);
                int j = GloDat.DateOrd.IndexOf(GloDat.evDate);


                eventsSorting.Add(new KeyValuePair<string, int>(GloDat.Events[e], j));
                eventCausesSorting.Add(new KeyValuePair<string, int>(GloDat.EventCauses[e], j));
            }
            eventsSorting = eventsSorting
                .OrderBy(pair => pair.Value)
                .ThenBy(pair => GloDat.Events.IndexOf(pair.Key))
                .ToList();
            eventCausesSorting = eventCausesSorting
                .OrderBy(pair => pair.Value)
                .ThenBy(pair => GloDat.EventCauses.IndexOf(pair.Key))
                .ToList();
            Console.WriteLine("waawawaaaaaa");
            foreach (var pair in eventsSorting)
            {
                Console.WriteLine(pair);
            }
            foreach (var pair in eventCausesSorting)
            {
                Console.WriteLine(pair);
            }
            GloDat.EventsSortedByDate = eventsSorting.Select(pair => pair.Key).ToList();
            GloDat.EventCausesSortedByDate = eventCausesSorting.Select(pair => pair.Key).ToList();
        }

        private void UpdWL_Click(object sender, EventArgs e)
        {
            bool doGen = true;
            List<string> CurrentWL = new List<string>();
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
                // WORLDLINE GEN STARTS HERE \/\/\/
                bool logo = false;
                //PrintConsole(logo);
                string wlStart = GloDat.WLBegin[0];

                bool endGen = false;
                while (!endGen)
                {
                    wlCounter++;
                    Console.WriteLine("WL" + wlCounter + ":");
                    if (wlCounter > 1 & GloDat.evType == "bttdep")
                    {
                        int j = GloDat.DateOrd.IndexOf(GloDat.evDate);
                        foreach (string ev in CurrentWL)
                        {
                            ParseEvent(ev);
                            if (GloDat.DateOrd.IndexOf(GloDat.evDate) > j)
                            {
                                CurrentWL.Remove(ev);
                            }
                        }
                        foreach (string ev in CurrentWL)
                        {
                            Console.WriteLine(ParseEvent(ev));
                        }
                    }
                    else
                    {
                        CurrentWL.Clear();
                    }
                    endGen = true;
                    CurrentWL.Add(wlStart);
                    Console.WriteLine(ParseEvent(wlStart));
                    for (int i = GloDat.EventsSortedByDate.IndexOf(wlStart); i < GloDat.EventsSortedByDate.Count; i++)
                    {
                        bool addEv = AddEventOrNot(i, CurrentWL);
                        if (addEv)
                        {
                            CurrentWL.Add(GloDat.EventsSortedByDate[i]);
                            Console.WriteLine(ParseEvent(GloDat.EventsSortedByDate[i]) + " wafa");
                        }
                        if (GloDat.evType == "bttdep")
                        {
                            // if its arrival exists on wl then skip past departure
                            if (CurrentWL.IndexOf(GloDat.Events[GloDat.Events.IndexOf(GloDat.EventsSortedByDate[i]) + 1]) == -1)
                            {
                                string nextStart = GloDat.Events[GloDat.Events.IndexOf(GloDat.EventsSortedByDate[i]) + 1];
                                ParseEvent(nextStart);
                                wlStart = nextStart;
                                endGen = false;
                                Console.WriteLine();
                                break;
                            }
                        }
                        //GloDat.EventsSortedByDate[i]
                    }
                    if (wlCounter == 200)
                    {
                        endGen = true;
                    }
                }
            }
        }

        private bool AddEventOrNot(int evNum, List<string> WLBuild)
        {
            bool addIt = false;

            sortCauses(GloDat.EventCausesSortedByDate[evNum]);
            foreach (int p in GloDat.evPreventatives)
            {
                if (WLBuild.IndexOf(GloDat.Events[p - 1]) != -1)
                {
                    return addIt;
                }
            }
            if (GloDat.evCauses.Count > 0)
            {
                foreach (int c in GloDat.evCauses)
                {
                    if (WLBuild.IndexOf(GloDat.Events[c - 1]) != -1)
                    {
                        addIt = true;
                    }
                }
            } else if (GloDat.WLBegin.IndexOf(GloDat.Events[evNum]) != -1)
            {
                addIt = true;
            }
            foreach (string wat in GloDat.WLBegin) { Console.WriteLine(wat); }
            Console.WriteLine("wgyubhr " + GloDat.Events[evNum] + "  :  " + GloDat.EventCauses[evNum]);

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
                Console.WriteLine(ParseEvent(a));
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
    }
}