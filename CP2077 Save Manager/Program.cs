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

                //Console.WriteLine(allSaves[index].saveName + " - " + allSaves[index].charType);
            }

            Save[] tempTypeArr = allSaves.Distinct(new SaveComparer()).ToArray();

        }

    public class SaveComparer : IEqualityComparer<Save>
    {
        public bool Equals(Save s1, Save s2)
        {
            if(ReferenceEquals(s1, s2))
            {
                return true;
            }
            
            else if(s1 == null || s2 == null)
            {
                return false;
            }

            return s1.lifePath == s2.lifePath
            && s1.bodyGender == s2.bodyGender
            && s1.voiceGender == s2.voiceGender;
        }

        public int GetHashCode(Save obj)
        {
            string s = $"{obj.lifePath} + {obj.bodyGender} + {obj.voiceGender}";
            return s.GetHashCode();
        }
    }


    }

}