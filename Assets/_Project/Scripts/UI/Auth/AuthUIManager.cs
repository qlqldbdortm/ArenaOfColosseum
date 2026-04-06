using Colosseum.Authentication;
using Colosseum.Core;
using UnityEngine;

namespace Colosseum.UI.Auth
{
    public class AuthUIManager: Singleton<AuthUIManager>
    {
        [Tooltip("회원가입 패널")] [SerializeField] private RegisterPanel registerPanel;
        [Tooltip("정보입력 패널")] [SerializeField] private FirstDataPanel firstDataPanel;


        void Reset()
        {
            registerPanel = GameObject.FindObjectOfType<RegisterPanel>();
            firstDataPanel = GameObject.FindObjectOfType<FirstDataPanel>();
        }

        protected override void Awake()
        {
            base.Awake();
            registerPanel.gameObject.SetActive(false);
            firstDataPanel.gameObject.SetActive(false);
        }
        void Start()
        {
            if (GameDataManager.Instance?.IsLoggedIn ?? false)
            {
                MainPageManager.Instance.ChangePage(MainPageType.Lobby);
            }
        }


        public void OpenRegisterPanel()
        {
            registerPanel.gameObject.SetActive(true);
            firstDataPanel.gameObject.SetActive(false);
        }
        public void OpenFirstDataPanel()
        {
            registerPanel.gameObject.SetActive(false);
            firstDataPanel.gameObject.SetActive(true);
        }
        public void CloseAll()
        {
            registerPanel.gameObject.SetActive(false);
            firstDataPanel.gameObject.SetActive(false);
        }
    }
}