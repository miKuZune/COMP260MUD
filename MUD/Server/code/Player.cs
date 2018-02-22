using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Player
    {
        public Player(Socket owner,String name, Room currRoom)
        {
            this.owner = owner;
            this.name = name;
            this.currRoom = currRoom;
        }

        //Variables
        public Socket owner;
        public String name;

        public Room currRoom;
    }
}
