using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Colosseum.UI.InGame
{
    public class RoundPanel: MonoBehaviour
    {
        public GameObject roundPanel;
        public GameObject victoryPanel;
        public GameObject defeatPanel;
        
        
        private static RoundPanel Instance { get; set; } = null;
        
        
        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            Instance = null;
        }


        public static void ShowRoundPanel()
        {
            if (Instance is null) return;
            
            Instance.roundPanel.SetActive(true);
            
            _ = HidePanels();
        }
        public static void ShowVictoryPanel()
        {
            if (Instance is null) return;

            Instance.victoryPanel.SetActive(true);
            
            _ = HidePanels();
        }
        public static void ShowDefeatPanel()
        {
            if (Instance is null) return;

            Instance.defeatPanel.SetActive(true);
            
            _ = HidePanels();
        }

        private static async UniTask HidePanels()
        {
            await UniTask.WaitForSeconds(1f);
            
            Instance.roundPanel.SetActive(false);
            Instance.victoryPanel.SetActive(false);
            Instance.defeatPanel.SetActive(false);
        }
    }
}