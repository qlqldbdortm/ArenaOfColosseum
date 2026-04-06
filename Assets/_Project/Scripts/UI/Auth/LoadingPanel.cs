using System;
using System.Threading;
using Colosseum.Authentication;
using Colosseum.Network;
using Colosseum.Network.Lobby;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Colosseum.UI.Auth
{
    public class LoadingPanel: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI loadingText;


        private CancellationTokenSource token = null;
        
        
        void OnEnable()
        {
            AuthManager.OnConnectMessage += WriteMessage;
            _ = LoadingAsync();
        }
        void OnDisable()
        {
            AuthManager.OnConnectMessage -= WriteMessage;
            token?.Cancel();
            token = null;
        }


        private void WriteMessage(string message)
        {
            token?.Cancel();
            token = null;
            
            loadingText.text = message;
        }

        private async UniTask LoadingAsync()
        {
            token = new();
            _ = CheckNetworkAsync();
            while (true)
            {
                loadingText.text = "Loading";
                for (int i = 0; i < 3; i++)
                {
                    loadingText.text += ".";
                    await UniTask.WaitForSeconds(0.25f, cancellationToken: token.Token);
                }
            }
            token = null;
        }

        private async UniTask CheckNetworkAsync()
        {
            await UniTask.WaitUntil(() => AuthManager.Instance != null);
            await UniTask.WaitUntil(() => AuthManager.Instance.IsInitialized);

            print("로딩 완료");
            if (!GameDataManager.Instance.IsLoggedIn)
            {
                MainPageManager.Instance.ChangePage(MainPageType.Auth);
            }
            else
            {
                await UniTask.WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
                MainPageManager.Instance.ChangePage(MainPageType.Lobby);
            }
        }
    }
}