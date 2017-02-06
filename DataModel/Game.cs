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
        public List<User> Players { get; set; }

        public Game(List<User> players)
        {
            for (int i = 0; i < 6; i++)
            {
                this.Decks.Add(new Deck());
                foreach (Deck d in Decks)
                {
                    d.ShuffleList();
                }
                this.Decks[2].AddCutCard(); // add cut card between 50% and 80% of game Cards
            }

            foreach(User p in players)
            {
                this.Players.Add(p);
            }
        }
    } 
}
