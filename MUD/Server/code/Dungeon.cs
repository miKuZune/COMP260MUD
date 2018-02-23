using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    /*var room = new Room("", "");
                //room.north = "";
                //room.south = "";
                //room.west = "";
                //room.east = "";

                roomMap.Add(room.Name, room);*/
    public class Dungeon
    {
        public Dictionary<String, Room> roomMap;

        public void Init()
        {
            roomMap = new Dictionary<string, Room>();

            {
                var room = new Room("Outside the cave", "A large cave entrance stands before you. ", 0, null);
                room.north = "Cave entrance";
                //room.south = "";
                //room.west = "";
                //room.east = "";

                roomMap.Add(room.Name, room);
            }
            {
                var room = new Room("Cave entrance", "The expanse of the cave streches before you. ", 0, null);
                room.north = "Dark passageway";
                room.south = "Outside the cave";
                room.west = "Side cave";
                //room.east = "";

                roomMap.Add(room.Name, room);
            }

            {
                Enemy skeleton = new Enemy("Skeleton", 25, 5, 15);

                var room = new Room("Side cave", "A small side cave. A pool of water sits in the middle. Droplets fall from the ceiling to the pool periodically. ", 2, skeleton);
                room.east = "Cave entrance";

                roomMap.Add(room.Name, room);
            }
            {
                var room = new Room("Dark passageway", "A dark passageway, you struggle to see your way.", 0, null);
                room.north = "Cave of gems";
                room.south = "Cave entrance";
                //room.west = "";
                //room.east = "";

                roomMap.Add(room.Name, room);
            }
            {
                Enemy gemWarriors = new Enemy("Gem Warrior", 40, 0, 5);
                var room = new Room("Cave of gems", "A large cavern of gems spreads before. Beautiful glowing crystals fill the room, lighting your way.", 4, gemWarriors);
                //room.north = "";
                room.south = "Dark passageway";
                room.west = "Second dark passageway";
                room.east = "Gem passageway";

                roomMap.Add(room.Name, room);

            }
            {
                var room = new Room("Gem passageway", "A passage filled with gems and crystals.", 0, null);
                //room.north = "";
                room.south = "Gem trap room";
                //room.west = "";
                room.east = "Gem treasure room";

                roomMap.Add(room.Name, room);
            }
            {
                Enemy gemWarriors = new Enemy("Gem Warrior", 40, 0, 5);

                var room = new Room("Gem trap room", "Another large room filled with gems and an ominous bolder seemingly floating from the ceiling.", 5, gemWarriors);
                room.north = "Gem passageway";
                //room.south = "";
                //room.west = "";
                //room.east = "";

                roomMap.Add(room.Name, room);
            }

            {
                var room = new Room("Gem treasure room", "A cavern of large gems, all pointing towards a chest in the middle of the room.", 0, null);
                //room.north = "";
                //room.south = "";
                room.west = "Gem passageway";
                //room.east = "";

                roomMap.Add(room.Name, room);
            }
            {
                var room = new Room("Second dark passageway", "Another dark passageway. Again you struggle to find your way.", 0, null);
                room.north = "Lava room";
                //room.south = "";
                //room.west = "";
                room.east = "Cave of gems";

                roomMap.Add(room.Name, room);
            }
            {
                var room = new Room("Lava room", "A large room where the floor is covered in lava. A few stepping stones stand to help you reach the other rooms.", 0, null);
                room.north = "Second lava room";
                room.south = "Second dark passageway";
                //room.west = "";
                //room.east = "";

                roomMap.Add(room.Name, room);
            }

            {
                var room = new Room("Second lava room", "Another large room with lava for a floor.", 0, null);
                //room.north = "";
                room.south = "Lava room";
                //room.west = "";
                room.east = "Lava boss room";

                roomMap.Add(room.Name, room);

            }

            {
                var room = new Room("Lava boss room", "The room is filled with a lavaours pressence.", 0, null);
                room.north = "Lava treasure room";
                //room.south = "";
                room.west = "Second lava room";
                //room.east = "";

                roomMap.Add(room.Name, room);
            }
            {
                var room = new Room("Lava treasure room", "Lava falls from the walls, all flowing under a chest in the center of the room.", 0, null);
                //room.north = "";
                room.south = "Lava boss room";
                //room.west = "";
                //room.east = "";

                roomMap.Add(room.Name, room);
            }
        }
    }
}
