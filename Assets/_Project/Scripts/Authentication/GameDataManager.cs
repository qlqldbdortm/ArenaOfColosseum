using System;
using Colosseum.Core;
using Colosseum.Network;
using Firebase.Database;
using Photon.Pun;

namespace Colosseum.Authentication
{
    public class GameDataManager : Singleton<GameDataManager>
    {
        public FirebaseDatabase DB { get; private set; }
        public bool IsLoggedIn { get; private set; } = false;
        public UserData PlayerData
        {
            get => playerData;
            set
            {
                playerData = value;
                IsLoggedIn = value != null;
                if (IsLoggedIn)
                {
                    PhotonNetwork.SetPlayerCustomProperties(new() { { PropName.UID, value.Uid } });
                }
            }
        }
        
        private bool _isSubscribed = false;
        private UserData playerData = null;

        
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        
        public void UpdateTotalMatches(bool isWin) //승리 수 기록하는 로직 //나중에 게임 종료시 불러오세요
        {
            //Fire-and-Forget 패턴 //행동해놓고 잊어버림
            UpdateMatchesAsync(isWin);
        }

        private async void UpdateMatchesAsync(bool isWin)
        {
            try
            {
                PlayerData.totalMatches++;
                if (isWin) PlayerData.wins++;
                await PlayerData.SaveAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Start()
        {
            if (AuthManager.Instance != null && AuthManager.Instance.IsInitialized)
            {
                InitializeDatabase();
            }
            else
            {
                SubscribeToFirebaseInit();
            }
        }
        
        #region FireBase초기화 이벤트
        void SubscribeToFirebaseInit()
        {
            if (!_isSubscribed && AuthManager.Instance != null)
            {
                AuthManager.Instance.OnFirebaseInitialized += InitializeDatabase;
                _isSubscribed = true;
            }
        }
        void InitializeDatabase()
        {
            DB = FirebaseDatabase.DefaultInstance;

            if (_isSubscribed && AuthManager.Instance != null)
            {
                AuthManager.Instance.OnFirebaseInitialized -= InitializeDatabase;
                _isSubscribed = false;
            }
        }
        #endregion

    }
}
