using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class TableToGameNav
    {
        public Api MyApi { get; set; }
        public Table GameTable { get; set; }

        public TableToGameNav(Api api, Table table)
        {
            this.MyApi = api;
            this.GameTable = table;
        }
    }
}
