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

namespace saveManager
{
    class Program
    {
        static string savesDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077"; //CP2077 save file location in Windows 11, need to check and see if it's different for Win10

        [STAThread]
        static void Main()
        {
            List<string> savesDirList;

            Console.WriteLine("Welcome to the CP2077 Save Manager!!!\n");

            string launcherPath = String.Empty;
            bool validPath = false;

            //check if there is a saved launcher path, and if the path contains the launcher.
            if (File.Exists($"{savesDir}\\launcher_path.txt"))
            {
                if (File.Exists(File.ReadAllText($"{savesDir}\\launcher_path.txt") + "\\REDprelauncher.exe"))
                {
                    launcherPath = File.ReadAllText($"{savesDir}\\launcher_path.txt");
                    validPath = true;
                }
            }

            //If no valid launcher path is detected, prompt user for CP2077 installation directory, check to mke sure launcher exists
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
                        launcherPath = dirBrowser.SelectedPath;
                        validPath = true;
                        File.Delete($"{savesDir}\\launcher_path.txt");
                        File.WriteAllText($"{savesDir}\\launcher_path.txt", launcherPath);
                    }
                    else { Console.WriteLine($"Launcher not found in {dirBrowser.SelectedPath}, please choose a different folder.\n"); }
                }
                else { Console.WriteLine("No folder selected, please try again.\n"); }
            }

            File.WriteAllText($"{savesDir}\\launcher_path.txt", launcherPath);

            Process launcher = new Process() { StartInfo = new ProcessStartInfo { FileName = $"{launcherPath}\\REDprelauncher.exe" } };

            launcher.Exited += (sender, e) => {System.Console.WriteLine("Process has exited.");};

            Console.Write("Scanning save file directory...");

            //Check to see if there is a playthrough already loaded, and ask user how to proceed
            if (Directory.Exists($"{savesDir}\\Inactive"))
            {
                Console.Write("Existing inactive saves folder detected, determining current active playthrough...");

                if (File.Exists($"{savesDir}\\last_loaded_playthrough.json")) 
                {
                    //deserialize last_loaded_playthrough.txt 
                    string fileString = File.ReadAllText($"{savesDir}\\last_loaded_playthrough.json");
                    JsonNode node = JsonNode.Parse(fileString);

                    Console.WriteLine("Complete!!\n");

                    //display last loaded playthrough and prompt user to proceed as is or switch playthrough
                    Console.WriteLine("The last playthrough loaded was:\n" + node["lifePath"].GetValue<string>()
                    + " (" + node["bodyGender"].GetValue<string>() + " Body + " + node["voiceGender"].GetValue<string>() + " Voice), "
                    + node["playThruId"].GetValue<string>() + "\n");

                    Console.WriteLine("Do you want to:\n1. Start game with this playthrough.\n2. Choose a different playthrough to load.");
                    switch (Convert.ToInt32(Console.ReadLine()))
                    {
                        case 1:
                            Console.Write("Complete!!\nLaunching Cyberpunk 2077...");
                            launcher.Start();
                            launcher.WaitForExit(30000);
                            return;
                        case 2:
                            moveSaves(new List<string>(Directory.EnumerateDirectories($"{savesDir}\\Inactive")), savesDir);
                            Directory.Delete($"{savesDir}\\Inactive");
                            break;
                        default:
                            Console.Write("Complete!!\nLaunching Cyberpunk 2077...");
                            launcher.Start();
                            launcher.WaitForExit(30000);
                            return;
                    }
                }
            }

            savesDirList = new List<string>(Directory.EnumerateDirectories(savesDir)); //build list of paths to all individual save directories
            Thread.Sleep(2000);

            Console.Write(" Complete!!\nBuilding save file list...");

            Save[] allSaves = scanSaves(savesDirList);

            Save[] playThruListArr = allSaves.Distinct(new SaveComparer()).ToArray(); //used to populate playthrough selection list
            Thread.Sleep(2000);

            //display list of available playthroughs
            Console.WriteLine(" Complete!!\n\nPlease select a playthrough to load:\n");
            foreach (var (index, item) in playThruListArr.Select((item, index) => (index, item)))
            {
                Console.WriteLine($"{index + 1}. {item.ToString()}, {Array.FindAll(allSaves, x => new SaveComparer().Equals(x, item)).Length} Saves");
            }

            Save choice = playThruListArr[Convert.ToInt32(Console.ReadLine()) - 1];

            var data = new JsonObject //JSON object containing relevant playthrough data to be written to last_loaded_playthrough.json
            {
                ["lifePath"] = choice.lifePath.ToString(),
                ["bodyGender"] = choice.bodyGender.ToString(),
                ["voiceGender"] = choice.voiceGender.ToString(),
                ["playThruId"] = choice.playThruId
            };

            Console.Write($"Loading {choice.ToString()} saves... ");

            Save[] allOtherSaves = Array.FindAll(allSaves, x => !(new SaveComparer().Equals(x, choice))); //list of all saves to be removed from main save directory

            moveSaves(allOtherSaves, $"{savesDir}\\Inactive");

            if (File.Exists($"{savesDir}\\last_loaded_playthrough.json")) //if file exists, delete it and remake it with new values
            {
                File.Delete($"{savesDir}\\last_loaded_playthrough.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                using (StreamWriter sw = File.CreateText($"{savesDir}\\last_loaded_playthrough.json")) { sw.WriteLine(data.ToJsonString(options)); }
            }
            else //make file with values
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                using (StreamWriter sw = File.CreateText($"{savesDir}\\last_loaded_playthrough.json")) { sw.WriteLine(data.ToJsonString(options)); }
            }

            Console.Write("Complete!!\nLaunching Cyberpunk 2077...");

            launcher.Start();
            launcher.WaitForExit(30000);
        }

        public class SaveComparer : IEqualityComparer<Save>
        {
            public bool Equals(Save s1, Save s2)
            {
                if (ReferenceEquals(s1, s2)) { return true; }

                else if (s1 == null || s2 == null) { return false; }

                return s1.playThruId == s2.playThruId
                && s1.lifePath == s2.lifePath
                && s1.bodyGender == s2.bodyGender
                && s1.voiceGender == s2.voiceGender;
            }

            public int GetHashCode(Save obj)
            {
                string s = $"{obj.playThruId} + {obj.lifePath} + {obj.bodyGender} + {obj.voiceGender}";
                return s.GetHashCode();
            }
        }

        public static Save[] scanSaves(List<string> dirList)
        {
            Save[] savesArr = new Save[dirList.Count];

            foreach (var (index, item) in dirList.Select((item, index) => (index, item))) //deserialize each save file's metadata and use to build array of all saves
            {
                string fileName = item + "\\metadata.9.json"; //Let's hope 2.3 doesn't mess with this...
                string jsonString = File.ReadAllText(fileName);
                JsonDocument document = JsonDocument.Parse(jsonString);
                JsonElement root = document.RootElement.GetProperty("Data").GetProperty("metadata");

                savesArr[index] = new Save(
                                        root.GetProperty("name").GetString(),
                                        item,
                                        root.GetProperty("playthroughID").GetString(),
                                        (LifePath)Enum.Parse(typeof(LifePath), root.GetProperty("lifePath").GetString()),
                                        (BodyGender)Enum.Parse(typeof(BodyGender), root.GetProperty("bodyGender").GetString()),
                                        (VoiceGender)Enum.Parse(typeof(VoiceGender), root.GetProperty("brainGender").GetString())
                                    );
            }

            return savesArr;
        }

        public static void moveSaves(List<string> saves, string dest) {
            
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
                            Directory.CreateDirectory($"{savesDir}\\Inactive"); //Note: I don't forsee a use case when the main save folder would not exist, but may be worth tweaking.
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

            Console.WriteLine("Saves moved successfully!!");
        }

        public static void moveSaves(Save[] saves, string dest) {
            foreach (Save save in saves)
            {
                string dir = save.saveDir.Split("\\").Last();

                try
                {
                    // Ensure the source directory exists
                    if (Directory.Exists(save.saveDir) == true)
                    {
                        // Ensure the destination directory exists
                        if (Directory.Exists(dest) == true)
                        {
                            // Perform the move
                            Directory.Move(save.saveDir, $"{dest}\\{dir}");
                        }
                        else
                        {
                            Directory.CreateDirectory($"{savesDir}\\Inactive");
                            Directory.Move(save.saveDir, $"{dest}\\{dir}");
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

            Console.WriteLine("Saves moved successfully!!");
        }
    
    }

}