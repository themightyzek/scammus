using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace scammus
{
    struct Message
    {
        public string client;
        public string body;
    }

    class Program
    {
        const char PREFIX = '!';
        static string ip = "irc.chat.twitch.tv";
        static int port = 6667;
        static string password = Environment.GetEnvironmentVariable("SCAMMUS_TWITCH_OAUTH");
        static string botUsername = "scammus";
        static string channel = "cutecrait";
        static StreamReader chatIn;
        static StreamWriter chatOut;
        static TcpClient tcpClient = new TcpClient();
        static Scammus scammus = new Scammus();

        static async Task Main(string[] args)
        {
            Task connect = tcpClient.ConnectAsync(ip, port);
            Console.WriteLine("> HACKING NASA ......");
            Console.WriteLine("> DONE ALL MONEY IS MINE ");

            await connect;
            Console.WriteLine("> connected");
            chatIn = new StreamReader(tcpClient.GetStream());
            chatOut = new StreamWriter(tcpClient.GetStream());
            chatOut.AutoFlush = true;
            chatOut.NewLine = "\r\n";

            await chatOut.WriteLineAsync($"PASS {password}");
            await chatOut.WriteLineAsync($"NICK {botUsername}");
            await chatOut.WriteLineAsync($"JOIN #{channel}");

            await chatOut.WriteLineAsync($"PRIVMSG #{channel} :TEST 123 LUUUUL");

            while (true)
            {
                string line = await chatIn.ReadLineAsync();
                HandleMessageAsync(line);
            }
        }

        static async Task HandleMessageAsync(string messageText)
        {
            Console.WriteLine("> " + messageText);

            // pingpong 
            if (messageText.StartsWith("PING "))
            {
                string pongReply = "PONG " + messageText.Substring(5) + " lmao get fucked";
                Console.WriteLine(">>> " + pongReply);
                await chatOut.WriteLineAsync(pongReply);
            }

            // extract info
            var messageParts = messageText.Split(' ', 4);
            if (messageParts[1] != "PRIVMSG")
                return;

            var msg = new Message()
            {
                body = messageParts[3].Remove(0, 1),
                client = messageParts[0].Substring(1, messageParts[0].IndexOf('!') - 1)
            };

            if (!msg.body.StartsWith(PREFIX))
                return;

            // handle it 
            var args = msg.body.Split(' ');
            string cmd = args[0].Remove(0, 1);

            try
            {
                switch (cmd)
                {
                    case "bet":
                        SendMessageAsync(scammus.Bet(args[1], msg.client, args[2]));
                        break;
                    case "prediction":
                        if(msg.client == "themightyzek") 
                            SendMessageAsync(scammus.Prediction(args[1], args[2], args[3]));
                        break;
                    case "result":
                        if(msg.client == "themightyzek")
                            SendMessageAsync(scammus.Result(args[1]));
                        break;
                    case "balance":
                        SendMessageAsync(scammus.Balance(msg.client));
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        static async void SendMessageAsync(string message)
        {
            await chatOut.WriteLineAsync($"PRIVMSG #{channel} :{message}");
            Console.WriteLine($">>> {message}");
        }
    }
}
