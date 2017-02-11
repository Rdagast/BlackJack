using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Card
    {
        public Color Type { get; set; }
        public Name Name { get; set; }
        public int Value { get; set; }
        public bool IsCutCard { get; set; }
        public String PictureUrl { get; set; }


        public Card(Color color, Name name)
        {
            this.Type = color;
            this.Name = name;
            this.Value = CalcValueCard();
            this.IsCutCard = false;
            this.PictureUrl = "/Assets/Images/" + this.Type + "/" + this.Type + "_" + this.Name + ".png";
        }
        public Card()
        {
            this.IsCutCard = true;
        }

        public int CalcValueCard()
        {
            int valeurTotal = 0;

            
                switch (Name)
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
            
            return valeurTotal;
        }
    }
}
