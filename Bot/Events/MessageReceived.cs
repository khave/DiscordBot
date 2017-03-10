using Bot.Responses;
using Discord;
using System;

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
            myBot.discord.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    if (e.Channel.IsPrivate)
                    {
                        if (myBot.getCooldownManager().hasCooldown(e.User.Name + "contactAdmin"))
                        {
                            await e.User.SendMessage("Please only contact the admins every 5 minutes!");
                            return;
                        }
                        myBot.getCooldownManager().addCooldown(e.User.Name + "contactAdmin", 300);

                        foreach (User user in myBot.discord.GetServer(myBot.serverId).Users){
                            if (myBot.hasAdmin(user) && !user.IsBot && user.Status == UserStatus.Online)
                            {
                                try {
                                    await user.SendMessage(e.User.Name + " has contacted an admin with the message: " +
                                        "\n" + e.Message.Text);
                                }catch(Exception ex)//Admin is offline or whatnot
                                {
                                    Console.WriteLine("Failed to write DM to: " + user.Name);
                                }
                            }
                        }
                    }

                    foreach (Response response in myBot.responses)
                    {
                        if (e.User.Name == response.getUser() && Utils.getRandInt(0, 100) < response.getChance())
                        {
                            if(response.getMention() == "")
                            {
                                await e.Channel.SendMessage(e.User.Mention + " " + response.getResponse());
                            }
                            else
                            if (e.Message.Text == response.getMention() || e.Message.Text.ToLower().Contains(response.getMention().ToLower()))
                            {
                                await e.Channel.SendMessage(e.User.Mention + " " + response.getResponse());
                            }
                        }
                    }
                }
            };
        }

    }
}
