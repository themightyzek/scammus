using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace scammus {
    public class Pushups
    {
        ConcurrentDictionary<string, int> pushups = new ConcurrentDictionary<string, int>();

        public Pushups()
        {
        }

        /// Draws a winner from the submitted pushup numbers
        public string Roll()
        {
            if(pushups.Count == 0) return "jetzt macht halt endlich pushups";
            
            List<string> allpushups = new List<string>();
            foreach(string key in pushups.Keys)
            {
                for(int i = 0; i < pushups[key]; i++)
                {
                    allpushups.Add(key);
                }
            }

            int win = RandomNumberGenerator.GetInt32(allpushups.Count);
            string winner = allpushups[win];
            return $"tadaa meine homies - {winner} hat die schiebhochherausforderung gewonnen!";
        }

        public Task<string> SetPushups(string chatter, int amount)
        {
            pushups.AddOrUpdate(chatter, amount, (chatter, amount) => amount);
            return Task.FromResult($"{chatter} hat {amount} pushups gemacht");
        }
    }
}