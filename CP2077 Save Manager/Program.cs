/* 

Copyright © 2025 Samuel Davenport

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

*/

using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.CodeDom;

namespace saveManager
{
    partial class Program
    {
        static string savesDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077"; //CP2077 save file location in Windows 11, need to check and see if it's different for Win10

        [STAThread]
        static void Main()
        {
            List<string> savesDirList;
            List<Save> allSaves;
            //string launcherDir = String.Empty;
            int saveNum;
            //bool validPath = false;
            //SaveManagerConfig config = new SaveManagerConfig(null, 0);

            Console.WriteLine("Welcome to the CP2077 Save Manager!!!\n");

            //bool hasConfig = false;

            //check for previously saved launcher path
            /* if (File.Exists($"{savesDir}\\launcher_directory.txt"))
            {
                string str = File.ReadAllText($"{savesDir}\\launcher_directory.txt");
                Console.WriteLine(str);

                if (File.Exists($"{str}\\REDprelauncher.exe"))
                {
                    launcherDir = str;
                    validPath = true;
                    //hasConfig = true;
                }
                else
                {
                    File.Delete($"{savesDir}\\launcher_directory.json");
                    Console.WriteLine("The saved path was invalid.");
                }
            }

            //If no launcher path detected, prompt user for CP2077 installation directory, check to make sure launcher exists
            while (!validPath)
            {
                FolderBrowserDialog dirBrowser = new FolderBrowserDialog();
                dirBrowser.Description = "Select the install folder for Cyberpunk 2077";
                dirBrowser.RootFolder = Environment.SpecialFolder.ProgramFiles;
                dirBrowser.OkRequiresInteraction = true;

                DialogResult result = dirBrowser.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (File.Exists($"{dirBrowser.SelectedPath}\\REDprelauncher.exe"))
                    {
                        launcherDir = dirBrowser.SelectedPath;
                        using (StreamWriter sw = File.CreateText($"{savesDir}\\launcher_directory.txt")) { sw.Write(launcherDir); }
                        validPath = true;
                    }
                    else { Console.WriteLine($"Launcher not found in {dirBrowser.SelectedPath}, please choose a different folder.\n"); }
                }
                else { Console.WriteLine("No folder selected, please try again.\n"); }
            }
            
            Process launcher = new Process() { StartInfo = new ProcessStartInfo { FileName = $"{launcherDir}\\REDprelauncher.exe" } };

            launcher.Exited += (sender, e) => {System.Console.WriteLine("Process has exited.");};
 */
            //Check to see if there is a playthrough already loaded, and ask user how to proceed
            if (Directory.Exists($"{savesDir}\\Inactive"))
            {
                Console.WriteLine("\nPreviously loaded playthrough detected!!\n\nDetermining current active playthroughs...\n");

                savesDirList = new List<string>(Directory.EnumerateDirectories(savesDir)); //Scan main folder
                savesDirList.Remove($"{savesDir}\\Inactive"); //remove 

                if (savesDirList.Count() > 0)
                {
                    allSaves = ScanSaves(savesDirList);

                    savesDirList = new List<string>(Directory.EnumerateDirectories(savesDir, "", SearchOption.AllDirectories)); //Scan for all saves, including Inactive ones

                    Save[] lastLoadedArr = allSaves.Distinct(new SaveComparer()).ToArray();
                    saveNum = RenameSaves(allSaves, GetCurrentSaveNum(savesDirList));

                    //deserialize save_manager_data.txt 
                    //string fileString = File.ReadAllText($"{savesDir}\\save_manager_data.json");
                    //JsonNode node = JsonNode.Parse(fileString);

                    //string l = ((LifePath)array[0]["BodyGender"].GetValue<int>()).ToString();

                    //display last loaded playthroughs and prompt user to proceed as is or switch playthrough
                    Console.WriteLine("Currently loaded playthroughs:");
                    foreach (Save save in lastLoadedArr)
                    {
                        Console.WriteLine(save.ToString());

                        //((LifePath)array[i]["LifePath"].GetValue<int>()).ToString() + " (" + ((BodyGender)array[i]["BodyGender"].GetValue<int>()).ToString() + " Body + "
                        //+ ((VoiceGender)array[i]["VoiceGender"].GetValue<int>()).ToString() + " Voice), " + array[i]["PlayThruId"]);
                    }

                    Console.WriteLine("\nDo you want to:\n1. Close Program.\n2. Choose different playthroughs to load.");
                    switch (Convert.ToInt32(Console.ReadLine()))
                    {
                        case 1:
                            // Console.WriteLine("Complete!!\nLaunching Cyberpunk 2077...");
                            // launcher.Start();
                            // launcher.WaitForExit(30000);
                            return;
                        case 2:
                            MoveSaves(new List<string>(Directory.EnumerateDirectories($"{savesDir}\\Inactive")), savesDir);
                            Directory.Delete($"{savesDir}\\Inactive");
                            break;
                        default:
                            // Console.WriteLine("Complete!!\nLaunching Cyberpunk 2077...");
                            // launcher.Start();
                            // launcher.WaitForExit(30000);
                            return;
                    }
                }

            }
            else { Console.WriteLine("No previously loaded playthroughs detected.\n"); }

            savesDirList = new List<string>(Directory.EnumerateDirectories(savesDir)); //build list of paths to all individual save directories
            //Thread.Sleep(2000);

            Console.Write("Building save file list...");

            allSaves = ScanSaves(savesDirList);
            saveNum = RenameSaves(allSaves, GetCurrentSaveNum(savesDirList));

            Save[] playThruListArr = allSaves.Distinct(new SaveComparer()).ToArray(); //used to populate playthrough selection list
            //Thread.Sleep(2000);

            //display list of available playthroughs
            Console.WriteLine(" Complete!!\n\nPlease select a playthrough/s to load (seperate multiple choices with a comma):\n");
            foreach (var (index, item) in playThruListArr.Select((item, index) => (index, item)))
            {
                Console.WriteLine($"{index + 1}. {item.ToString()}, {allSaves.FindAll(x => new SaveComparer().Equals(x, item)).Count} Saves");
            }

            string[] userEntry = Console.ReadLine().Split(",");

            Save[] selection = new Save[userEntry.Length];

            foreach (var (index, item) in userEntry.Select((item, index) => (index, item))) { selection[index] = playThruListArr[Int32.Parse(item) - 1]; }

            foreach (Save save in selection) { allSaves.RemoveAll(x => new SaveComparer().Equals(x, save)); } //list of all saves to be removed from main save directory

            MoveSaves(allSaves, $"{savesDir}\\Inactive");

            // var opts = new JsonSerializerOptions { WriteIndented = true };
            // string selectionStr = JsonSerializer.Serialize(config,opts); //JSON object containing relevant config data to be written to save_manager_data.json

            // //Console.WriteLine(selectionStr);

            // if (File.Exists($"{savesDir}\\save_manager_data.json")) //if file exists, delete it and remake it with new values
            // {
            //     File.Delete($"{savesDir}\\save_manager_data.json");
            //     using (StreamWriter sw = File.CreateText($"{savesDir}\\save_manager_data.json")) { sw.WriteLine(selectionStr); }
            // }
            // else //make file with values
            // {
            //     using (StreamWriter sw = File.CreateText($"{savesDir}\\save_manager_data.json")) { sw.WriteLine(selectionStr); }
            // }

            //Console.Write("Complete!!\nLaunching Cyberpunk 2077...");

            //launcher.Start();
            //launcher.WaitForExit(60000);
        }

        public class SaveComparer : IEqualityComparer<Save>
        {
            public bool Equals(Save s1, Save s2)
            {
                if (ReferenceEquals(s1, s2)) { return true; }

                else if (s1 == null || s2 == null) { return false; }

                return s1.PlayThruId == s2.PlayThruId
                && s1.LifePath == s2.LifePath
                && s1.BodyGender == s2.BodyGender
                && s1.VoiceGender == s2.VoiceGender;
            }

            public int GetHashCode(Save obj)
            {
                string s = $"{obj.PlayThruId} + {obj.LifePath} + {obj.BodyGender} + {obj.VoiceGender}";
                return s.GetHashCode();
            }
        }

        public static List<Save> ScanSaves(List<string> dirList)
        {
            List<Save> savesList = new List<Save>();

            foreach (var (index, item) in dirList.Select((item, index) => (index, item))) //deserialize each save file's metadata and use to build array of all saves
            {
                try
                {
                    string fileName = item + "\\metadata.9.json"; //Let's hope 2.3 doesn't mess with this...
                    string jsonString = File.ReadAllText(fileName);
                    JsonDocument document = JsonDocument.Parse(jsonString);
                    JsonElement root = document.RootElement.GetProperty("Data").GetProperty("metadata");

                    savesList.Add(new Save(
                                            item,
                                            root.GetProperty("playthroughID").GetString(),
                                            (LifePath)Enum.Parse(typeof(LifePath), root.GetProperty("lifePath").GetString()),
                                            (BodyGender)Enum.Parse(typeof(BodyGender), root.GetProperty("bodyGender").GetString()),
                                            (VoiceGender)Enum.Parse(typeof(VoiceGender), root.GetProperty("brainGender").GetString())
                                        ));
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e.ToString());
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e.ToString());
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine(e.ToString());
                }

            }

            return savesList;
        }

        public static int RenameSaves(List<Save> saves, int num)
        {
            foreach (Save save in saves)
            {
                if (!(MyRegex().IsMatch(save.SaveDir.Split("\\").Last())))
                {
                    string newDir = $"{savesDir}\\Save{num} - {save.ToString()}";
                    Console.WriteLine(newDir);
                    Directory.Move(save.SaveDir, $"{newDir}");
                    save.SaveDir = newDir;
                    num++;
                }
            }

            return num;
        }

        public static int GetCurrentSaveNum(List<string> dirArr)
        {
            int num = 0;

            foreach (string dir in dirArr)
            {
                string str = dir.Split("\\").Last();
                if (MyRegex().IsMatch(str))
                {
                    int i = Int32.Parse(MyRegex().Match(str).Groups[1].Value);
                    if (num < i) { num = i; }
                }

            }

            return num+1;
        }

        public static void MoveSaves(List<string> saves, string dest)
        {

            foreach (string save in saves)
            {
                string dir = save.Split("\\").Last(); //grabbing the name of the individual directory, could maybe get from saveName, see note in class file.

                try
                {
                    // Ensure the source directory exists
                    if (Directory.Exists(save) == true)
                    {
                        // Ensure the destination directory exists
                        if (Directory.Exists(dest) == true)
                        {
                            // Perform the move
                            Directory.Move(save, $"{dest}\\{dir}");
                        }
                        else
                        {
                            Directory.CreateDirectory($"{dest}");
                            Directory.Move(save, $"{dest}\\{dir}");
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                catch (Exception exc)
                {
                    Console.WriteLine(exc.ToString());
                    return;
                }
            }

            Console.WriteLine("Saves moved successfully!!\n");
        }

        public static void MoveSaves(List<Save> saves, string dest) {
            foreach (Save save in saves)
            {
                string dir = save.SaveDir.Split("\\").Last();

                try
                {
                    // Ensure the source directory exists
                    if (Directory.Exists(save.SaveDir) == true)
                    {
                        // Ensure the destination directory exists
                        if (Directory.Exists(dest) == true)
                        {
                            // Perform the move
                            Directory.Move(save.SaveDir, $"{dest}\\{dir}");
                        }
                        else
                        {
                            Directory.CreateDirectory($"{dest}");
                            Directory.Move(save.SaveDir, $"{dest}\\{dir}");
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                catch (Exception exc)
                {
                    Console.WriteLine(exc.ToString());
                    return;
                }
            }

            Console.WriteLine("Saves moved successfully!!\n");
        }

        [GeneratedRegex("Save(\\d{1,3}) - \\w{5,6} \\(\\w{3,4}?Body,\\w{1,10}Voice\\),\\w{16}")]
        private static partial Regex MyRegex();
    }

}