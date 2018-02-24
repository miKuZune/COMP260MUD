using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Enemy
    {
        //Enemy intialisation
        public Enemy(string name, int startHealth, int minDmg, int maxDmg)
        {
            this.name = name;
            this.enemyHealth.SetHealth(startHealth);
            this.minDamage = minDmg;
            this.maxDamage = maxDmg;
        }
        //Gets an integer for damage to deal
        public int GetRandomDamgeInRange()
        {
            int damage = 0;

            Random ran = new Random();
            damage = ran.Next(minDamage, maxDamage);

            return damage;
        }
        //Deals damage to the given player.
        public void EnemyAttack(Player attackPlayer, int damage)
        {
            attackPlayer.playerHealth.TakeHealth(damage);
        }
        
        //Get and set the name of the enemy
        public void SetName(string name)
        {
            this.name = name;
        }
        public string GetName()
        {
            return this.name;
        }

        //Variables
        String name;

        public Health enemyHealth = new Health(0);
        public int minDamage;
        public int maxDamage;
    }
}
