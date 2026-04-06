using System;
using System.Threading.Tasks;
using Colosseum.UI;
using Firebase.Database;
using UnityEngine;

namespace Colosseum.Authentication
{
    [System.Serializable]
    public class CustomizeData
    {
        public int hair = 0;
        public int chest = 0;
        public int arm = 0;
        public int waist = 0;
        public int leg = 0;
        public int facialHair = 0;
        public int isFemale = 0;
        public float bustSize = 0.8f;
    }
    
    [System.Serializable]
    public class UserData
    {
        [NonSerialized]
        public string Uid; 
        //데이터 보낼때 UID를 계속 주고 받고 하지말고 이미 있는 UID 주소에 내용만 넣는다. 외부에서 UID를 조작하여 다른사람의 데이터를 접근하는 걸 방지
        
        public string nickname = string.Empty;
        public int totalMatches = 0;
        public int wins = 0;
        public CustomizeData customizeData = new();
        
        
        public float WinRate => totalMatches > 0 ? (float)wins / totalMatches * 100f : 0f;
        
        
        public UserData(string uid)
        {
            Uid = uid;
        }
        

        // ReSharper disable Unity.PerformanceAnalysis
        public async Task SaveAsync() //이 메서드를 불러오면 현재 이 오브젝트에 저장된 데이터를 올림 
        {
            if (string.IsNullOrEmpty(Uid))
            {
                Debug.LogError("로그인된 사용자가 없습니다. 데이터를 저장할 수 없습니다.");
                return;
            }
            
            string json = JsonUtility.ToJson(this);
            
            try
            {
                DatabaseReference userRef = GameDataManager.Instance.DB.RootReference.Child("users").Child(Uid);
                await userRef.SetRawJsonValueAsync(json);
                
                Debug.Log($"사용자 데이터가 성공적으로 저장되었습니다. UserID: {Uid}");
            }
            catch (Exception e)
            {
                Debug.LogError($"사용자 데이터 저장 실패: {e.Message}");
            }
        }
        
        //현재 구조상 GameDataManager 로컬에서 먼저 데이터를 업데이트하고, 그 다음 서버로 보내는 방식이므로 
        //당장 LoadAsync메서드를 사용하지는 않습니다.
        // ReSharper disable Unity.PerformanceAnalysis
        public async Task LoadAsync()
        {
            if (string.IsNullOrEmpty(Uid))
            {
                DialogMessage.ShowMessage("UserData Load 에러", "사용자 UID가 없어 데이터를 불러올 수 없습니다.");
                return;
            }

            try
            {
                DatabaseReference userRef = GameDataManager.Instance.DB.RootReference.Child("users").Child(Uid);
                DataSnapshot snapshot = await userRef.GetValueAsync();

                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    JsonUtility.FromJsonOverwrite(json, this);

                    Debug.Log("사용자의 데이터가 성공적으로 로드되었습니다.");
                }
                else
                {
                    DialogMessage.ShowMessage("UserData Load 에러", $"데이터베이스에 사용자 데이터가 없습니다. UserID: {Uid}");
                }
            }
            catch (System.Exception e)
            {
                DialogMessage.ShowMessage("UserData Load 에러", $"사용자 데이터 로드 실패: {e.Message}");
            }
        }
        
    }
}
