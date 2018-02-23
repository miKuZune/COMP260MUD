using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Health
    {
        public Health(int startHealth)
        {
            this.health = startHealth;
        }

        public bool IsDead()
        {
            if(health <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHealth()
        {
            return health;
        }
        public void SetHealth(int value)
        {
            this.health = value;
        }
        public void TakeHealth(int damage)
        {
            this.health -= damage;

        }
        public void AddHealth(int health)
        {
            this.health += health;
        }


        //Variables
        int health;
    }
}
