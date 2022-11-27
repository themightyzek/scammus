using System;

namespace scammus {
    class TwitchSpawner {
        bool isActive = false;
        DateTime activation;
        public void Activate(string sender)
        {
            if(sender == "CuteCrait")
            {
                isActive = true;
                activation = DateTime.Now;
            }
        }

        public string Spawn(string message)
        {
            if(!isActive || DateTime.Now.Subtract(activation).TotalSeconds < 60)
            {
                isActive = false;
                return null;
            }
            else 
            {
                return message;
            }
        }
    }
}