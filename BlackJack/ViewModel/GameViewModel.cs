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

        public GameViewModel(List<User> players)
        {
            MyGame = new Game(players);
        }

        public void CoreGame()
        {
            bool _run = true;
            while (_run) // run game
            {
                MyGame.Players[0].MyCards[0].Add(MyGame.Decks[0].Cards[0]); // distribute card
                MyGame.Decks.RemoveAt(0);

                _run = IsGameFinish();
            }
        }

        public bool IsGameFinish()
        {
            //foreach (List<Card> c in MyGame.Players[0].MyCards)
            //{
            //    if(c)
            //}
            return true;
        }
    }
}
