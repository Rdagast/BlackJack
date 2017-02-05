using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Deck
    {
        public List<Card> Cards { get; set; }

        public Deck()
        {
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                foreach (Name name in Enum.GetValues(typeof(Name)))
                {
                    Cards.Add(new Card(color, name));
                }
            }

        }
    }
}
