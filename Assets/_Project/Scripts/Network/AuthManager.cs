using Colosseum.Core;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using System;
using System.Threading.Tasks;
using Colosseum.Authentication;
using Colosseum.Customizing;
using Colosseum.Customizing.Custom;
using Colosseum.InputSystem;
using Colosseum.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Colosseum.Network
{
    public partial class AuthManager : PhotonSingleton<AuthManager>
    {
        public static event Action<string> OnConnectMessage;
        
        
        public bool IsInitialized => connectedFirebase && connectedPhoton;
        
        public event Action OnFirebaseInitialized;

        public FirebaseAuth Auth { get; private set; }
        public FirebaseApp App { get; private set; }
        public FirebaseDatabase DB { get; private set; }


        private bool connectedFirebase = false;
        private bool connectedPhoton = false;

        
        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) return;
            DontDestroyOnLoad(gameObject);
            PhotonNetwork.GameVersion = "Area of Colosseum 1.0.0";
        }
        void Start()
        {
            // onConnectMessage는 매 씬 변환마다 초기화해야 함
            OnConnectMessage += Debug.Log;
            
            ConnectNetwork();
        }

        private void ConnectNetwork()
        {
            if (connectedFirebase && connectedPhoton)
            {
                return;
            }
            MainPageManager.Instance?.ChangePage(MainPageType.Loading);
            
            if (!connectedFirebase)
            {
                _ = ConnectFirebase();
            }
            if (!connectedPhoton)
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.AutomaticallySyncScene = true;
            }
        }

        private async Task ConnectFirebase()
        {
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (status != DependencyStatus.Available)
            {
                OnConnectMessage?.Invoke($"DB 연결 실패: {status}");
                DialogMessage.ShowMessage("DB 연결 실패", status.ToString());
            }
            else
            {
                App =  FirebaseApp.DefaultInstance;
                Auth = FirebaseAuth.DefaultInstance;
                DB = FirebaseDatabase.DefaultInstance;
                OnFirebaseInitialized?.Invoke(); //GameDataManager가 구독합니다.
            }
            
            FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
            
            OnConnectMessage?.Invoke($"DB 연결 성공");

            connectedFirebase = true;
        }
        
        //회원가입 로직
        public async Task CreateAccountAsync(string email, string password, Action<string> callback)
        {
            string errorMessage = null;
            try
            {
                AuthResult result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
                GameDataManager.Instance.PlayerData = new(result.User.UserId);
            }
            catch (FirebaseException ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                callback?.Invoke(errorMessage);
            }
        }
        
        //로그인 로직
        public async Task LoginAsync(string email, string password, Action<string, bool> callback)
        {
            string errorMessage = null;
            bool hasData = false;

            try
            {
                var loginTask = await Auth.SignInWithEmailAndPasswordAsync(email, password);
                string userId = loginTask.User.UserId; //유저아이디 가져옴
                var dataTask = await DB.RootReference.Child($"users/{userId}").GetValueAsync(); //유저의 데이터 필드 주소 가져옴

                // UserData 생성
                UserData userData = new(userId); // UID 먼저 설정!
                GameDataManager.Instance.PlayerData = userData; //로컬에 저장
                
                if (hasData = dataTask.Exists) // 계정정보가 있을 때만 동작
                {
                    // Firebase에 있는 내용을 uid 키로 호출
                    await userData.LoadAsync();
                    CustomManager.Instance.ApplyCustomization(userData.customizeData);
                    // Photon에 계정 설정
                    string userNickname = userData.nickname;
                    PhotonNetwork.NickName = userNickname;
                }
            }
            catch (FirebaseException ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                callback?.Invoke(errorMessage, hasData);
            }
        }

        public async UniTask Logout(Action<string> callback)
        {
            string errorMessage = null;
            try
            {
                await UniTask.WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
                
                GameDataManager.Instance.PlayerData = null;
                PhotonNetwork.LeaveLobby();
                PhotonNetwork.NickName = string.Empty;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                callback?.Invoke(errorMessage);
            }
        }

        public async Task ChangeNicknameAsync(string nickname, Action<string> callback)
        {
            string errorMessage = null;
            
            try
            {
                GameDataManager.Instance.PlayerData.nickname = nickname;
                await GameDataManager.Instance.PlayerData.SaveAsync();
            }
            catch (FirebaseException ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                callback?.Invoke(errorMessage);
            }
        }
        
        #region 닉네임관련 메서드, 오류메세지
        public async Task<bool> IsNicknameAvailable(string nickname)
        {
            try
            {
                DatabaseReference nicknameRef = DB.RootReference.Child($"nicknames/{nickname}");
                DataSnapshot snapshot = await nicknameRef.GetValueAsync();

                //snapshot.Exists가 true면 중복, false면 사용가능
                return snapshot.Exists;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"닉네임 중복 검사 오류: {e.Message}");
                return false;
            }
        }
        //te
        private string GetRegisterErrorMessage(FirebaseException fe)
        {
            AuthError errorCode = (AuthError)fe.ErrorCode;
    
            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    return "이미 사용 중인 이메일입니다";
        
                case AuthError.InvalidEmail:
                    return "올바르지 않은 이메일 형식입니다";
        
                case AuthError.WeakPassword:
                    return "비밀번호가 너무 약합니다. 6자 이상 입력해주세요";
        
                case AuthError.NetworkRequestFailed:
                    return "네트워크 연결을 확인해주세요";
                
                case AuthError.TooManyRequests:
                    return "너무 많은 요청이 발생했습니다. 잠시 후 다시 시도해주세요.";
        
                default:
                    return $"회원가입에 실패했습니다 : {errorCode}";
            }
        }

        private string GetSignInErrorMessage(FirebaseException fe)
        {
            AuthError errorCode = (AuthError)fe.ErrorCode;

            switch (errorCode)
            {
                case AuthError.UserNotFound:
                    return "등록되지 않은 이메일입니다.";
                    
                case AuthError.WrongPassword:
                    return "비밀번호가 올바르지 않습니다.";
        
                case AuthError.InvalidEmail:
                    return "올바르지 않은 이메일 형식입니다.";

                case AuthError.TooManyRequests:
                    return "로그인 시도 횟수를 초과했습니다. 잠시 후 다시 시도해주세요.";
        
                case AuthError.NetworkRequestFailed:
                    return "네트워크 연결을 확인해주세요.";
                    
                case AuthError.InvalidCredential:
                    return "잘못된 로그인 정보입니다.";
                
                default:
                    return $"로그인에 실패했습니다. 이메일과 비밀번호를 확인해주세요. : {errorCode}";
            }
        }
        #endregion
    }
}
