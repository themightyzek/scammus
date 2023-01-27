using System;
using System.Text.RegularExpressions;
namespace scammus
{

    public static class Streamlabs
    {
        public static string HandleMessage(Message msg)
        {
            string regex = @"\w+(?=\s+to the Timer\^\^)";
            var donotype = Regex.Match(msg.body, regex);

            string answer = "";

            switch (donotype.Value)
            {
                case "Tip":
                 answer = "I added the tip @Streamlabs";
                 break;
                case "Bits":
                 break;
                case "Sub":
                 break;
                case "Subs":
                 break;

                default:
                 break;
            }
            return answer;
        }
    }
}