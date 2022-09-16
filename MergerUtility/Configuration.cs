using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MergerUtility
{
    class Configuration
    {
        [JsonProperty("FileDirectory")]
        internal string FileDirectory { get; set; }

        [JsonProperty("MergeFileName")]
        internal string MergeFileName { get; set; }

        internal string NewFilePath {
            get {
                return $@"{FileDirectory}\{MergeFileName}";
            }
        }
    }
}
