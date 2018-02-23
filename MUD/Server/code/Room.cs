using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Room
    {
        public Room(String name, String description, int numOfEnemies, Enemy enemy)
        {
            this.Name = name;
            this.description = description;
            //this.enemies = new Enemy[numOfEnemies];

            for(int i = 0; i < numOfEnemies; i++)
            {
                int enemyID = i + 1;
                Enemy enemyToAdd = new Enemy(enemy.GetName() + enemyID, enemy.enemyHealth.GetHealth(), enemy.minDamage, enemy.maxDamage);
                enemyList.Add(enemyToAdd);
                //this.enemies[i] = new Enemy(enemy.GetName() + enemyID, enemy.enemyHealth.GetHealth(), enemy.minDamage, enemy.maxDamage);
            }
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

        //public Enemy[] enemies;
        public List<Enemy> enemyList = new List<Enemy>();
    }
}
