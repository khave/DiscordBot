using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Commands;
using System.Threading;

namespace Bot.Commands.AudioCommands
{
    class MusicShuffle : BotCommand
    {

        MyBot myBot;
        private Random rand = new Random();


        public MusicShuffle(MyBot myBot) : base("music shuffle", "Shuffle the music queue", "!music shuffle", true)
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            var list = myBot.audioManager.queue.ToList();
            list.Shuffle();
            myBot.audioManager.setQueue(new Queue<YouTubeVideo>(list));
            e.Channel.SendMessage("Shuffled the queue!");
        }

        private List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

    static class MyExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
    
