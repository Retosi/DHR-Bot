using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace OsuNetCore
{
    public class Program
    {
        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 100,
            });
            //client.Log += Log;
            
            commands = new CommandService();
            //commands.AddModulesAsync();
            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            string token = "NDQxOTA5OTAwODY0Mzg5MTIw.Dc8r6g.cUi3r3jDn4tb2f8UNBgQODGGuq4";
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            client.MessageUpdated += MessageUpdated;
            client.Ready += () =>
            {
                Console.WriteLine("Bot ist verbunden");
                return Task.CompletedTask;
            };
            await Task.Delay(-1);
        }

        private async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            //return Task.CompletedTask;
        }

        public string GetChannelTopic(ulong id)
        {
            //var channel = )
            return String.Empty;
        }
    }
}
