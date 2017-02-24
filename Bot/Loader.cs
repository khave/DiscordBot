using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Bot
{
    class Loader
    {

        private int loadPercent;
        private Message message;
        private System.Timers.Timer timer;
        private Task<Message> task;
        private onDone.onDoneEventHandler onDoneEventHandler;

        public Loader(Message message)
        {
            this.loadPercent = 0;
            this.message = message;   

            timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_tick);
            timer.Interval = 1000; // in miliseconds
            timer.Start();
        }

        public void addPercent(int percent)
        {
            loadPercent += percent;
        }

        public void setPercent(int percent)
        {
            this.loadPercent = percent;
        }

        private void timer_tick(object sender, EventArgs e)
        {
            updateTimer();
        }

        public void updateTimer()
        {
            if (loadPercent >= 100)
            {
                message.Edit("Done! " + loadPercent + "%");
                timer.Stop();
                timer.Close();
                timer.Dispose();
                return;
            }

            message.Edit("Loading... " + loadPercent + "%");
        }

        public void onDone(onDone.onDoneEventHandler onDone)
        {
            this.onDoneEventHandler = onDone;
        }
    }



    class onDone
    {

        public interface onDoneEventHandler
        {
            void onDone(onDone onDone);
        }

        private Message message;

        public onDone(Message message)
        {
            this.message = message;
        }

        public Message getMessage()
        {
            return message;
        }
        
    }
}
