using System;
using Colosseum.Network;
using Colosseum.UI.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Auth
{
    public class RegisterPanel: MonoBehaviour
    {
        [Header("Input Fields")]
        [Tooltip("ID")] [SerializeField] private TMP_InputField emailInput;
        [Tooltip("비밀번호")] [SerializeField] private TMP_InputField passwordInput;
        [Tooltip("비밀번호 확인")] [SerializeField] private TMP_InputField confirmPasswordInput;


        [Header("Buttons")]
        [Tooltip("회원가입 버튼")] [SerializeField] private Button registerButton;
        [Tooltip("닫기 버튼")] [SerializeField] private Button closeButton;


        private Func<bool>[] inputCheckers = null;


        void Reset()
        {
            TMP_InputField[] inputs = GetComponentsInChildren<TMP_InputField>();
            emailInput = inputs[0];
            passwordInput = inputs[1];
            confirmPasswordInput = inputs[2];
            
            registerButton = transform.Find("RegisterButton").GetComponent<Button>();
            closeButton = transform.Find("CloseButton").GetComponent<Button>();
        }

        void Awake()
        {
            registerButton.onClick.AddListener(OnRegisterButton);
            closeButton.onClick.AddListener(OnCloseButton);
            
            inputCheckers = new Func<bool>[]
            {
                IsEmptyEmail,
                IsEmptyPassword,
                IsWrongPasswordLength,
                IsNotEqualPassword,
            };
        }

        void OnEnable()
        {
            LobbyAudioManager.PlaySfx(SfxType.PopupOpen);
        }
        void OnDisable()
        {
            LobbyAudioManager.PlaySfx(SfxType.PopupClose);
        }

        private void OnRegisterButton()
        {
            ChangeInteractable(false);
            
            string email = emailInput.text.Trim();
            string password = passwordInput.text.Trim();

            foreach (Func<bool> checker in inputCheckers)
            {
                if (checker())
                {
                    ChangeInteractable(true);
                    return;
                }
            }
            
            _ = AuthManager.Instance.CreateAccountAsync(email, password, OnCreateAccountCallback);
        }
        private void OnCreateAccountCallback(string errorMessage)
        {
            ChangeInteractable(true);

            if (string.IsNullOrEmpty(errorMessage)) // 회원가입 성공
            {
                gameObject.SetActive(false);
                
                DialogMessage.ShowMessage("회원가입 성공", $"E-mail: {emailInput.text}\n회원가입에 성공하였습니다.");
                emailInput.text = passwordInput.text = confirmPasswordInput.text = string.Empty;
                
                AuthUIManager.Instance.OpenFirstDataPanel();
            }
            else // 회원가입 실패
            {
                DialogMessage.ShowMessage("회원가입 실패", errorMessage);
            }
        }

        private void OnCloseButton()
        {
            emailInput.text = string.Empty;
            passwordInput.text = string.Empty;
            confirmPasswordInput.text = string.Empty;
            
            gameObject.SetActive(false);
        }

        private void ChangeInteractable(bool isOn)
        {
            emailInput.interactable = passwordInput.interactable = confirmPasswordInput.interactable =
                registerButton.interactable = closeButton.interactable = isOn;
        }


        #region ◇ 회원가입 시도, 연쇄책임패턴 Methods ◇
        private bool IsEmptyEmail()
        {
            bool result = string.IsNullOrWhiteSpace(emailInput?.text);
            if (result)
            {
                DialogMessage.ShowMessage("회원가입 실패", "Email을 입력해주세요.");
            }
            return result;
        }
        private bool IsEmptyPassword()
        {
            bool result = string.IsNullOrWhiteSpace(passwordInput?.text);
            if (result)
            {
                DialogMessage.ShowMessage("회원가입 실패", "패스워드를 입력해주세요.");
            }
            return result;
        }
        private bool IsWrongPasswordLength()
        {
            bool result = passwordInput.text.Trim().Length < 6;
            if (result)
            {
                DialogMessage.ShowMessage("회원가입 실패", "패스워드를 6자 이상 입력해주세요.");
            }
            return result;
        }
        private bool IsNotEqualPassword()
        {
            bool result = passwordInput.text.Trim() != confirmPasswordInput.text.Trim();
            if (result)
            {
                DialogMessage.ShowMessage("회원가입 실패", "패스워드가 서로 다릅니다.");
            }
            return result;
        }
        #endregion
    }
}