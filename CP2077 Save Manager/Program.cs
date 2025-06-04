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
        static string savesDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+"\\Saved Games\\CD Projekt Red\\Cyberpunk 2077";

        static void Main()
        {
            List<string> savesList;

            Console.WriteLine("Hello World!! This is the CP2077 Save Manager!!!");

            savesList = new List<string>(Directory.EnumerateDirectories(savesDir));

            Save[] allSavesArr = new Save[savesList.Count];

            foreach (var (index, item) in savesList.Select((item, index) => (index, item))) 
            {
                
                string fileName = item+"\\metadata.9.json";
                string jsonString = File.ReadAllText(fileName);
                JsonDocument document = JsonDocument.Parse(jsonString);
                JsonElement root = document.RootElement.GetProperty("Data").GetProperty("metadata");

                Save save = new Save (root.GetProperty("name").GetString(),
                                    item,
                                    (LifePath)Enum.Parse(typeof(LifePath),root.GetProperty("lifePath").GetString()),
                                    (Gender)Enum.Parse(typeof(Gender),root.GetProperty("bodyGender").GetString())
                                    );

                allSavesArr[index] = save;

                Console.WriteLine(allSavesArr[index].saveName + " - " + allSavesArr[index].charGender + " " + allSavesArr[index].charPath);
            }

            /* string fileName = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+"\\Saved Games\\CD Projekt Red\\Cyberpunk 2077\\QuickSave-7\\metadata.9.json";
            string jsonString = File.ReadAllText(fileName);
            JsonDocument document = JsonDocument.Parse(jsonString);

            JsonElement root = document.RootElement.GetProperty("Data").GetProperty("metadata");

            Console.WriteLine(root.GetProperty("lifePath").GetRawText());
            Console.WriteLine(root.GetProperty("bodyGender").GetRawText());
            Console.WriteLine(root.GetProperty("name").GetRawText());
            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            testSave = new Save (root.GetProperty("name").GetRawText(),
                                 Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+"\\Saved Games\\CD Projekt Red\\Cyberpunk 2077\\QuickSave-7",
                                 (LifePath)Enum.Parse(typeof(LifePath),root.GetProperty("lifePath").GetString()),
                                 (Gender)Enum.Parse(typeof(Gender),root.GetProperty("bodyGender").GetString())
                                );

            Console.WriteLine("Save Name: "+testSave.saveName);
            Console.WriteLine("Lifepath: "+testSave.charPath);
            Console.WriteLine("Gender: "+testSave.charGender); */
        }   
    }

}