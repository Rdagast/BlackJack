using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Game
    {
        public List<Deck> Decks { get; set; }
        public List<Player> Players { get; set; }

        public Game(List<Player> players)
        {
            for (int i = 0; i < 6; i++)
            {
                this.Decks.Add(new Deck());
            }

            foreach(Player p in players)
            {
                this.Players.Add(p);
            }
        }
    } 
}
