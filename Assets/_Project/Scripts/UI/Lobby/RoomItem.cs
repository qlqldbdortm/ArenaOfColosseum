using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Lobby
{
    public class RoomItem: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Toggle playerToggle;
        [SerializeField] private Button joinButton;
        
        
        private Toggle[] toggles;
        private string roomName = string.Empty;


        public void Init(string roomName, int maxPlayers)
        {
            nameText.text = this.roomName = roomName;
            
            toggles = new Toggle[maxPlayers];
            toggles[0] = playerToggle;

            for (int i = 1; i < maxPlayers;i++)
            {
                toggles[i] = Instantiate(playerToggle, playerToggle.transform.parent);
            }
            
            joinButton.onClick.AddListener(OnJoinButtonClick);
        }
        public void SetPlayerCount(int count)
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = i < count;
            }
        }

        private void OnJoinButtonClick()
        {
            PhotonNetwork.JoinRoom(this.roomName);
        }
    }
}