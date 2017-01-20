using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Bot.Responses
{
    class Response
    {

        User user;
        private string mention;
        private string response;

        public Response(User user, string mention, string response)
        {
            this.user = user;
            this.mention = mention;
            this.response = response;
        }

        public User getUser()
        {
            return user;
        }

        public string getMention()
        {
            return mention;
        }

        public string getResponse()
        {
            return response;
        }

        public string toString()
        {
            return user.Id + ";" + mention + ";" + response;
        }

        public Response fromString(Channel channel, String responseString)
        {
            string[] response = responseString.Split(';');
            return new Response(channel.GetUser(ulong.Parse(response[0])), response[1], response[2]);
        }
    }
}
