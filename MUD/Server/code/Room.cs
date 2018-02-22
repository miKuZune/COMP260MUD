using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Room
    {
        public Room(String name, String description)
        {
            this.Name = name;
            this.description = description;
        }
        public String north
        {
            get { return exits[0]; }
            set { exits[0] = value; }
        }
        public String south
        {
            get { return exits[1]; }
            set { exits[1] = value; }
        }
        public String east
        {
            get { return exits[2]; }
            set { exits[2] = value; }
        }
        public String west
        {
            get { return exits[3]; }
            set { exits[3] = value; }
        }

        //Variables
        public String Name = "";
        public String description = "";

        public String[] exits = new String[4];
        public static String[] exitNames = { "NORTH", "SOUTH", "EAST", "WEST" };
    }
}
