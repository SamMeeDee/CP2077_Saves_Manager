using System;

namespace saveManager
{
    public enum LifePath {StreetKid,Nomad,Corporate}
    public enum BodyGender {Male,Female}
    public enum VoiceGender {Male,Female}

    public class Save
    {

        public string saveName { get; set; }
        public string saveDir { get; set; }
        public LifePath lifePath { get; set; } //Nomad, StreetKid, or Corporate
        public BodyGender bodyGender { get; set; } //Body type, either male or female 
        public VoiceGender voiceGender { get; set; }

        public Save(string name, string dir, LifePath path, BodyGender bod, VoiceGender vox)
        {
            saveName = name;
            saveDir = dir;
            lifePath = path;
            bodyGender = bod;
            voiceGender = vox;
        }

        public Save() { }

    }
}