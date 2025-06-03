using System;

namespace saveManager
{
    public enum LifePath { StreetKid, Nomad, Corpo }
    public enum Gender {Male,Female}

    public class Save
    {

        public string name { get; set; }
        public string dir { get; set; }
        public LifePath charPath
        {
            get;
            set => field = (LifePath)Enum.Parse(typeof(LifePath), value);
        }

        public Gender charGender
        {
            get;
            set => field = (Gender)Enum.Parse(typeof(Gender), value);
        }

        public Save() { }
        
    }
}