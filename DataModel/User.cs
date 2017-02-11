using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{

    public class User
    {
        [JsonProperty("status")]
        public int status { get; set; }

       

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("username")]
        public String UserName { get; set; }
        [JsonProperty("email")]
        public String Email { get; set; }
        [JsonProperty("created_at")]
        public DateTime Created_at { get; set; }

        [JsonProperty("updated_at")]
        public DateTime Updated_at { get; set; }
        [JsonProperty("firstname")]
        public String FirstName { get; set; }
        [JsonProperty("lastname")]
        public String Lastname { get; set; }
        [JsonProperty("stack")]
        public Double Stack { get; set; }

        [JsonProperty("is_connected")]
        public int IsConnected { get; set; }

        [JsonProperty("last_refill")]
        public DateTime LastRefill { get; set; }


        [JsonProperty("password")]
        public String Password { get; set; }
        public String Secret { get; set; }


        //public List<List<Card>> MyCards { get; set; }
        //public List<Double> Bets { get; set; }
        
        public Double Assurance { get; set; }
        public ObservableCollection<UserHand> UserHands { get; set; }



        //public User()
        //{

        //}
        public User()
        {
            this.status = 0;
            this.Id = 0;
            this.UserName = null;
            this.Email = null;
            this.Created_at = new DateTime();
            this.Updated_at = new DateTime();
            this.FirstName = null;
            this.Lastname = null;
            this.Stack = 0;
            this.IsConnected = 0;
            this.LastRefill = new DateTime();
            this.UserHands = new ObservableCollection<UserHand>();
            this.Assurance = 0;
        }

        ~User()
        {

        }
    }
}
