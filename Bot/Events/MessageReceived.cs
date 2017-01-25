using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Events
{
    class MessageReceived
    {

        MyBot myBot;

        public MessageReceived(MyBot myBot)
        {
            this.myBot = myBot;
            messageReceived();
        }

        public void messageReceived()
        {
            /*
            myBot.discord.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                {

                    if (e.Message.IsMentioningMe() || e.Message.Text.StartsWith("!"))
                    {
                        string msg = e.Message.Text.Replace("@" + myBot.discord.CurrentUser.Name + " ", "").Replace("!", "");
                        foreach (BotCommand cmd in myBot.commands)
                        {    
                            //Check aliases
                            if(cmd.getAliases() != null)
                            {
                                foreach(string alias in cmd.getAliases())
                                {
                                    if (alias.StartsWith(cmd.getCommand()))
                                    {
                                        string[] args = e.Message.Text.Replace(cmd.getCommand(), "").Replace("!", "").Split(null);
                                        cmd.onCommand(e, myBot.discord, args);
                                        return;
                                    }
                                }
                            }

                            if (msg.StartsWith(cmd.getCommand()))
                            {
                                cmd.onCommand(e, myBot.discord, new string[] { });
                                await e.Channel.SendMessage("test");
                            }
                        }
                    }
                }
            };
        }
        */
        }
    }
}
