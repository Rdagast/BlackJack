using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.ViewModel
{
    class GameViewModel
    {

        public Game MyGame { get; set; }

        public GameViewModel()
        {
            MyGame = new Game();
        }
    }
}
