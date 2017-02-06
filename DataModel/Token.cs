using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Token
    {
        [JsonProperty("token_type")]
        public String token_type { get; set; }
        [JsonProperty("expires_in")]
        public double expires_in { get; set; }
        [JsonProperty("access_token")]
        public String access_token { get; set; }
        [JsonProperty("refresh_token")]
        public String refresh_token { get; set; }

        public Token()
        {
            this.token_type = null;
            this.expires_in = 0;
            this.access_token = null;
            this.refresh_token = null;
        }
    }
}
