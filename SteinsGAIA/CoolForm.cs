using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
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
            listBoxes[0] = listBox1; //events
            listBoxes[1] = listBox2; //causes
            listBoxes[2] = listBox3; //entities
            listBoxes[3] = listBox4; //af
            listBoxes[4] = listBox5; //dates
            listBoxes[5] = listBox6; //rs
            listBoxes[6] = listBox7; //starter
            listBoxes[7] = listBox8; //bttdates
            int i = 0;
            listBoxes[lisNum].Items.Clear();

            foreach (string s in lisData)
            {
                i++;
                //if (new[] { 3, 4, 5 }.Contains(lisNum)) {listBoxes[lisNum].Items.Add(s);} else 
                if (lisNum == 6)
                {
                    listBoxes[lisNum].Items.Add("[" + (GloDat.Events.IndexOf(s) + 1) + "] " + ParseEvent(s, false, true));
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
            GloDat.EventsSortedByDate.Clear(); // cant believe i forgot to add this for so long, was causing issues without lol
            GloDat.EventCausesSortedByDate.Clear();
            for (int d = 0; d < GloDat.DateOrd.Count; d++)
            {
                for (int e = 0; e < GloDat.Events.Count; e++)
                {
                    ParseEvent(GloDat.Events[e], false, false);

                    if (GloDat.evDate != GloDat.DateOrd[d] || GloDat.EventsSortedByDate.Contains(GloDat.Events[e]))
                    {
                        continue;
                    }

                    GloDat.EventsSortedByDate.Add(GloDat.Events[e]);
                    GloDat.EventCausesSortedByDate.Add(GloDat.EventCauses[e]);
                }
            }
            //Console.WriteLine(string.Join(" | ", GloDat.EventsSortedByDate));
            //Console.WriteLine(string.Join(" | ", GloDat.DateOrd));
            //Console.WriteLine("sorted");
            //Console.ReadKey();
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
            if (panel1.Visible == true) { return; }
            Console.Clear(); Console.WriteLine("\x1b[3J");
            GloDat.randomValue = GloDat.randomVal.Next(0, 999);
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
                    ParseEvent(i, false, false);
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
                    listBox7.Items.Add("[" + (GloDat.Events.IndexOf(pair.Key) + 1) + "] " + ParseEvent(pair.Key, false, false));
                }
            }
            UpdText(result);

            int wlCounter = 0;
            if (doGen)
            {
                DateSortEvents();

                bool logo = false;
                PrintConsole(logo);
                Console.WriteLine($"\u001b[38;2;30;30;40m\u001b[60C\u001b[2AColor seed {GloDat.randomValue}\u001b[37m\n");
                string wlStart = GloDat.WLBegin[0];
                List<string> AllWorldLines = new List<string>();
                List<int> BTTActiveInfluences = new List<int>();

                // WORLDLINE GEN STARTS HERE \/\/\/

                bool endGen = false;
                while (!endGen) // this will loop, one loop = one worldline, all rules are contained within this 'while'
                {
                    endGen = true; // by default the generation of new WLs will end after this one and the future will be allowed to continue, until any type of WL ending event says otherwise.
                    wlCounter++; // before this line, wlCounter is 0, we begin at 1 for WL1
                    AllWorldLines.Add("-"); // every 'WL(num):' is == to a '-' in this code version
                    Console.WriteLine($"\x1b[3mWL{wlCounter}:\x1b[0m"); // simply prints "WL1:" increasing the number for every new worldline

                    // formatting - each rule will be one chunk of code, no empty lines between them, and separated by these // RULE comments

                    // RULE ONE: a BTT arriving in the past will arrive before any events that have a higher date.

                    if (wlCounter > 1 && GloDat.evType == "bttarv") // if this is the 2nd+ wl, and last one ended via btt, then this deletes all events happening after the btt arival date, preserving all with a date prior or equal
                    {
                        int j = GloDat.DateOrd.IndexOf(GloDat.evDate);
                        //Console.WriteLine(string.Join(", ", CurrentWorldLine));
                        for (int i = CurrentWorldLine.Count - 1; i >= 0; i--)
                        {
                            string ev = CurrentWorldLine[i];
                            ParseEvent(ev, false, false);
                            if (GloDat.DateOrd.IndexOf(GloDat.evDate) > j || (GloDat.DateOrd.IndexOf(GloDat.evDate) == j && GloDat.evType == "bttarv")) // deletes events of dates higher than the arriving btt that started this wl, and deletes btt arrivals sharing the same btt arrival date
                            {
                                CurrentWorldLine.Remove(ev); // remove all events happening after the arriving BTT date. they may return based on causes in later rules.
                            }
                        }
                        for (int i = 0; i < CurrentWorldLine.Count; i++)
                        {
                            string parsed = ParseEvent(CurrentWorldLine[i], true, false);
                            if (GloDat.evType == "bttdep")
                            {
                                Console.WriteLine(parsed + " \u001b[38;2;30;30;40m(in holding)\u001b[37m"); // parsing an event will also return a readable sentence version rather than code
                            } else if (GloDat.evType == "bttarv") {
                                Console.WriteLine($"{parsed} {GloDat.evSuffix.Replace("<", "-")}");
                            }
                            else
                            {
                                Console.WriteLine(parsed);
                            }
                        }
                        CurrentWorldLine.Add(wlStart); // adding the btt arrival event now
                        Console.WriteLine($"{ParseEvent(wlStart, true, false)} {GloDat.evSuffix}"); // printing translated readable ver to console
                    }
                    else
                    {
                        CurrentWorldLine.Clear(); // if we got here via convergence breaking instead (no btt, just disobey prev AF) then we will build the WL via other rules.
                    }

                    // RULE TWO: cycle through the list of events in order of date, if their causes exist on the current line, allow its addition
                    
                    for (int i = CustomIndexOf(GloDat.EventsSortedByDate, wlStart); i < GloDat.EventsSortedByDate.Count; i++) // 1 loop = 1 event checked. starting from events with the same date as the event in var 'wlStart', likely the BTT arrival event.
                    {
                        //Console.WriteLine($"{GloDat.evLabel} !! {wlStart}, {CustomIndexOf(GloDat.EventsSortedByDate, wlStart)}");
                        //LoadAllLists(GloDat.allLists[4], 4);//updates dates listbox

                        if (CustomIndexOf(CurrentWorldLine, GloDat.EventsSortedByDate[i]) != -1) { continue; } // if the event we are checking already exists on the wl, skip to next event
                        bool addEv = AddEventOrNot(i, CurrentWorldLine); // goes to a function checking if the current worldline contains causes or preventatives for the event in question, returns true (=it should be added) or false
                        string parsed = ParseEvent(GloDat.EventsSortedByDate[i], true, false);
                        if (addEv && GloDat.evType != "bttdep")
                        {
                            CurrentWorldLine.Add(GloDat.EventsSortedByDate[i]); Console.WriteLine(parsed);
                        }

                        // RULE THREE: if the event being added is a BTT departure then this spells THE END of the current worldline, but ONLY IF [its identical arrival exists on the current worldline] OR [its arrival + all current events prior to its arrival do NOT identically match the beginning timeline of ANY prior active worldline, only scanning backwards until the starting worldline of the current AF that was NOT reached via BTT]
                        // tl;dr worldline ends, else no change no shift

                        if (addEv && GloDat.evType == "bttdep")
                        {
                            BTTActiveInfluences.Clear();
                            //if the BTT has a travel entity (meaning it changes constantly along a worldline) and has no set influences, by default everything is an influence to it
                            if (GloDat.TTEntities.Contains(GloDat.evLabel)) // reminder to add a check for its influences too - default rn is that itll act as if everything prior is an influence if theres an entity
                            {
                                foreach (string ev in CurrentWorldLine)
                                {
                                    BTTActiveInfluences.Add(CustomIndexOf(GloDat.Events, ev) + 1);
                                }
                                BTTActiveInfluences.Sort();
                            }
                            string BTTInfluenceAddon = string.Join("", BTTActiveInfluences.ConvertAll(item => $"|{item}"));
                            if (BTTInfluenceAddon != "")
                            {
                                BTTInfluenceAddon += "|";
                            }

                            int departResult = 0;
                            ParseEvent(GloDat.EventsSortedByDate[i], false, false);
                            string getEv = $">{GloDat.evLabel}#{GloDat.evBttDate}\\";
                            //foreach (string aa in GloDat.Events) { if (aa.Split('*')[0] == getEv) { getEv = aa; } }   // if theres something in between the bttarrivaldate and influences this would fix but dont think i'll need it

                            CurrentWorldLine.Add(GloDat.EventsSortedByDate[i]);//add it because we need it on the worldline for next part & itll get added either way (console msg add is later)
                            if (CurrentWorldLine.IndexOf(getEv + BTTInfluenceAddon) == -1) //if arrival (with same influences therefore identical entity) DOES NOT exist on wl, check 2nd condition before deciding to end WL
                            {
                                //this is after the 'OR' in rule 3 description, we search prior worldlines now (ignoring the AF thing cus dont have em yet)
                                List<string> currentUpToNextArrive = new List<string> { "-" };
                                //Console.WriteLine(GloDat.EventsSortedByDate[i] + " ????");
                                int dateReadTo = GloDat.DateOrd.IndexOf(GloDat.BTTDates[int.Parse(GloDat.evBttDate) - 1]);
                                for (int ja = 0; ja < CurrentWorldLine.Count; ja++)
                                {
                                    ParseEvent(CurrentWorldLine[ja], false, false);
                                    //currentUpToNextArrive.Add(CurrentWorldLine[ja]);
                                    //Console.WriteLine($"huh {GloDat.DateOrd.IndexOf(GloDat.evDate)} and {dateReadTo}");
                                    int dateCheck = GloDat.DateOrd.IndexOf(GloDat.evDate);
                                    //Console.WriteLine($"WHAT {dateCheck} < {dateReadTo} w {CurrentWorldLine[ja]}");
                                    if (dateCheck < dateReadTo)
                                    {
                                        currentUpToNextArrive.Add(CurrentWorldLine[ja]);
                                    }
                                    else if (dateCheck == dateReadTo)
                                    {
                                        //Console.WriteLine($"HOW {dateCheck} == {dateReadTo} w {CurrentWorldLine[ja]}");
                                        if (CurrentWorldLine[ja] == getEv + BTTInfluenceAddon) // just so we dont add events that happen on the same date as btt arrival
                                        {
                                            currentUpToNextArrive.Add(CurrentWorldLine[ja]);
                                        }
                                    }
                                    else
                                    {
                                        currentUpToNextArrive.Add(getEv + BTTInfluenceAddon); // add it onto the end of this 'before this date' worldline
                                        break;
                                    } // events are in order of date so if we find one >greater we break early
                                }
                                int dashCount = AllWorldLines.Count(item => item == "-") - 2; // start on the previous-to-current worldline
                                bool identicalPreviousWL = false;
                                for (int je = dashCount; je > -1; je--)
                                {
                                    List<string> prevWLSegment = GetWorldlineSegment(AllWorldLines, je, dateReadTo, getEv + BTTInfluenceAddon);
                                    //Console.WriteLine(string.Join(":", currentUpToNextArrive));
                                    //Console.WriteLine($"Worldline{je + 1}: checking {getEv + BTTInfluenceAddon}");
                                    //Console.WriteLine(string.Join(Environment.NewLine, prevWLSegment));
                                    identicalPreviousWL = currentUpToNextArrive.Count <= prevWLSegment.Count && !currentUpToNextArrive.Where((s, ie) => s != prevWLSegment[ie]).Any();
                                    if (identicalPreviousWL)
                                    {
                                        departResult = 2;
                                        Console.WriteLine(parsed + $" \u001b[38;2;30;30;40m(no change no shift - result on WL{je+1})\u001b[37m"); // add btt no change no shift comment (was already added before the if)
                                        if (CustomIndexOf(CurrentWorldLine, getEv) == -1)
                                        {
                                            departResult = 3;
                                            Console.WriteLine("extremely rare so im keeping this msg for now");
                                        }
                                        break;
                                    }
                                }
                                if (!identicalPreviousWL)
                                {
                                    departResult = 0;
                                    ParseEvent(GloDat.EventsSortedByDate[i], false, false); // updates suffix thing
                                    Console.WriteLine($"{parsed} {GloDat.evSuffix}"); // adds btt depart msg (code already added to wl before this if)

                                    ParseEvent(getEv, false, false);

                                    wlStart = getEv + BTTInfluenceAddon;
                                    endGen = false;
                                    Console.WriteLine();
                                    break; // if neither condition for no 'change no shift' is met, exit loop and end the current worldline
                                }
                            }
                            else
                            {
                                departResult = 1;
                                Console.WriteLine(parsed + " \u001b[38;2;30;30;40m(no change no shift)\u001b[37m"); // add btt no change no shift comment (was already added before the if)
                            }

                            // RULE FOUR: any BTT that departs without ending the worldline (AND if its scheduled arrival exists) will be put into 'holding', then if a BTT e.g. 'entity1' departs from the same worldline this holding occurs, and entity1 arrives to a point prior to the held-BTT's arrival (thus removing its clone), the held BTT will now come out of holding to arrive AND be anchored to entity1's btt-arrival date

                            // departResults
                            // 0: departure valid, goes to the past. (wont reach this)
                            // 1: identical arrival exists on the worldline. (goes into holding)
                            // 2: non identical arrival exists, identical result on previous worldline. (goes into holding)
                            // 3: no arrival exists, identical result on previous worldline (rare, only known case is the first dmail in beta, skip it)
                            if (departResult == 1 || departResult == 2)
                            {

                            }
                        }
                    }
                    foreach (string ev in CurrentWorldLine) { AllWorldLines.Add(ev); } //add the fully created current WL to our list of all of them
                    if (wlCounter == 16) // just in case i goof up and make an infinite loop ive put a cap here
                    {
                        endGen = true;
                        Console.WriteLine("reached generation cap"); continue;
                    }
                    if (endGen)
                    {
                        Console.WriteLine("future continues... Add more events!");
                        //foreach (string ev in AllWorldLines) { Console.WriteLine($"{ev}"); }
                    }
                }
            }
        }

        private bool AddEventOrNot(int evNum, List<string> WLBuild)
        {
            List<string> WLBuild2 = WLBuild.ToList(); // changing wlbuild here changes the original list too lol this caused me a confusion
            for (int i = 0; i < WLBuild2.Count; i++)
            {
                WLBuild2[i] = WLBuild2[i].Split('|')[0]; // removes the influences from being appended
            }
            bool addIt = false;
            sortCauses(GloDat.EventCausesSortedByDate[evNum]); // loads the event's causes (+num+num) and preventatives (-num-num) into lists
            foreach (int p in GloDat.evPreventatives)
            {
                if (WLBuild2.IndexOf(GloDat.Events[p - 1]) != -1) // if the preventative in question is present on the worldline, the event has been ruled out from happening, return variable 'addIt' while it is set to 'false'
                {
                    //Console.WriteLine("prevented " + GloDat.EventsSortedByDate[evNum]);
                    return addIt;
                }
            }
            if (GloDat.evCauses.Count > 0)
            {
                foreach (int c in GloDat.evCauses)
                {
                    if (WLBuild2.IndexOf(GloDat.Events[c - 1]) != -1) // if the cause event in question is present on the worldline, allow the event to be added
                    {
                        //Console.WriteLine("true 1");
                        addIt = true;
                    }
                }
            } else if (GloDat.WLBegin.IndexOf(GloDat.EventsSortedByDate[evNum]) != -1 && !GloDat.EventCausesSortedByDate[evNum].Contains("/")) // if the event in question has no causes, but was added to the 'starter worldline', assume it happens by default and so we return 'true'
            {
                //Console.WriteLine("true 2");
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
            if (panel1.Visible == false)
            {
                Console.Clear(); Console.WriteLine("\x1b[3J");
                for (int i = 0; i < 1; i++)
                {
                    panel1.Visible = true;
                    bool logo = false;
                    PrintConsole(logo);

                    Console.WriteLine("These are all of the events you have created! The list in the gui shows them as codes, below is the translated list:\n");

                    int n = 0;
                    foreach (string ev in GloDat.Events)
                    {
                        n++;
                        Console.WriteLine($"{n}) {ParseEvent(ev, true, true)}");
                    }
                }
                panel1.Visible = false;
            }
        }

        private void button3_Click(object sender, EventArgs e) // save button
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

        private static string CheckDateFormat(string[] parts)
        {
            string buildDate;
            int year = -1;
            int month;
            int day;
            int hour;
            int minute;
            if (int.TryParse(parts[0], out var value))
            {
                year = value;
                month = (parts.Length > 1 && int.TryParse(parts[1], out var m)) ? ((m < 13) ? m : -1) : -1;
                day = (parts.Length > 2 && int.TryParse(parts[2], out var d)) ? ((d < 32) ? d : -1) : -1; // in theory feb 31st can be entered but it doesnt rly matter the user can do that if they want ig
                hour = (parts.Length > 3 && int.TryParse(parts[3], out var h)) ? ((h < 24) ? h : -1) : -1;
                minute = (parts.Length > 4 && int.TryParse(parts[4], out var min)) ? ((min < 60) ? min : -1) : -1;
                buildDate = $"{year}{(month < 1 ? "" : $":{month}{(day < 1 ? "" : $":{day}{(hour < 0 ? "" : $":{hour}{(minute < 0 ? "" : $":{minute}")}")}")}")}";
            }
            else
            {
                buildDate = parts[0];
            }
            if (year == -1)
            {
                GloDat.random = buildDate;
                return ""; //we still use buildDate but the code after calling this needs to know its not a valid format
            } else
            {
                return buildDate;
            }
        }

        private async Task InsertDateToList(List<string> dateOrd, string newDate)
        {
            if (dateOrd.Contains(newDate)) // if input date is 'x' and it exists, dont readd
            {
                GloDat.random = newDate;
                return;
            }
            string buildDate = CheckDateFormat(newDate.Split(':'));
            if (buildDate == "") // if invalid date format (still uses date but prompts manual adding)
            {
                buildDate = GloDat.random;//takes it saved from the 'random' var since i had to return blank to enter this IF
                Console.WriteLine($"Date '{buildDate}' unable to be placed automatically\nPlease enter a #num of the DATES list to insert this");
                for (int i = 0; i < 1; i+=0)
                {
                    if ((GloDat.random = await AskUser("", "", GloDat.removeCharsSpace)) == GloDat.cancelCode) return;
                    if (int.TryParse(GloDat.random, out var pos))
                    {
                        if (pos > 0)
                        {
                            if (pos > dateOrd.Count) { pos = dateOrd.Count+1; }
                            dateOrd.Insert(pos-1, buildDate);
                            Console.WriteLine($"Date inserted at position {pos}");
                            GloDat.random = buildDate;
                            break;
                        }
                    }
                    Console.WriteLine($"Not a valid position, retry or {GloDat.cancelCode}");
                }
            } else
            {
                if (dateOrd.Contains(buildDate)) // if input date after formatting exists, dont read
                {
                    GloDat.random = buildDate;
                    return;
                }
                int pos = InsertEntryDate(dateOrd, buildDate) +1;
                GloDat.random = buildDate;
                Console.WriteLine($"Date auto inserted at position {pos}");
            }
            return;
        }

        static int InsertEntryDate(List<string> dates, string newEntry)
        {
            string[] newParts = newEntry.Split(':');
            for (int i = dates.Count - 1; i >= 0; i--)
            {
                string[] currentParts = dates[i].Split(':');
                string checkDate = CheckDateFormat(currentParts);
                if (checkDate == "")
                {
                    continue;
                }
                for (int j = 0; j < newParts.Length; j++)
                {
                    int newValue = int.Parse(newParts[j]);
                    if (j >= currentParts.Length || newValue > int.Parse(currentParts[j]))
                    {
                        dates.Insert(i + 1, newEntry);
                        return i+1;
                    }
                    if (newValue < int.Parse(currentParts[j]))
                    {
                        break;
                    }
                }
            }
            dates.Insert(0, newEntry); // Insert at the start if it's smaller than all
            return 0;
        }

        private async void AddEv_Click(object sender, EventArgs e)
        {
            if (panel1.Visible == false)
            {
                Console.Clear(); Console.WriteLine("\x1b[3J");
                for (int i = 0; i < 1; i++)
                {
                    panel1.Visible = true;
                    bool logo = false;
                    PrintConsole(logo);
                    string input;
                    Console.WriteLine($"For any input, type '{GloDat.cancelCode}' to escape without modifying the config.\n");
                    Console.WriteLine("Add normal or TT departure event? Enter 'n' or 'tt'.");
                    if ((input = await AskUser("n", "tt", GloDat.removeCharsSpace)) == GloDat.cancelCode) break;
                    string eventBuild;
                    if (input == "n")
                    {
                        Console.WriteLine("Label your event (e.g. 'Okabe walks to the shop')");
                        if ((eventBuild = await AskUser("", "", GloDat.removeChars)) == GloDat.cancelCode) break;
                        Console.WriteLine("Enter the date for this event (format 'year:month:day:hour:minute', minimum of 'year' required)\nExamples: 'x' '2010:08:30' '1975' '-500:01:12:23:59' '-17million'");
                        if ((input = await AskUser("", "", GloDat.removeCharsSpace)) == GloDat.cancelCode) break;

                        await InsertDateToList(GloDat.DateOrd, input); if (GloDat.random == GloDat.cancelCode) { GloDat.random = ""; break; }
                        string correctInput = GloDat.random;

                        LoadAllLists(GloDat.allLists[4], 4);//updates dates listbox
                        eventBuild = $"{eventBuild}#{correctInput}";

                        if (GloDat.Events.Contains(eventBuild))
                        {
                            Console.WriteLine($"The event '{eventBuild}' already exists!");
                            break;
                        }
                        GloDat.Events.Add(eventBuild);
                        LoadAllLists(GloDat.allLists[0], 0);//updates events listbox
                        GloDat.EventCauses.Add("");
                        LoadAllLists(GloDat.allLists[1], 1);//updates eventcauses listbox
                        Console.WriteLine($"Created event '{ParseEvent(eventBuild, true, false)}' ({eventBuild})");
                    }
                    else
                    {
                        Console.WriteLine("Label your departure (e.g. 'Suzuha', 'Faris dmail')");
                        if ((eventBuild = await AskUser("", "", GloDat.removeChars)) == GloDat.cancelCode) break;
                        List<string> matchEntityListArrivals = new List<string>();
                        //scan existing events for departures of the same label, create a list of its used btt arrival dates
                        foreach (string eve in GloDat.Events)
                        {
                            if (eve.Contains('<') && eve.Contains('\\'))//btt departure event
                            {
                                ParseEvent(eve, false, false);
                                if (GloDat.evLabel == eventBuild)
                                {
                                    matchEntityListArrivals.Add(GloDat.BTTDates[int.Parse(eve.Split('\\')[1]) - 1]);
                                }
                            }
                        }
                        if (matchEntityListArrivals.Count > 0)
                        {
                            Console.WriteLine($"Found {matchEntityListArrivals.Count} used BTT arrivals by the entity '{eventBuild}': {string.Join(", ", matchEntityListArrivals.ConvertAll(item => $"'{item}'"))}");
                        }
                        Console.WriteLine("Enter the date of this departure event (format 'year:month:day:hour:minute', minimum of 'year' required)\nExamples: 'x' '2010:08:30' '1975' '-500:01:12:23:59' '-17million'");
                        if ((input = await AskUser("", "", GloDat.removeCharsSpace)) == GloDat.cancelCode) break;
                        string eventArrival = $">{eventBuild}#";
                        bool btt = true;
                        string arriveDate = "";
                        List<string> cloneGloDates = GloDat.DateOrd.ToList();
                        await InsertDateToList(cloneGloDates, input); if (GloDat.random == GloDat.cancelCode) { GloDat.random = ""; break; }
                        string correctInput = GloDat.random;
                        LoadAllLists(cloneGloDates, 4);//temporarily updates dates listbox with clone
                        Console.WriteLine("Enter the targeted date of arrival (BTT-arrival dates must be unique between unrelated travel entities)");
                        for (int a = 0; a < 1; a += 0)
                        {
                            List<string> backupClone = cloneGloDates.ToList();
                            if ((GloDat.random = await AskUser("", "", GloDat.removeCharsSpace)) == GloDat.cancelCode) break;
                            await InsertDateToList(cloneGloDates, GloDat.random); if (GloDat.random == GloDat.cancelCode) { break; }
                            arriveDate = GloDat.random;

                            btt = cloneGloDates.FindIndex(ee => ee == arriveDate) < cloneGloDates.FindIndex(ee => ee == correctInput);
                            if (correctInput == arriveDate)
                            {
                                Console.WriteLine("Invalid arrival date (same as departure)");
                                cloneGloDates = backupClone.ToList();
                                continue;
                            }
                            btt = cloneGloDates.FindIndex(ee => ee == arriveDate) < cloneGloDates.FindIndex(ee => ee == correctInput);
                            if (btt)
                            {
                                if (GloDat.BTTDates.Contains(arriveDate) && !matchEntityListArrivals.Contains(arriveDate)) // if the same entity uses the btt arrival then its allowed (e.g. fail and successful suzuha leaving from different days in 2010 to replace herself in 1975)
                                {
                                    Console.WriteLine("Invalid arrival date (BTT using an existing BTT arrival date)");
                                    cloneGloDates = backupClone.ToList();
                                    continue;
                                }
                            }
                            break;
                        }
                        if (GloDat.random == GloDat.cancelCode) { GloDat.random = ""; break; }
                        if (btt)
                        {
                            if (!GloDat.BTTDates.Contains(arriveDate)) { GloDat.BTTDates.Add(arriveDate); }
                            int indexBTT = GloDat.BTTDates.FindIndex(ee => ee == arriveDate) + 1;
                            eventBuild = $"<{eventBuild}#{correctInput}\\{indexBTT}";
                            eventArrival = $"{eventArrival}{indexBTT}\\";
                            LoadAllLists(GloDat.allLists[7], 7);//updates btt-dates listbox
                        } else
                        {
                            eventBuild = $"<{eventBuild}#{correctInput}/{arriveDate}";
                            eventArrival = $"{eventArrival}{arriveDate}/";
                        }
                        GloDat.DateOrd.Clear();
                        foreach (string d in cloneGloDates)
                        {
                            GloDat.DateOrd.Add(d);
                        }
                        LoadAllLists(GloDat.allLists[4], 4);//updates dates listbox
                        if (GloDat.Events.Contains(eventBuild))
                        {
                            Console.WriteLine($"The event '{eventBuild}' already exists!");
                            break;
                        }
                        GloDat.Events.Add(eventBuild);
                        GloDat.EventCauses.Add("");
                        Console.WriteLine($"Created departure event '{ParseEvent(eventBuild, true, false)}' ({eventBuild})");
                        if (!GloDat.Events.Contains(eventArrival))
                        {
                            Console.WriteLine("+created its paired arrival event");
                            GloDat.Events.Add(eventArrival);
                            GloDat.EventCauses.Add("/");
                        }
                        LoadAllLists(GloDat.allLists[0], 0);//updates events listbox
                        LoadAllLists(GloDat.allLists[1], 1);//updates eventcauses listbox
                    }
                }
                LoadAllLists(GloDat.allLists[4], 4); //updates dates listbox in case of cancel to revert it
                panel1.Visible = false;
            }
        }

        static async Task<string> AskUser(string option1, string option2, string removeChars)
        {
            return await Task.Run(() =>
            {
                for (int i = 0; i < 1; i += 0)
                {
                    while (Console.KeyAvailable) { Console.ReadKey(true); } // Clears input buffer
                    string input = Console.ReadLine();
                    string unmodifInput = input;

                    if (input == GloDat.cancelCode)
                    {
                        Console.WriteLine("Cancelled action");
                        return GloDat.cancelCode;
                    }
                    input = Regex.Replace(input.Trim(), $"[{removeChars}]", "");
                    if (option1 != "")
                    {
                        if (input == option1)
                        {
                            return option1;
                        }
                        else if (input == option2)
                        {
                            return option2;
                        }
                        else
                        {
                            Console.WriteLine($"\u001b[A\u001b[30m{unmodifInput}\u001b[37m\u001b[A"); //blacks out input and returns to the line
                        }
                    } else if (input.Trim() == "")
                    {
                        Console.WriteLine($"\u001b[2A");
                    } else
                    {
                        return input;
                    }
                }
                return string.Empty; //unreachable
            });
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void EditTT_Click(object sender, EventArgs e)
        {

        }

        public static int CustomIndexOf(List<string> list, string searchTerm) // same as list.IndexOf() but gets just the part before the | character
        {
            for (int index = 0; index < list.Count; index++)
            {
                // Split the item and get the part before '|'
                string[] parts = list[index].Split('|');

                if (parts[0] == searchTerm.Split('|')[0])
                {
                    return index; // Return the index if found
                }
            }

            return -1; // Return -1 if not found
        }

        static List<string> GetWorldlineSegment(List<string> bigList, int index, int dateReadTo, string keepBTTArrival)
        {
            List<string> segment = new List<string>();
            int currentIndex = -1; // Track the current segment index

            for (int i = 0; i < bigList.Count; i++)
            {
                if (bigList[i] == "-")
                {
                    currentIndex++;
                    if (currentIndex > index) // If we've gone past the desired segment
                        break;
                    segment.Clear();
                    segment.Add(bigList[i]);
                    continue;
                }
                ParseEvent(bigList[i], false, false);
                if (currentIndex == index && (GloDat.DateOrd.IndexOf(GloDat.evDate) < dateReadTo || bigList[i] == keepBTTArrival)) // dont add events of the same date, will conflict if listed as an earlier dated event
                {
                    segment.Add(bigList[i]);
                }
            }

            return segment;
        }

        private void EditCause_Click(object sender, EventArgs e)
        {
            if (panel1.Visible == false)
            {
                Console.Clear(); Console.WriteLine("\x1b[3J");
                for (int i = 0; i < 1; i++)
                {
                    panel1.Visible = true;
                    bool logo = false;
                    PrintConsole(logo);
                    Console.WriteLine("Here you can choose an event, and set the +cause or -preventative!\nIf an event has no +causes, you may add it to the 'World Defaults' list (type its number in the box)\nand it will happen by default, until prevented.\nBTT departures and AF endpoints cannot have -preventatives, these must be prevented by creating an event that\ndestroys their cause event (nullifying its causal ability for all events happening after it)\n\nCauses: If an event has 2 causes, just one needs to exist to cause it.\nSetting an arrival of the same TT entity as a cause (max 1) will recognise it's continuing the same journey."); //inside this for loop below this line is new, everything else is the template for all buttons

                    int n = 0;
                    foreach (string ev in GloDat.Events)
                    {
                        n++;
                        string gap = $"\u001b[30m{n}  \u001b[37m";
                        Console.WriteLine($"{n}) {ParseEvent(ev, true, true)}");
                        if (GloDat.EventCauses[n - 1] == "/")
                        {
                            Console.WriteLine($"{gap}\u001b[38;2;45;45;60m---\u001b[37m");
                        }
                        else if (GloDat.EventCauses[n - 1] != "")
                        {
                            List<string> causeSplit = Regex.Split(GloDat.EventCauses[n - 1], @"(?<=\d)(?=[+\-|])|(?<=[+\-|])(?=\d)").Where(s => !string.IsNullOrEmpty(s)).ToList();
                            for (int j = 0; j < causeSplit.Count; j++)
                            {
                                if (causeSplit[j] != "+" && causeSplit[j] != "-" && causeSplit[j] != "/")
                                {
                                    ParseEvent(GloDat.Events[int.Parse(causeSplit[j].Split('|')[0]) - 1], false, false);
                                    causeSplit[j] = $"{GloDat.evColor}{causeSplit[j]}\u001b[37m";
                                }
                            }
                            Console.WriteLine($"{gap}{string.Join("", causeSplit)}");
                        } else
                        {
                            Console.WriteLine($"{gap}\u001b[38;2;45;45;60m(none)\u001b[37m");
                        }
                    }
                    Console.WriteLine($"\nEnter the #num of event you'd like to set a cause for! For any input, enter '{GloDat.cancelCode}' to cancel");
                }
                panel1.Visible = false;
            }
        }
    }
}