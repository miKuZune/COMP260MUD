using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class Player
    {
        public Player(Socket owner,String name, Room currRoom)
        {
            this.owner = owner;
            this.name = name;
            this.currRoom = currRoom;
        }

        public int GetDamage()
        {
            int damage = 0;

            Random ran = new Random();
            damage = ran.Next(minDamage, maxDamage);

            return damage;
        }

        //Variables
        public Socket owner;
        public String name;

        public Room currRoom;

        public Health playerHealth = new Health(100);

        public int minDamage = 10;
        public int maxDamage = 25;
    }
}
