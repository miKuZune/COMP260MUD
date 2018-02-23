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

        public void EnemyAttack(Player attackPlayer)
        {
            int damageToDeal = 0;
            Random ran = new Random();
            damageToDeal = ran.Next(minDamage, maxDamage);

            attackPlayer.playerHealth.TakeHealth(damageToDeal);
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

        Health enemyHealth = new Health(0);
        int minDamage;
        int maxDamage;
    }
}
