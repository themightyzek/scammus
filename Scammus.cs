using System;
using System.Collections.Concurrent;

namespace scammus
{
    class Scammus
    {
        public YesNoPred prediction;
        private ConcurrentDictionary<string, BankAccount> accounts = new ConcurrentDictionary<string, BankAccount>();
        private const uint startMoney = 10000;

        public Scammus()
        {
            // Prediction("test", "yes", "no");
        }

        public string LockPrediction()
        {
            prediction.isOpen = false;
            return $"Prediction {prediction.text} has been locked";
        }

        public string Prediction(string text, string answer1, string answer2)
        {
            prediction = new YesNoPred() {
                text = text, 
                yesText = answer1, 
                noText = answer2,
                isOpen = true
            };
            return $"Prediction {text} created";
        }

        public string Bet(string option, string name, string amount)
        {
            bool yes = prediction.yesText.StartsWith(option);

            if(
                System.Int32.TryParse(
                    amount, 
                    System.Globalization.NumberStyles.AllowLeadingSign, 
                    null, 
                    out int int_amount)
            )
            {
                accounts.TryAdd(name, new BankAccount(startMoney));
                lock (accounts[name].balance_lock)
                {
                    var act = accounts[name];
                    return prediction.ModifyWager(yes, name, int_amount, ref act);
                }

            }
            return "youre full of shit mate";
        }

        public string Result(string outcome)
        {
            lock(prediction)
            {
                bool outcomeYes = prediction.yesText.StartsWith(outcome);
                return prediction.End(outcomeYes, accounts);
            }
        }

        public string Balance(string name)
        {
            return $"{name}: {accounts.GetOrAdd(name, s => new BankAccount(startMoney)).balance}";
        }

        public string Give(string sender, string recipient, string amount)
        {

            lock(accounts[sender].balance_lock)
            {
                System.Int32.TryParse(
                    amount, 
                    System.Globalization.NumberStyles.None, 
                    null, 
                    out int int_amount);

                if(int_amount < 0)
                    return "nice try du bitch AHJA";

                uint uint_amount = (uint)int_amount;
        
                if(accounts[sender].balance < int_amount)
                    return "youre too poor";

                lock(accounts[recipient].balance_lock)
                {
                    accounts[sender].balance -= uint_amount;
                    accounts[recipient].balance += uint_amount;
                }
            }

            return $"{sender} graciously gave {recipient} {amount} money.";
        }
    }
}