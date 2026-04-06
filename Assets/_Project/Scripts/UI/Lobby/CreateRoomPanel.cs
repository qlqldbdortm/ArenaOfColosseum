using Colosseum.UI.Audio;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Lobby
{
    public class CreateRoomPanel : PopupBase
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject createPanelUI;
        [SerializeField] private TMP_InputField roomNameInput;
        [SerializeField] private Button createAcceptButton;
        [SerializeField] private Button cancelButton; // 취소 버튼 추가

        private void Awake()
        {
            createAcceptButton.onClick.AddListener(OnCreateAcceptButtonClick);
            cancelButton.onClick.AddListener(ClosePopupAndResetFocus); // 취소 버튼에 닫기 기능 연결

            // 팝업이 열렸을 때 기본 포커스를 입력 필드로 설정
            firstSelectableUI = roomNameInput.gameObject;
        }

        private void OnCreateAcceptButtonClick()
        {
            string roomName = roomNameInput.text;
            if (string.IsNullOrWhiteSpace(roomName))
            {
                // DialogMessage는 이미 팝업이므로, 이 팝업을 닫지 않고 그 위에 띄웁니다.
                DialogMessage.ShowMessage("방 제목은 공백이 될 수 없습니다.");
                return;
            }

            RoomOptions option = new RoomOptions()
            {
                MaxPlayers = 8
            };
            PhotonNetwork.CreateRoom(roomName, option);

            // 방 생성을 시도한 후, 이 팝업은 닫습니다.
            ClosePopupAndResetFocus();
        }
        
        #region PopupBase Overrides

        public override void OnPopupOpened()
        {
            LobbyAudioManager.PlaySfx(SfxType.PopupOpen);
            base.OnPopupOpened();
            roomNameInput.text = ""; // 팝업이 열릴 때마다 입력 필드 초기화
            createPanelUI.SetActive(true);
        }

        public override void OnPopupClosed()
        {
            LobbyAudioManager.PlaySfx(SfxType.PopupClose);
            base.OnPopupClosed();
            createPanelUI.SetActive(false);
        }

        #endregion
    }
}
