using System.Threading;
using Colosseum.Core;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Colosseum.UI.Room
{
    public class ReadyTimerPanel: Singleton<ReadyTimerPanel>
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI timerText;


        private float timer = 0;
        private CancellationTokenSource token = null;


        protected override void OnDestroy()
        {
            token?.Cancel();
            base.OnDestroy();
        }


        public static void StopTimer()
        {
            if (Instance == null) return;
            
            Instance.panel.SetActive(false);
            Instance.token?.Cancel();
        }
        public static void StartTimer(float seconds)
        {
            if (Instance == null) return;

            Instance.panel.SetActive(true);
            _ = Instance.StartTimerAsync(seconds);
        }
        

        private async UniTask StartTimerAsync(float seconds)
        {
            token?.Cancel();
            token = new();
            timer = seconds;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                timerText.text = $"{timer:0.0}s";
                await UniTask.Yield(token.Token);
            }
            timerText.text = $"{0:0.0}s";
            token = null;
        }
    }
}