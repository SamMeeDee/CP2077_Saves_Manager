using System;

namespace saveManager
{
    public enum LifePath {StreetKid,Nomad,Corpo}
    public enum Gender {Male,Female}

    public class Save
    {

        public string saveName { get; set; }
        public string saveDir { get; set; }
        public LifePath charPath { get; set; }
        public Gender charGender { get; set; }

        public Save(string name, string dir, LifePath path, Gender gender) 
        { 
            saveName=name;
            saveDir=dir;
            charPath=path;
            charGender=gender;
        }

        public Save() {}
        
    }
}