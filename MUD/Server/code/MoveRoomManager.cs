using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class MoveRoomManager
    {
        public MoveRoomManager(Player player, string[] input)
        {
            this.player = player;
            this.input = input;
        }

        //Checks if the enemy count in the current room is more than 0.
        public bool CheckForEnemiesInRoom()
        {
            bool enemiesInRoom = false;

            if (player.currRoom.enemyList.Count > 0)
            {
                enemiesInRoom = true;
            }

            return enemiesInRoom;
        }

        //Returns a string with a list of enemy names in the current room
        string GetListOfEnemies()
        {
            string message = null;

            for(int i = 0; i < player.currRoom.enemyList.Count; i++)
            {
                message = message + player.currRoom.enemyList[i].GetName() + " ";
            }

            return message;
        }

        //Returns a string containing all players names currently in the room.
        public string GetPlayersInRoom(List<Player> playerList)
        {
            string message = null;
            bool playerInRoom = false;

            for(int i = 0; i < playerList.Count; i++)
            {
                if(playerList[i].currRoom == player.currRoom && playerList[i] != player)
                {
                    if (!playerInRoom)
                    {
                        playerInRoom = true;
                        message = "You see the following people in the room: ";
                    }
                    message = message + playerList[i].name + " ";
                }
            }

            return message;
        }

        //Returns a string to send to players about the enemies in a room.
        public string ListEnemiesInRoom()
        {
            string message = null;
            if (CheckForEnemiesInRoom())
            {
                message = "You see several enemies in the room: " + GetListOfEnemies();
            }

            return message;
        }

        //Returns a string with all possible directions in the current room.
        public string GetPossibleDirections()
        {
            string message = "You can go ";

            if (player.currRoom.north != null)
            {
                message = message + " north";
            }
            if (player.currRoom.south != null)
            {
                message = message + " south";
            }
            if (player.currRoom.east != null)
            {
                 message = message + " east ";
            }
            if (player.currRoom.west != null)
            {
                message = message + " west.";
            }

            return message;
        }

        //Checks if the input direction is valid. Moves to room if so. Returns a error message if false.
        public string MoveRoom(Dictionary<String, Room> roomMap)
        {
            string message = null;

            if (input[1].ToLower() == "north" && player.currRoom.north != null)
            {
                player.currRoom = roomMap[player.currRoom.north];
            }
            else if (input[1].ToLower() == "south" && player.currRoom.south != null)
            {
                player.currRoom = roomMap[player.currRoom.south];
            }
            else if (input[1].ToLower() == "east" && player.currRoom.east != null)
            {
                player.currRoom = roomMap[player.currRoom.east];
            }
            else if (input[1].ToLower() == "west" && player.currRoom.west != null)
            {
                player.currRoom = roomMap[player.currRoom.west];
            }
            else
            {
                message = "ERROR, you cannot go that way.";
            }

            return message;
        }
        //Variables
        Player player;
        string[] input;
    }
}
