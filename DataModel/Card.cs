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


        public Card(Color color, Name name)
        {
            this.Type = color;
            this.Name = name;
            this.Value = CalcValueCard();
            this.IsCutCard = false;
        }
        public Card()
        {
            this.IsCutCard = true;
        }

        public int CalcValueCard()
        {
            return 0;
        }
    }
}
