using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Player : User
    {
        public int _isConnected { get; set; }
        public Double stack { get; set; }

        public DateTime _lastRefill { get; set; }

        public Double _bet { get; set; }

        public List<List<Card>> _listCards { get; set; }

        public Double _assurance { get; set; }
    }
}
