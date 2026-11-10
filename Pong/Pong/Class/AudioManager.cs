using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace Class
{
    public class AudioManager
    {
        private readonly Dictionary<string, SoundEffect> _sounds = new();
        public void LoadSound(ContentManager content, string name)
        {
            _sounds[name] = content.Load<SoundEffect>(name);
        }
        public void PlaySound(string name)
        {
            if (_sounds.TryGetValue(name, out SoundEffect sound))
            {
                sound.Play();
            }
        }
        public void PlayMusic(Song song, bool loop = true)
        {
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Play(song);
        }
        public void SetMusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }
    }
}
