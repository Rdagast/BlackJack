using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Game
    {
        
        public User Winner { get; set; }
        //public List<User> Players { get; set; }
        public bool IsStop { get; set; }

        public Game()
        {
            
            //foreach(User p in players)
            //{
            //    this.Players.Add(p);
            //}

            this.IsStop = false;
        }
    } 
}
