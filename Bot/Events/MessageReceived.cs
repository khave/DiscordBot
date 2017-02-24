using Bot.Responses;

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
