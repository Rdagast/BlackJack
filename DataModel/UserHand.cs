using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class UserHand
    {
        public List<Card> Cards { get; set; }
        public int Value { get; set; }
        public Double Bet { get; set; }
        public bool IsFinish { get; set; }

        public UserHand()
        {
            this.Cards = new List<Card>();
            this.Bet = 0;
            this.Value = 0;
            this.IsFinish = false;
        }
    }
}
