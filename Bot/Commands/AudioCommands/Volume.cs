using System;
using Discord;
using Discord.Commands;

namespace Bot.Commands
{
    class Volume : BotCommand
    {

        MyBot myBot;

        public Volume(MyBot myBot) : base("music volume", "Turn the volume up/down", "music volume <0-100>", true, new string[] { "volume", "v" })
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            int vol;
            try
            {
                vol = Convert.ToInt32(args[0]);

                if (!(vol >= 1 && vol <= 100))
                {
                    e.Channel.SendMessage("Must be between 1-100!");
                    return;
                }

                myBot.audioManager.setVolume(vol);
                e.Channel.SendMessage("Set the volume to " + vol + " (" + ((double) vol/100) + ")");
            }
            catch (FormatException ex)
            {
                e.Channel.SendMessage("That's not a number!");
            }
        }
    }
}
