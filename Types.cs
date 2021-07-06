using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;


namespace scammus
{
    class Prediction
    {
        public string text;
        System.TimeSpan submissionTime;
    }

    class YesNoPred : Prediction
    {
        public string yesText;
        public string noText;
        ConcurrentDictionary<string, uint> yesWagers = new ConcurrentDictionary<string, uint>();
        ConcurrentDictionary<string, uint> noWagers = new ConcurrentDictionary<string, uint>();

        public string ModifyWager(bool yes, string name, int amount, ref BankAccount account)
        {
            if (account.balance < amount)
                return "youre too poor for that shit";

            var dict = yes ? yesWagers : noWagers;

            if (!dict.ContainsKey(name))
            {
                if (amount > 0)
                {
                    account.balance -= (uint)amount;
                    dict.TryAdd(name, (uint)amount);
                }
            }
            else
            {
                if (amount >= 0)
                {
                    account.balance -= (uint)amount;
                    dict[name] += (uint)amount;
                }
                else if (dict[name] + amount >= 0)
                {
                    dict[name] = (uint)(dict[name] + amount);
                    account.balance = (uint)(account.balance - amount);
                }
            }

            string text = yes ? yesText : noText;
            return $"{name} adds {amount} for total {dict[name]} to \"{text}\"";
        }

        public string End(bool resultYes, ConcurrentDictionary<string, BankAccount> accounts)
        {
            var winningLedger = resultYes ? yesWagers : noWagers;
            var losingLedger = resultYes ? noWagers : yesWagers;

            uint pot = 0;
            uint winningTotal = 0;

            foreach (var wager in losingLedger.Values)
            {
                pot += wager;
            }

            foreach (var wager in winningLedger.Values)
            {
                winningTotal += wager;
            }

            var winners = winningLedger.ToList();
            winners.OrderBy(kvp => kvp.Value); // FIX wrong order
            uint remainingPot = pot;

            foreach (var winner in winners)
            {
                double share = (double)winner.Value / (double)winningTotal;
                double d_profit = share * pot;
                uint profit = (uint)Math.Ceiling(d_profit);

                if(profit > remainingPot) profit = remainingPot;

                remainingPot -= profit;
                accounts[winner.Key].balance += profit; // get share of losing bets
                accounts[winner.Key].balance += winner.Value; // get own stake back
            }

            yesWagers.Clear();
            noWagers.Clear();

            return $"prediction {text} ended. {pot} go to {winners.FirstOrDefault().Key} and others";
        }
    }

    class BankAccount
    {
        public uint balance = 0;
        public object balance_lock = new object();
        public BankAccount(uint balance)
        {
            this.balance = balance;
        }
    }
}