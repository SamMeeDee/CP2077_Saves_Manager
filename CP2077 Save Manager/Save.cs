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

using System;

namespace saveManager
{
    public enum LifePath {StreetKid,Nomad,Corporate}
    public enum BodyGender {Male,Female}
    public enum VoiceGender {Male,Female}

    public class Save
    {

        public string saveName { get; set; } //TODO: refactor to have this field populated from saveDir, or maybe remofe altogether. Not currently used.
        public string saveDir { get; set; } //TODO: maybe change to be saved as a Path instead of a string?
        public string playThruId { get; set; }
        public LifePath lifePath { get; set; } 
        public BodyGender bodyGender { get; set; } //Body type, either male or female. Does not relate to genitalia (probably saved in .dat file)
        public VoiceGender voiceGender { get; set; } //Voice type, either male or female

        public Save(string name, string dir, string playthru, LifePath path, BodyGender bod, VoiceGender vox)
        {
            saveName = name;
            saveDir = dir;
            playThruId = playthru;
            lifePath = path;
            bodyGender = bod;
            voiceGender = vox;
        }

        public override string ToString() 
        {
            string output = $"{this.lifePath.ToString()} ({this.bodyGender.ToString()} Body + {this.voiceGender.ToString()} Voice), {this.playThruId}";
            return output;
        }

        public Save() { }
    }
}