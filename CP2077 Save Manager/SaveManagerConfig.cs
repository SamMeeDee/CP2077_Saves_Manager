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
    public class SaveManagerConfig
    {

        public Save[] LastLoadedArr { get; set; }
        public string LauncherDir { get; set; }
        public int SaveNum { get; set; }

        public SaveManagerConfig(Save[] lastLoadedArr, string launcherDir, int saveNum)
        {
            LastLoadedArr = lastLoadedArr;
            LauncherDir = launcherDir;
            SaveNum = saveNum;
        }

        /* public override string ToString() 
        {
            string output = $"{this.lifePath.ToString()} ({this.bodyGender.ToString()} Body + {this.voiceGender.ToString()} Voice), {this.playThruId}";
            return output;
        } */
    }
}