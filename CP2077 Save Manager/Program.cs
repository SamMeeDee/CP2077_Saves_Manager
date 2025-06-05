// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

namespace saveManager
{
    class Program
    {
        static string savesDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077"; //CP2077 save file location in Windows 11

        static void Main()
        {
            List<string> savesDirList;

            Console.WriteLine("Welcome to the CP2077 Save Manager!!!");
            Console.WriteLine("Scanning your save file directory...");

            savesDirList = new List<string>(Directory.EnumerateDirectories(savesDir)); //put the paths to all save directories in a list

            Console.WriteLine("Directory scan complete. Building save file list...");

            Save[] allSaves = new Save[savesDirList.Count];
            List<string> tempTypeList = new List<string>();

            foreach (var (index, item) in savesDirList.Select((item, index) => (index, item))) //deserialize each save file's metadata and use it to build array of all Saves
            {
                string fileName = item + "\\metadata.9.json";
                string jsonString = File.ReadAllText(fileName);
                JsonDocument document = JsonDocument.Parse(jsonString);
                JsonElement root = document.RootElement.GetProperty("Data").GetProperty("metadata");

                allSaves[index] = new Save(
                                        root.GetProperty("name").GetString(),
                                        item,
                                        (LifePath)Enum.Parse(typeof(LifePath), root.GetProperty("lifePath").GetString()),
                                        (BodyGender)Enum.Parse(typeof(BodyGender), root.GetProperty("bodyGender").GetString()),
                                        (VoiceGender)Enum.Parse(typeof(VoiceGender), root.GetProperty("brainGender").GetString())
                                    );

                tempTypeList.Add($"{allSaves[index].lifePath} ({allSaves[index].bodyGender} Body + {allSaves[index].voiceGender} Voice)"); //populate temp list of type strings to then build final list for UI

                //Console.WriteLine(allSaves[index].saveName + " - " + allSaves[index].charType);
            }

            List<string> saveTypeList = tempTypeList.Distinct().ToList();
            //saveTypeList.ForEach(Console.WriteLine);

        }


    }

}