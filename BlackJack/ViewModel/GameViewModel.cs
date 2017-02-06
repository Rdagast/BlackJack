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
        private Double _bet;

        public GameViewModel(List<Player> players)
        {
            MyGame = new Game(players);
            this._bet = 0;
        }



        public void IsGameFinish()
        {

        }
    }
}
