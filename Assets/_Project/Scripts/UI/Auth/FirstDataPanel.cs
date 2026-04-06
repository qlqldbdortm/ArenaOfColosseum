using Colosseum.Customizing;
using Colosseum.Network;
using Colosseum.Utility;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Colosseum.UI.Auth
{
    public class FirstDataPanel: MonoBehaviour
    {
        [Tooltip("닉네임")] [SerializeField] private TMP_InputField nickInput;
        [Tooltip("커스터마이징 버튼")] [SerializeField] private Button confirmButton;


        void Reset()
        {
            nickInput = GetComponentInChildren<TMP_InputField>();
            confirmButton = GetComponentInChildren<Button>();
        }


        void Awake()
        {
            confirmButton.onClick.AddListener(OnCustomButton);
        }


        private void OnCustomButton()
        {
            ChangeInteractable(false);
            DataManager.Instance.ClearData();
            string nick = nickInput.text;
            _ = StringChecker.HasFineNicknameAsync(nick, OnSetNicknameCallback);
        }
        private void OnSetNicknameCallback(string nick, string errorMessage)
        {
            ChangeInteractable(true);

            if (string.IsNullOrEmpty(errorMessage)) // 닉네임 등록 성공
            {
                // DialogMessage.ShowMessage("닉네임 등록 성공", "닉네임 설정 성공"); // 닉네임 등록은 성공을 굳이 표시할 이유가 없음
                nickInput.text = string.Empty;

                _ = AuthManager.Instance.ChangeNicknameAsync(nick, OnNicknameChanged);
            }
            else // 닉네임 등록 실패
            {
                DialogMessage.ShowMessage("닉네임 등록 실패", errorMessage);
            }
        }

        private void OnNicknameChanged(string errorMessage)
        {
            ChangeInteractable(true);
            
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                AuthUIManager.Instance.CloseAll();
                PhotonNetwork.JoinLobby();
            }
            else
            {
                DialogMessage.ShowMessage("닉네임 등록 실패", errorMessage);
            }
        }

        
        private void ChangeInteractable(bool isOn)
        {
            nickInput.interactable = confirmButton.interactable = isOn;
        }
    }
}