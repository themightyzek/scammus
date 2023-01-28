using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace scammus
{
    public struct SubGoal
    {
        public float Points;
        public string Name;
    }

    public static class Streamlabs
    {
        const string logfile = "birthday.log";
        const string popufile = "birthday.popu";
        public static List<SubGoal> Subgoals = new List<SubGoal>()
        {
            new SubGoal{Points = 10, Name = "LoL Queue Challenges"},
            new SubGoal{Points = 25, Name = "Neues Poro-Emote zeichnen"},
            new SubGoal{Points = 50, Name = "No-Face Cosplay Stream"},
            new SubGoal{Points = 75, Name = "Poromützenzeit"},
            new SubGoal{Points = 100, Name = "Red Bull-Verkostung"},
            new SubGoal{Points = 125, Name = "Viper-Cosplay"},
            new SubGoal{Points = 150, Name = "LoL 1v1 Stream"},
            new SubGoal{Points = 175, Name = "Einhorn-Anzug anziehen"},
            new SubGoal{Points = 200, Name = "MC TwitchSpawns Stream, aber alles ist free"},
            new SubGoal{Points = 225, Name = "10 min ASMR"},
            new SubGoal{Points = 250, Name = "Spicy Noodle Challenge"},
            new SubGoal{Points = 275, Name = "Spicy Noodle Challenge mit Zek"},
            new SubGoal{Points = 300, Name = "Schnurrbart aufmalen"},
            new SubGoal{Points = 325, Name = "10 min flirten"},
            new SubGoal{Points = 350, Name = "D&D DM Starter Kit"},
            new SubGoal{Points = 400, Name = "In Limette beißen (mit Schale)"},
            new SubGoal{Points = 450, Name = "Dead by Daylight Stream"},
            new SubGoal{Points = 681, Name = "2 Wochen Mo-Fr 8h Stream"},
        };
        static float totalsubpoints = 0;
        static TimeSpan initialTimer = new TimeSpan(6, 0, 0);
        static TimeSpan maxTimer = new TimeSpan(14, 0, 0);
        static DateTime starttime = new DateTime(2023, 1, 29, 12, 0, 0);


        public static string Initialize()
        {
            try
            {
                using (var stream = File.OpenText(popufile))
                {
                    string file = stream.ReadToEnd();
                    totalsubpoints = float.Parse(file);
                }

                return $"Wir starten mit {totalsubpoints.ToString("n2")} Poropunkten! Porosnack";
            }
            catch (System.Exception)
            {
                return "Uff. Hier ist kein save. Sadge";
            }
        }


        public static List<string> HandleMessage(Message msg)
        {
            var oldTotal = totalsubpoints;
            var answers = new List<string>();
            var messageParts = msg.body.Split(' ');
            string donotype = messageParts[messageParts.Length - 6];

            float subpoints = 0;
            string answer = "";

            switch (donotype)
            {
                case "Dono":
                    if (messageParts[2].StartsWith("€"))
                    {
                        var amountd = decimal.Parse(messageParts[2].TrimStart('€'));
                        subpoints = (float)(amountd / 1.47m);
                        break;
                    }
                    else
                    {
                        return new List<string> { "Ich bin der einzige Scammer hier, danke für die dono tho LUL" };
                    }

                case "Bits":
                    var amountb = Int32.Parse(messageParts[2]);
                    subpoints = amountb / 147f;
                    break;

                case "Sub":
                    if (messageParts[2].EndsWith("1")) subpoints = 1;
                    else if (messageParts[2].EndsWith("2")) subpoints = 2;
                    else subpoints = 5;
                    break;

                case "Subs":
                    if (messageParts[3].EndsWith("1")) subpoints = Int32.Parse(messageParts[2]);
                    else if (messageParts[3].EndsWith("2")) subpoints = Int32.Parse(messageParts[2]) * 2;
                    else subpoints = Int32.Parse(messageParts[2]) * 5;
                    break;

                default:
                    break;
            }

            totalsubpoints += subpoints;

            TryLog($"add {subpoints} | total {totalsubpoints} | sender {msg.client} | message {msg.body}");
            TrySavePoints();

            answer = "+" + subpoints.ToString("n2") + " Poropunkte LETSGOO";
            answers.Add(answer + " " + SendTime());
            answers.AddRange(CheckSubGoals(oldTotal, totalsubpoints));
            return answers;
        }

        public static string SendTime()
        {
            string answer = "";

            var currentTimer = initialTimer + (totalsubpoints * new TimeSpan(0, 2, 0));
            currentTimer = currentTimer < maxTimer ? currentTimer : maxTimer;
            var currentEndTime = starttime + currentTimer;
            var remainingTime = currentEndTime - DateTime.Now;
            answer = $"Ende: {currentEndTime.ToShortTimeString()} Uhr, also noch {remainingTime.ToString(@"hh\:mm")} Stunden wideVIBE";
            return answer;
        }

        public static List<string> CheckSubGoals(float oldTotal, float newTotal)
        {
            var answers = new List<string>();
            foreach (var goal in Subgoals)
                if (goal.Points > oldTotal && goal.Points <= newTotal)
                    answers.Add($"LETSGO Wir haben das Goal \"{goal.Name}\" erreicht! EZ Clap Stonkers WIDEGIGACHAD");

            var closestGoal = Subgoals.Where(sg => sg.Points > newTotal).OrderBy(sg => sg.Points).FirstOrDefault();
            answers.Add($"Schon {totalsubpoints.ToString("n2")} Punkte! rammusrollin Noch {(closestGoal.Points - newTotal).ToString("n2")} Poropunkte zum nächsten Goal: {closestGoal.Name} STONKS");

            return answers;
        }

        public static void TryLog(string message)
        {
            try
            {
                File.AppendAllLines(logfile, new List<string> { $"{DateTime.Now.ToString()} ||| {message}" });
            }
            catch (System.Exception)
            {

            }
        }

        public static void TrySavePoints()
        {
            try
            {
                File.WriteAllText(popufile, totalsubpoints.ToString());
            }
            catch (System.Exception)
            {

            }
        }
    }
}