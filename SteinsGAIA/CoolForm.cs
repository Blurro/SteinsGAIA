using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
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
                return;
            }
            string buildDate = CheckDateFormat(newDate.Split(':'));
            if (buildDate == "") // if invalid date format (still uses date but prompts manual adding)
            {
                buildDate = GloDat.random;//takes it saved from the 'random' var since i had to return blank to enter this IF
                Console.WriteLine($"Date '{buildDate}' unable to be placed automatically\nPlease enter a #num of the DATES list to insert this");
                for (int i = 0; i < 1; i+=0)
                {
                    if ((GloDat.random = await AskUser("", "", "# /><~()\\*")) == "/cancel") return;
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
                    Console.WriteLine("Not a valid position, retry or /cancel");
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
                for (int i = 0; i < 1; i++)
                {
                    panel1.Visible = true;
                    bool logo = false;
                    PrintConsole(logo);
                    string input;
                    Console.WriteLine("For any input, type '/cancel' to escape without modifying the config.\n");
                    Console.WriteLine("Add normal or TT departure event? Enter 'n' or 'tt'.");
                    if ((input = await AskUser("n", "tt", "#/><~()\\*")) == "/cancel") break;
                    string eventBuild;
                    if (input == "n")
                    {
                        Console.WriteLine("Label your event (e.g. 'Okabe walks to the shop')");
                        if ((eventBuild = await AskUser("", "", "#/><~()\\*")) == "/cancel") break;
                        Console.WriteLine("Enter the date for this event (format 'year:month:day:hour:minute', minimum of 'year' required)\nExamples: 'x' '2010:08:30' '1975' '-500:01:12:23:59' '-17million'");
                        if ((input = await AskUser("", "", "# /><~()\\*")) == "/cancel") break;
                        await InsertDateToList(GloDat.DateOrd, input); if (GloDat.random == "/cancel") { GloDat.random = ""; break; }
                        string correctInput = GloDat.random;
                        LoadAllLists(GloDat.allLists[4], 4);//updates dates listbox
                        eventBuild = $"{eventBuild}#{correctInput}";
                        GloDat.Events.Add(eventBuild);
                        LoadAllLists(GloDat.allLists[0], 0);//updates events listbox
                        GloDat.EventCauses.Add("");
                        LoadAllLists(GloDat.allLists[1], 1);//updates eventcauses listbox
                        Console.WriteLine($"Created event '{ParseEvent(eventBuild)}' ({eventBuild})");
                    }
                    else
                    {
                        Console.WriteLine("Label your departure (e.g. 'Suzuha', 'Faris dmail')");
                        if ((eventBuild = await AskUser("", "", "#/><~()\\*")) == "/cancel") break;

                        //scan existing events for departures of the same label, create a list of these but only print a count of them to console ('3 departures of the same entity detected')

                        Console.WriteLine("Enter the date of this departure event (format 'year:month:day:hour:minute', minimum of 'year' required)\nExamples: 'x' '2010:08:30' '1975' '-500:01:12:23:59' '-17million'");
                        if ((input = await AskUser("", "", "# /><~()\\*")) == "/cancel") break;
                        string eventArrival = $">{eventBuild}#";
                        bool btt = true;
                        string arriveDate = "";
                        List<string> cloneGloDates = GloDat.DateOrd.ToList();
                        await InsertDateToList(cloneGloDates, input); if (GloDat.random == "/cancel") { GloDat.random = ""; break; }
                        string correctInput = GloDat.random;
                        LoadAllLists(cloneGloDates, 4);//temporarily updates dates listbox with clone
                        Console.WriteLine("Enter the targeted date of arrival (BTT-arrival dates must be unique between unrelated travel entities)");
                        for (int a = 0; a < 1; a += 0)
                        {
                            List<string> backupClone = cloneGloDates.ToList();
                            if ((GloDat.random = await AskUser("", "", "# /><~()\\*")) == "/cancel") break;
                            await InsertDateToList(cloneGloDates, GloDat.random); if (GloDat.random == "/cancel") { break; }
                            arriveDate = GloDat.random;
                            Console.WriteLine($"uhh {arriveDate}");
                            btt = cloneGloDates.FindIndex(ee => ee == arriveDate) < cloneGloDates.FindIndex(ee => ee == correctInput);
                            if (correctInput == arriveDate)
                            {
                                //not invalid if existing same entity uses same btt date

                                Console.WriteLine("Invalid arrival date (same as departure)");
                                cloneGloDates = backupClone.ToList();
                                continue;
                            }
                            btt = cloneGloDates.FindIndex(ee => ee == arriveDate) < cloneGloDates.FindIndex(ee => ee == correctInput);
                            if (btt)
                            {
                                if (GloDat.BTTDates.Contains(arriveDate))
                                {
                                    Console.WriteLine("Invalid arrival date (BTT using an existing BTT arrival date)");
                                    cloneGloDates = backupClone.ToList();
                                    continue;
                                }
                            }
                            break;
                        }
                        if (GloDat.random == "/cancel") { GloDat.random = ""; break; }
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
                        GloDat.Events.Add(eventBuild);
                        GloDat.Events.Add(eventArrival);
                        LoadAllLists(GloDat.allLists[0], 0);//updates events listbox

                        GloDat.EventCauses.Add("");
                        GloDat.EventCauses.Add("/");
                        LoadAllLists(GloDat.allLists[1], 1);//updates eventcauses listbox
                        Console.WriteLine($"Created departure event '{ParseEvent(eventBuild)}' ({eventBuild})\n+created its paired arrival event");
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

                    if (input == "/cancel")
                    {
                        Console.WriteLine("Cancelled action");
                        return "/cancel";
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
                            Console.WriteLine($"\u001b[A\u001b[30m{input}\u001b[37m\u001b[A"); //blacks out input and returns to the line
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
    }
}