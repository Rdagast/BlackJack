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
            //Create 54 cards for a deck
            Cards = new List<Card>();
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                foreach (Name name in Enum.GetValues(typeof(Name)))
                {
                    Cards.Add(new Card(color, name));
                }
            }
        }

        public void ShuffleList()
        {
            List<Card> cardList = new List<Card>();

            Random r = new Random();
            int randomIndex = 0;
            while (this.Cards.Count > 0)
            {
                randomIndex = r.Next(0, this.Cards.Count); //Choose a random index in the list
                cardList.Add(this.Cards[randomIndex]); //add it to the new, random list
                this.Cards.RemoveAt(randomIndex); //remove to avoid duplicates
            }
            this.Cards = cardList;
        }

        public void AddCutCard()
        {
            var random = new Random();
            int randomIndex = random.Next(0, this.Cards.Count);
            this.Cards.Insert(randomIndex, new Card());
        }

        
    }
}
