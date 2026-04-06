using Photon.Pun;
using Photon.Realtime;
using Colosseum.Authentication;
using Colosseum.Data;
using Colosseum.Network.Lobby;
using Colosseum.Unit;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Room
{
    public class PlayerEntryPanel: MonoBehaviour
    {
        [Header("유저 확인용")]
        [Tooltip("플레이어가 있는지 확인용 패널")][SerializeField] private GameObject userExistPanel;
        [Tooltip("플레이어가 없는지 확인용 패널")][SerializeField] private GameObject userVanishPanel;
        [Tooltip("자리 이동 버튼")][SerializeField] private Button slotChangeButton;
        [Tooltip("준비 확인용 패널")][SerializeField] private GameObject readyPanel;
        [Tooltip("마스터인지 확인용 오브젝트")][SerializeField] private GameObject masterObject;
        
        
        [Header("플레이어 정보")]
        [Tooltip("이름 텍스트")][SerializeField] private TextMeshProUGUI nameText;
        [Tooltip("전적 텍스트")][SerializeField] private TextMeshProUGUI winRateText;
        
        [Header("선택 정보")]
        [Tooltip("클래스 아이콘")][SerializeField] private Image classIconImage;


        public int SlotNumber { get; set; } = -1;
        public Player User { get; private set; } = null;

        private bool canSlotChange = true;


        void Awake()
        {
            ResetUI();
            _ = SetUserAsync(null);
            slotChangeButton.onClick.AddListener(OnSlotChangeButtonClick);
        }


        public async UniTask SetUserAsync(Player player)
        {
            ResetUI();
            
            // 플레이어 패널 On / Off
            if ((User = player) == null) // 플레이어 나감 or 애초에 없었음
            {
                nameText.gameObject.SetActive(false);
                userExistPanel.SetActive(false);
                userVanishPanel.SetActive(true);
                return;
            }
            nameText.gameObject.SetActive(true);
            userExistPanel.SetActive(true);
            userVanishPanel.SetActive(false);


            // uid를 받아서, userData 호출
            Hashtable table = player.CustomProperties;
            string uid = table[PropName.UID].ToString();
            var userData = new UserData(uid);
            await userData.LoadAsync();
            
            
            nameText.SetText(userData.nickname);
            winRateText.SetText($"{userData.WinRate:F1}% ({userData.wins} / {userData.totalMatches})");

            bool isReady = table.GetValueOrDefault(PropName.ROOM_READY, false);
            CharacterClass classType = table.GetValueOrDefault(PropName.CLASS_TYPE, CharacterClass.None);
            OnReadyChanged(isReady);
            OnMasterSelected(PhotonNetwork.MasterClient == player);
            OnClassChanged(classType);
        }

        public void OnReadyChanged(bool isReady)
        {
            readyPanel.SetActive(isReady);
        }
        public void OnMasterSelected(bool isMaster)
        {
            masterObject.SetActive(isMaster);
        }
        public void OnClassChanged(CharacterClass newClass)
        {
            ClassData classData = ClassDataManager.GetData(newClass);
            classIconImage.sprite = classData?.classIcon ?? ClassDataManager.Instance.defaultIcon;
        }

        public void LockSlotChangeButton(bool isOn)
        {
            slotChangeButton.interactable = canSlotChange = isOn;
        }



        private void OnSlotChangeButtonClick()
        {
            RoomPunManager.Instance.TrySlotChange(SlotNumber);
            _ = DelayLockAsync();
        }
        private async UniTask DelayLockAsync()
        {
            slotChangeButton.interactable = false;
            await UniTask.WaitForSeconds(1f); // TODO: 일단 테스트삼아 1초로 했는데, 바꿔도 됨.
            slotChangeButton.interactable = canSlotChange;
        }

        private void ResetUI()
        {
            nameText.SetText(string.Empty);
            winRateText.SetText(string.Empty);
            
            masterObject.SetActive(false);
            readyPanel.SetActive(false);
        }
    }
}