using System;
using Colosseum.Network;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Auth
{
    public class LoginPanel: MonoBehaviour
    {
        [Header("Input Fields")]
        [Tooltip("ID")] [SerializeField] private TMP_InputField emailInput;
        [Tooltip("비밀번호")] [SerializeField] private TMP_InputField passwordInput;


        [Header("Buttons")]
        [Tooltip("Login 버튼")] [SerializeField] private Button loginButton;
        [Tooltip("회원가입 버튼")] [SerializeField] private Button registerButton;


        private Func<bool>[] inputCheckers = null;


        void Reset()
        {
            TMP_InputField[] inputs = GetComponentsInChildren<TMP_InputField>();
            emailInput = inputs[0];
            passwordInput = inputs[1];
            
            Button[] buttons = GetComponentsInChildren<Button>();
            loginButton = buttons[0];
            registerButton = buttons[1];
        }

        void Awake()
        {
            loginButton.onClick.AddListener(OnLoginButton);
            registerButton.onClick.AddListener(OnRegisterButton);
            
            inputCheckers = new Func<bool>[]
            {
                IsEmptyEmail,
                IsEmptyPassword,
            };
        }


        private void OnLoginButton()
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

            _ = AuthManager.Instance.LoginAsync(email, password, OnLoginCallback);
        }
        private void OnLoginCallback(string errorMessage, bool hasData)
        {
            ChangeInteractable(true);

            if (string.IsNullOrEmpty(errorMessage)) // 로그인 성공
            {
                // DialogMessage.ShowMessage("로그인 성공", "로그인 성공"); // 로그인은 성공을 굳이 표시할 이유가 없음
                emailInput.text = passwordInput.text = string.Empty;

                if (hasData) // 유저 정보 존재
                {
                    PhotonNetwork.JoinLobby();
                }
                else // 유저 정보 미존재
                {
                    DialogMessage.ShowMessage("로그인 성공", "유저 정보를 생성합니다.");
                    AuthUIManager.Instance.OpenFirstDataPanel();
                }
            }
            else // 로그인 실패
            {
                DialogMessage.ShowMessage("로그인 실패", errorMessage);
            }
        }
        
        private void OnRegisterButton()
        {
            AuthUIManager.Instance.OpenRegisterPanel();
        }

        private void ChangeInteractable(bool isOn)
        {
            emailInput.interactable = passwordInput.interactable = loginButton.interactable = registerButton.interactable = isOn;
        }


        #region ◇ 회원가입 시도, 연쇄책임패턴 Methods ◇
        private bool IsEmptyEmail()
        {
            bool result = string.IsNullOrWhiteSpace(emailInput?.text);
            if (result)
            {
                DialogMessage.ShowMessage("로그인 실패", "Email을 입력해주세요.");
            }
            return result;
        }
        private bool IsEmptyPassword()
        {
            bool result = string.IsNullOrWhiteSpace(passwordInput?.text);
            if (result)
            {
                DialogMessage.ShowMessage("로그인 실패", "패스워드를 입력해주세요.");
            }
            return result;
        }
        #endregion
    }
}