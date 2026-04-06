using Colosseum.Core;
using UnityEngine;

namespace Colosseum.UI.Audio
{
    public class InGameAudioManager: Singleton<InGameAudioManager>
    {
        private static bool HasInstance => Instance != null;
        
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxAudioSource;
        
        [Header("SFX")]
        [SerializeField] private AudioClip emoteSfx;
        

        public static void PlaySfx(SfxType type)
        {
            if (!HasInstance) return;

            Instance.sfxAudioSource.PlayOneShot(type switch
            {
                SfxType.Emote => Instance.emoteSfx,
                _ => Instance.emoteSfx
            });
        }
    }
}