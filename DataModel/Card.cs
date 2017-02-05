using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Card
    {
        private Color _type { get; set; }
        private Name _name { get; set; }
        private int _value { get; set; }

        

        public Card(Color color, Name name)
        {
            this._type = color;
            this._name = name;
            this._value = CalcValueCard();
        }
        public int CalcValueCard()
        {
            return 0;
        }
    }
}
