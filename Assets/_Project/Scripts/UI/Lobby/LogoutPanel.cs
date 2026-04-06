using Colosseum.Network;
using Colosseum.UI.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Lobby
{
    public class LogoutPanel : PopupBase
    {
        [Header("UI 오브젝트")]
        [Tooltip("활성화/비활성화할 팝업 UI 패널")]
        public GameObject logoutPanelUI;

        [Tooltip("로비창의 로그아웃 버튼")]
        public Button logoutButton;
        
        [Header("버튼")]
        [Tooltip("로그아웃을 최종 확인하는 '예' 버튼")]
        public Button confirmButton;

        [Tooltip("팝업을 닫는 '아니오' 버튼")]
        public Button cancelButton;

        private void Awake()
        {
            // '예' 버튼을 누르면 ConfirmLogout 메서드 실행
            confirmButton.onClick.AddListener(ConfirmLogout);

            // '아니오' 버튼을 누르면 포커스 리셋과 함께 팝업 닫기
            cancelButton.onClick.AddListener(CancelLogout);

            // 이 팝업이 열렸을 때 기본으로 포커스될 버튼을 '아니오' 버튼으로 
            firstSelectableUI = cancelButton.gameObject;
            
            logoutButton.onClick.AddListener(() => PopupManager.Instance.OpenPopup(this));
        }

        /// <summary>
        /// '예' 버튼을 눌렀을 때 실행될 로직
        /// </summary>
        private void ConfirmLogout()
        {
            _ = AuthManager.Instance.Logout(OnLogoutCallback);
            ClosePopupAndResetFocus();
        }

        private void CancelLogout()
        {
            LobbyAudioManager.PlaySfx(SfxType.PopupClose);
            ClosePopupAndResetFocus();
        }

        private void OnLogoutCallback(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                DialogMessage.ShowMessage("로그아웃 실패", $"로그아웃 중 오류 발생.\n{errorMessage}");
            }
        }

        #region PopupBase 오버라이드

        public override void OnPopupOpened()
        {
            base.OnPopupOpened();
            logoutPanelUI.SetActive(true); // 팝업이 열릴 때 UI 활성화
            LobbyAudioManager.PlaySfx(SfxType.PopupOpen);
        }

        public override void OnPopupClosed()
        {
            base.OnPopupClosed();
            logoutPanelUI.SetActive(false); // 팝업이 닫힐 때 UI 비활성화
        }

        #endregion
    }
}
