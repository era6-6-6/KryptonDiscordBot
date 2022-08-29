using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


namespace KryptonBot;

public class Program
{
    static void Main(string[] args)
        =>
        new Program()
        .RunBotAsync()
        .GetAwaiter()
        .GetResult();




    public static DiscordSocketClient _client;
    private CommandService _commands;
    public IServiceProvider _services;

    public async Task RunBotAsync()
    {
        _client = new DiscordSocketClient();
        _commands = new CommandService();

        //_services = (IServiceProvider)new ServiceCollection()
        //    .AddSingleton(_client)
        //    .AddSingleton(_commands);
            

        #region token
        string token = "MTAxMzkyNDM1MzA3Njg4NzcxMw.G8H5mY.z7IUiyAx97PbBgPQED17-t9yBxm_1fQbvCQmtI"; 
        #endregion

        _client.Log += _client_Log;
        await RegisterCommandsAsync();

        await _client.LoginAsync(TokenType.Bot, token);

        await _client.StartAsync();
        await _client.SetStatusAsync(UserStatus.DoNotDisturb);

        await _client.SetGameAsync("Watching Krypton");


        await Task.Delay(-1);
    }

    private Task _client_Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask;
    }

    public async Task RegisterCommandsAsync()
    {

        
        _client.MessageReceived += HandleCommandAsync;
        _client.MessageReceived += BugReport;
        _client.MessageReceived += RemoveFromBugs;

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);


    }

    private async Task BugReport(SocketMessage arg)
    {
        if (arg.Channel.Id == 913416470271242253 & !arg.Author.IsBot)
        {
            var guild =  _client.Guilds.FirstOrDefault(x => x.Id == 913416470271242250);
            var channel = guild.GetTextChannel(913416470271242253);
            await channel.SendMessageAsync("Thank you for your report your message was send to developers!");
            var DevChannel = guild.GetTextChannel(913416528727261225);
            var role = guild.GetRole(1013927590106501222);
            await DevChannel.SendMessageAsync(arg.Author.Username + $" reported new bug!\n\n{arg.Content}\n\nplease check it! " + role.Mention);

            var channelBugs = guild.GetTextChannel(1013930948632510494);

            var message = channelBugs.GetMessageAsync(1013931473360932874);
          
            var messageToChange = message.Result as IUserMessage;
            await messageToChange.ModifyAsync(x => x.Content = $"{message.Result.Content}\n-{arg.Content}");
            
        }
    }

    private async Task RemoveFromBugs(SocketMessage arg)
    {
        if(arg.Channel.Id != 1013930948632510494) return;
        if(!arg.Content.StartsWith("-")) return;
        var guild = _client.Guilds.FirstOrDefault(x => x.Id == 913416470271242250);
        
        
        
        

        var channelBugs = guild.GetTextChannel(1013930948632510494);

        var message = channelBugs.GetMessageAsync(1013931473360932874);

        var messageToChange = message.Result as IUserMessage;
        string messageNew = messageToChange.Content.Replace(arg.Content.Replace("-" ,"") , $"");
        
        await messageToChange.ModifyAsync(x => x.Content = messageNew);

        var channel = guild.GetTextChannel(1013938949368066119);
        var prepatch = await channel.GetMessageAsync(1013939374985064459);
        var prep = prepatch as IUserMessage;
        await prep.ModifyAsync(x => x.Content = $"\n{ prepatch.Content + "\n" + arg.Content.Replace("-", "")}");




    }


    private async Task HandleCommandAsync(SocketMessage arg)
    {
        try
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("+", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                if (result.Error.Equals(CommandError.UnmetPrecondition)) await message.Channel.SendMessageAsync(result.ErrorReason);
            }

        }
        catch { };


    }
}
