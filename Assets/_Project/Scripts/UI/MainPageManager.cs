using Colosseum.Core;
using System.Collections.Generic;
using Colosseum.UI.Audio;
using UnityEngine;
using static P09.Modular.Humanoid.DemoPageController;

namespace Colosseum.UI
{
    public class MainPageManager: Singleton<MainPageManager>
    {
        [SerializeField] private GameObject loadingPage;
        [SerializeField] private GameObject authPage;
        [SerializeField] private GameObject lobbyPage;
        [SerializeField] private GameObject roomPage;


        private Dictionary<MainPageType, GameObject> pages = null;


        void Reset()
        {
            loadingPage = transform.GetChild(0).gameObject;
            authPage = transform.GetChild(1).gameObject;
            lobbyPage = transform.GetChild(2).gameObject;
            roomPage = transform.GetChild(3).gameObject;
        }
        
        protected override void Awake()
        {
            base.Awake();
            pages = new()
            {
                { MainPageType.Loading, loadingPage },
                { MainPageType.Auth, authPage },
                { MainPageType.Lobby, lobbyPage },
                { MainPageType.Room, roomPage },
            };
        }

        void Start()
        {
            ChangePage(MainPageType.Loading);
        }
        

        public void ChangePage(MainPageType pageType)
        {
            foreach (MainPageType type in pages.Keys)
            {
                pages[type].SetActive(type == pageType);
            }

            PlayBGM(pageType);
        }
        
        public void OpenCustomizePage()
        {
            // TODO: 커스터마이징 페이지 오픈
        }

        public void PlayBGM(MainPageType pageType)
        {
            switch (pageType)
            {
                case MainPageType.Auth:
                    LobbyAudioManager.PlayBgm(BgmType.Auth);
                    break;
                case MainPageType.Lobby:
                    LobbyAudioManager.PlayBgm(BgmType.Lobby);
                    break;
                case MainPageType.Room:
                    LobbyAudioManager.PlayBgm(BgmType.Room);
                    break;
            }
        }
    }
}