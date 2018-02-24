using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class AttackManager
    {
        public AttackManager(Player player, Enemy enemy)
        {
            this.player = player;
            this.enemy = enemy;
        }

        //Player deals damage to the enemy and a message is sent back.
        public string PlayerAttack(int damageToDeal)
        {
            string messageBack = null;

            enemy.enemyHealth.TakeHealth(damageToDeal);

            messageBack = player.name + " has attacked " + enemy.GetName() + " for " + damageToDeal + ".";

            return messageBack;
        }
        //Gets the enemies ID from the list of enemies in the room.
        int GetEnemeyPosInList()
        {
            int id = 0;

            for(int i = 0; i < player.currRoom.enemyList.Count; i++)
            {
                if(player.currRoom.enemyList[i] == enemy)
                {
                    id = i;
                }
            }

            return id;
        }

        //Checks if there is more than 0 enemies in the room.
        bool EnemiesLeftInRoom()
        {
            if(player.currRoom.enemyList.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Returns a string containing the names of all the enemies left in the room.
        string EnemiesInRoom()
        {
            string enemiesInRoom = null;

            for(int i = 0; i < player.currRoom.enemyList.Count; i++)
            {
                enemiesInRoom = enemiesInRoom + player.currRoom.enemyList[i].GetName() + " ";
            }

            return enemiesInRoom;
        }
        //When the enemy is killed, send a message saying they are dead, remove them from the room and output the enemies left in the room if any.
        public string OnEnemyDead()
        {
            string message = null;
            int enemyID = GetEnemeyPosInList();

            message = enemy.GetName() + " has been slain! ";
            player.currRoom.enemyList.Remove(player.currRoom.enemyList[enemyID]);

            if(EnemiesLeftInRoom())
            {
                message = message + "The enemies in the room are now: " + EnemiesInRoom();
            }
            else
            {
                message = message + "You have cleared all enemies in the room.";
            }

            return message;
        }
        //Resets the player's health when they die and sends a message.
        public string OnPlayerDeath()
        {
            string message = null;

            message = player.name + " has died. They will now be sent back to the start of the dungeon.";
            player.playerHealth.SetHealth(100);

            return message;
        }

        public string EnemyAttackPlayer(int damageToDeal)
        {
            string message = null;

            enemy.EnemyAttack(player, damageToDeal);

            message = player.name + " has been attacked by " + enemy.GetName() + " for " + damageToDeal + ". This player now has " + player.playerHealth.GetHealth() + " health. ";

            return message;
        }

        //Variables
        public Player player;
        public Enemy enemy;
    }
}
