using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class ErrorApi
    {
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("error_code")]
        public String Error_code { get; set; }
        [JsonProperty("error")]
        public String Error { get; set; }
        [JsonProperty("message")]
        public String  Message { get; set; }
    }
}
