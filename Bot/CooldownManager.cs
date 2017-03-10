using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    class CooldownManager
    {

        
        Dictionary<string, DateTime> cooldowns;
        

        public CooldownManager()
        {
           cooldowns = new Dictionary<string, DateTime>();
        }

        public void addCooldown(string key, int cooldown)
        {
            if (cooldowns.ContainsKey(key)) return;

            cooldowns.Add(key, System.DateTime.Now.AddSeconds(cooldown));
        }

        public bool hasCooldown(string key)
        {
            if (!cooldowns.ContainsKey(key))
            {
                return false;
            }

            TimeSpan timeLeft = cooldowns[key].Subtract(System.DateTime.Now);
            if(timeLeft.TotalSeconds <= 0)
            {
                cooldowns.Remove(key);
                return false;
            }
            return true;
        }

        public void forceStop()
        {
            
        }
    }
}
