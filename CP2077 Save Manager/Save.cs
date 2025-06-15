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
using System.Text.Json.Serialization;

namespace saveManager
{
    public enum LifePath {StreetKid,Nomad,Corporate}
    public enum BodyGender {Male,Female}
    public enum VoiceGender {Male,Female}

    public class Save
    {
        [JsonIgnore]
        public string SaveDir { get; set; } //TODO: maybe change to be saved as a Path instead of a string?
        public string PlayThruId { get; set; }
        public LifePath LifePath { get; set; } 
        public BodyGender BodyGender { get; set; } //Body type, either male or female. Does not relate to genitalia (probably saved in .dat file)
        public VoiceGender VoiceGender { get; set; } //Voice type, either male or female

        public Save(string saveDir, string playThruId, LifePath lifePath, BodyGender bodyGender, VoiceGender voiceGender)
        {
            SaveDir = saveDir;
            PlayThruId = playThruId;
            LifePath = lifePath;
            BodyGender = bodyGender;
            VoiceGender = voiceGender;
        }

        public override string ToString()
        {
            string output = $"{this.LifePath.ToString()} ({this.BodyGender.ToString()} Body + {this.VoiceGender.ToString()} Voice), {this.PlayThruId}";
            return output;
        }

        public Save(){}
    }
}