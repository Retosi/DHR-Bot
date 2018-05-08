using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

namespace OsuNetCore
{


    public class InfoModule : ModuleBase
    {
        // ~say hello -> hello
        [Command("test"), Summary("Test erfolgreich.")]
        public async Task Say()
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(":ok_hand:");
        }
    }

    public class recentPlays : ModuleBase
    {
        [Command("recent"), Summary("Letzten 10 osu Plays")]
        public async Task Recent([Remainder, Summary("")] string user)
        {
            string response = OsuWeebShit.GetRecentPlaysByUser(user);
            await ReplyAsync(response);

        }
    }

    public class joinMe : ModuleBase<ICommandContext>
    {
        private IVoiceChannel targetChannel;
        private IAudioClient client;


        //private readonly AudioSAervice
        [Command("join", RunMode = RunMode.Async), Summary("Tritt Voicechannel bei")]
        public async Task join()
        {
            targetChannel = (Context.User as IVoiceState).VoiceChannel;
            IAudioClient audioClient = await targetChannel.ConnectAsync();
            await ReplyAsync(audioClient.ConnectionState.ToString());
        }

        [Command("play")]
        public async Task playAudio(string path)
        {
            using (var ffmpeg = CreateStream(path))
            using (var stream = client.CreatePCMStream(AudioApplication.Music))
            {
                try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                finally { await stream.FlushAsync(); }
            }

        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = @"/bin/Debug/ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }

    public class spectateMe : ModuleBase
    {
        [Command("spec"), Summary("Zuschauen")]
        public async Task specMe()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Klick auf den Link um Retosi zuzuschauen!",
                Description = "<osu://spectate/Retosi>",
                //Url = "https://www.google.de",
                ThumbnailUrl = "https://store-images.s-microsoft.com/image/apps.34947.9007199266250531.1c07b79d-376b-446a-8f50-ebeb9867dae6.5b1c5498-f32a-4498-97a8-e64aa0cc70d2?w=180&h=180&q=60"
            };
            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }

    // Create a module with the 'sample' prefix
    [Group("boi")]
    public class Sample : ModuleBase
    {
        // ~sample square 20 -> 400
        [Command("square"), Summary("Squares a number.")]
        public async Task Square([Summary("The number to square.")] int num)
        {
            // We can also access the channel from the Command Context.
            await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
        }

        // ~sample userinfo --> foxbot#0282
        // ~sample userinfo @Khionu --> Khionu#8708
        // ~sample userinfo Khionu#8708 --> Khionu#8708
        // ~sample userinfo Khionu --> Khionu#8708
        // ~sample userinfo 96642168176807936 --> Khionu#8708
        // ~sample whois 96642168176807936 --> Khionu#8708
        [Command("userinfo"), Summary("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfo([Summary("The (optional) user to get info for")] Discord.IUser user = null)
        {
            var userInfo = user ?? Context.Client.CurrentUser;
            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
        }
    }
}
