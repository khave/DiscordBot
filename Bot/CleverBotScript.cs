using System;
using Cleverbot.Net;

namespace Bot
{
    class CleverBotScript
    {

        MyBot myBot;

        public CleverBotScript(MyBot myBot)
        {
            this.myBot = myBot;
            messageReceived();
            Console.WriteLine("Loaded CleverBotScript");
        }

        public void messageReceived()
        {
            var session = CleverbotSession.NewSession("oWBfq3FGSQA8Xo50", "PyQAXcJswt7DsPply9lSq2e1pN0bd1wK");
            myBot.discord.MessageReceived += async (s, e) =>
            { 
                if (!e.Message.IsAuthor && e.Message.IsMentioningMe())
                {
                    string msg = e.Message.Text.Replace("@" + myBot.discord.CurrentUser.Name + " ", "").Replace("!", "");
                    foreach (BotCommand cmd in myBot.commands)
                    {
                        if (msg == cmd.getCommand()) return;
                    }
                    try {
                        await e.Channel.SendIsTyping();
                        await e.Channel.SendMessage(session.Send(msg));
                    }catch(ArgumentException exc)
                    {
                        await e.Channel.SendMessage("Something went wrong with the bot :(");
                    }
                }
            };
        }
    }
}
