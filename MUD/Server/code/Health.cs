using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Health
    {
        //Health initalisation
        public Health(int startHealth)
        {
            this.health = startHealth;
        }

        //Returns true if the health is less than 0
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

        //Get and set health as set values.
        public int GetHealth()
        {
            return health;
        }
        public void SetHealth(int value)
        {
            this.health = value;
        }
        //Takes an integer for damage and takes that away from health.
        public void TakeHealth(int damage)
        {
            this.health -= damage;

        }
        //Takes an integer for damage and adds that to health.
        public void AddHealth(int health)
        {
            this.health += health;
        }


        //Variables
        int health;
    }
}
