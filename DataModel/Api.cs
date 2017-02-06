using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
   
    public class Api
    {
        [JsonProperty("user")]
        public User user { get; set; }
        [JsonProperty("tokens")]
        public Token token { get; set; }
        public Api()
        {
            user = new User();
            this.token = new Token();
        }
        
       
    }
}
