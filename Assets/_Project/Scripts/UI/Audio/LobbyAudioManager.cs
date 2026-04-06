using Colosseum.Core;
using UnityEngine;

namespace Colosseum.UI.Audio
{
    public class LobbyAudioManager: Singleton<LobbyAudioManager>
    {
        private static bool HasInstance => Instance != null;
        
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private AudioSource sfxAudioSource;

        [Header("BGM")]
        [SerializeField] private AudioClip authBgm;
        [SerializeField] private AudioClip lobbyBgm;
        [SerializeField] private AudioClip roomBgm;
        
        [Header("SFX")]
        [SerializeField] private AudioClip readySfx;
        [SerializeField] private AudioClip popupOpenSfx;
        [SerializeField] private AudioClip popupCloseSfx;
        [SerializeField] private AudioClip lobbyExitSfx;
        [SerializeField] private AudioClip roomEnterSfx;
        [SerializeField] private AudioClip roomExitSfx;


        public static void PlayBgm(BgmType type)
        {
            if (!HasInstance) return;

            Instance.bgmAudioSource.clip = type switch
            {
                BgmType.Auth => Instance.authBgm,
                BgmType.Lobby => Instance.lobbyBgm,
                BgmType.Room => Instance.roomBgm,
                _ => Instance.authBgm
            };
            Instance.bgmAudioSource.Play();
        }

        public static void PlaySfx(SfxType type)
        {
            if (!HasInstance) return;

            Instance.sfxAudioSource.PlayOneShot(type switch
            {
                SfxType.Ready => Instance.readySfx,
                SfxType.PopupOpen => Instance.popupOpenSfx,
                SfxType.PopupClose => Instance.popupCloseSfx,
                SfxType.LobbyExit => Instance.lobbyExitSfx,
                SfxType.RoomEnter => Instance.roomEnterSfx,
                SfxType.RoomExit => Instance.roomExitSfx,
                _ => Instance.popupOpenSfx
            });
        }
    }
}