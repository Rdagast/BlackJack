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
            this.Cards = new List<Card>();
           
        }
        public void CreateDeck()
        {
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                foreach (Name name in Enum.GetValues(typeof(Name)))
                {
                    Cards.Add(new Card(color, name, CalcValueCard()));
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

        public int CalcValueCard()
        {
            int valeurTotal = 0;

            foreach (var card in Cards)
            {
                switch (card.Name)
                {
                    case Name.TWO: valeurTotal += 2; break;
                    case Name.THREE: valeurTotal += 3; break;
                    case Name.FOUR: valeurTotal += 4; break;
                    case Name.FIVE: valeurTotal += 5; break;
                    case Name.SIX: valeurTotal += 6; break;
                    case Name.SEVEN: valeurTotal += 7; break;
                    case Name.EIGHT: valeurTotal += 8; break;
                    case Name.NINE: valeurTotal += 9; break;
                    case Name.TEN: valeurTotal += 10; break;
                    case Name.JACK: valeurTotal += 10; break;
                    case Name.QUEEN: valeurTotal += 10; break;
                    case Name.KING: valeurTotal += 10; break;
                    case Name.ACE:
                        if (valeurTotal > 10)
                        {
                            valeurTotal += 1;
                        }
                        else
                        {
                            valeurTotal += 11;
                        }
                        break;
                }
            }
            return valeurTotal;
        }
    }
}
