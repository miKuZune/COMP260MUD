using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Enemy
    {
        public Enemy(string name, int startHealth, int minDmg, int maxDmg)
        {
            this.name = name;
            this.enemyHealth.SetHealth(startHealth);
            this.minDamage = minDmg;
            this.maxDamage = maxDmg;
        }

        public int GetRandomDamgeInRange()
        {
            int damage = 0;

            Random ran = new Random();
            damage = ran.Next(minDamage, maxDamage);

            return damage;
        }

        public void EnemyAttack(Player attackPlayer, int damage)
        {
            attackPlayer.playerHealth.TakeHealth(damage);
        }

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
