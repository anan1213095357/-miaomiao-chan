using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{


    public class StreamResponse
    {
        [JsonProperty("id")] 
        public string? ID { get; set; }
        [JsonProperty("object")] 
        public string? Object { get; set; }
        [JsonProperty("created")] 
        public int Created { get; set; }
        [JsonProperty("model")] 
        public string? Model { get; set; }
        [JsonProperty("system_fingerprint")] 
        public string? SystemFingerprint { get; set; }
        [JsonProperty("choices")]
        public Choice[]? Choices { get; set; }

        [JsonProperty("usage")] 
        public Usage? Usage { get; set; }



    }





}
