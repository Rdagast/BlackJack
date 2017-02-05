using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{

    public class User
    {
        [JsonProperty("status")]
        public int status { get; set; }

        [JsonProperty("tokens\":{\"token_type")]
        public String token_type { get; set; }
        [JsonProperty("expires_in")]
        public double expires_in { get; set; }
        [JsonProperty("access_token")]
        public String access_token { get; set; }
        [JsonProperty("refresh_token")]
        public String refresh_token { get; set; }

        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("username")]
        public String username { get; set; }
        [JsonProperty("email")]
        public String email { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
        [JsonProperty("firstname")]
        public String firstname { get; set; }
        [JsonProperty("lastname")]
        public String lastname { get; set; }
        [JsonProperty("stack")]
        public Double stack { get; set; }

        [JsonProperty("is_connected")]
        public int _isConnected { get; set; }

        [JsonProperty("last_refill")]
        public DateTime _lastRefill { get; set; }


        [JsonProperty("password")]
        public String password { get; set; }
        public String secret { get; set; }
        
       
        
       
        
      
       

        public Double _bet { get; set; }

        public List<List<Card>> _listCards { get; set; }
        public Double _assurance { get; set; }

        //public User()
        //{

        //}
        public User()
        {
            this.status = 0;
            this.token_type = null;
            this.expires_in = 0;
            this.access_token = null;
            this.refresh_token = null;
            this.id = 0;
            this.username = null;
            this.email = null;
            this.created_at = new DateTime();
            this.updated_at = new DateTime();
            this.firstname = null;
            this.lastname = null;
            this.stack = 0;
            this._isConnected = 0;
            this._lastRefill = new DateTime();
           
           
        }




    }
}
