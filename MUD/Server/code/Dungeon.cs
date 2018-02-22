using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Dungeon
    {
        public Dictionary<String, Room> roomMap;

        public void Init()
        {
            roomMap = new Dictionary<string, Room>();

            {
                var room = new Room("Cave entrance", "A large cave entrance stands before you. ");
                room.north = "Cave Room 1";
                //room.south = "";
                //room.west = "";
                //room.east = "";

                roomMap.Add(room.Name, room);
            }
            {
                var room = new Room("Cave Room 1", "The expanse of the cave streches before you. ");
                room.north = "";
                room.south = "Cave entrance";
                //room.west = "";
                //room.east = "";

                roomMap.Add(room.Name, room);
            }

        }
    }
}
