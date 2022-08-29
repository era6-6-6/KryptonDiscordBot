using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace KryptonDiscordBot.Commands
{
    public class BasicCommands : ModuleBase<SocketCommandContext>
    {

        [Command("Ban")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Ban(IGuildUser user)
        {
            await ReplyAsync($"Sended by {user.DisplayName}");
        }
        [Command("clean")]
        public async Task Clean(int max)
        {

            var messages = Context.Channel.GetMessagesAsync(max).Flatten();
            foreach (var h in await messages.ToArrayAsync())
            {
                await this.Context.Channel.DeleteMessageAsync(h);
            }
        }
        [Command("Info")]
        public async Task Info(IGuildUser user)
        {
            string perms = "";
            var userPerms = user.GuildPermissions.ToList().ToArray();
            var createdAt = user.CreatedAt.ToString();
            var joinedAt = user.JoinedAt.ToString();
            var premiumSince = user.PremiumSince.ToString();
            if (premiumSince == null || premiumSince == "")
            {
                premiumSince = "No boost";
            }
            else
            {
                premiumSince = premiumSince.Substring(0, premiumSince.IndexOf("+"));

            }
            string isBot = "";
            if (user.IsBot == false)
            {
                isBot = "No";
            }
            else
            {
                isBot = "Yes";
            }
            joinedAt = joinedAt.Substring(0, joinedAt.IndexOf('+'));
            createdAt = createdAt.Substring(0, createdAt.IndexOf("+"));
            for (int i = 0; i < userPerms.Length; i++)
            {
                perms += userPerms[i] + ",";
            }
            perms = perms.Substring(0, perms.LastIndexOf(","));
            var embed = new EmbedBuilder();
            embed.WithTitle($"Info about: {user.Username}");
            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.WithDescription($"Status: {user.Status}"
                                  + $"\n\nuser ID: {user.Id}"
                                  + $"\n\nAccount Created: {createdAt}"
                                  + $"\n\nJoined: {joinedAt}"
                                  + $"\n\nBooster from: {premiumSince}"
                                  + $"\n\nBot: {isBot}"
                                  + $"\n\nPermissions: {perms}");
            embed.WithCurrentTimestamp();
            await ReplyAsync("", false, embed.Build());
        }
        [Command("meme")]
        public async Task Meme()
        {

            var client = new HttpClient();
            var result = await client.GetStringAsync("https://reddit.com/r/memes/random.json?limit=1");
            JArray arr = JArray.Parse(result);
            JObject post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());
            await ReplyAsync(post["url"].ToString());



        }


    }

    
}
