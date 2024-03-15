using Plugin.SimpleAudioPlayer;
using System;
using System.Reflection;

namespace AutoRemis.Helpers
{
    public static class SoundHelper
    {
        public enum SoundType { Error, Alert, Success, NewTrip, Message }

        private static ISimpleAudioPlayer player;
        private static bool isPlaying = false;
        private static object lockObject = new object();

        public static void PlaySound(SoundType type)
        {
            if (isPlaying)
                return;

            lock (lockObject)
            {
                try
                {
                    player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();

                    var assembly = typeof(App).GetTypeInfo().Assembly;

                    player.Load(assembly.GetManifestResourceStream($"AutoRemis.Common.Sounds.{type}.mp3"));

                    player.Play();

                    isPlaying = true;
                    player.PlaybackEnded += Player_PlaybackEnded;
                }
                catch (Exception) { }
            }
        }

        public static void StopCurrentSound()
        {
            if (player != null && isPlaying)
            {
                player.Stop();
                isPlaying = false;
                player.PlaybackEnded -= Player_PlaybackEnded;
            }
        }

        private static void Player_PlaybackEnded(object sender, EventArgs e)
        {
            isPlaying = false;
            player.PlaybackEnded -= Player_PlaybackEnded;
        }
    }
}
