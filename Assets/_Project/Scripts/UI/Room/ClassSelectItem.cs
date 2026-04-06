using Colosseum.Data;
using Colosseum.UI.Audio;
using Photon.Pun;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Room
{
    public class ClassSelectItem: MonoBehaviour
    {
        [SerializeField] private Image classIcon;
        [SerializeField] private TextMeshProUGUI className;
        
        [SerializeField] private Button selectButton;
        
        
        public ClassData ClassData { get; private set; } = null;


        void Awake()
        {
            selectButton.onClick.AddListener(OnSelectButtonClick);
        }


        public void Init(ClassData classData)
        {
            this.ClassData = classData;
            
            className.text = ClassData.className;
            classIcon.sprite = ClassData.classIcon;
        }


        private void OnSelectButtonClick()
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.GetValueOrDefault(PropName.ROOM_READY, false)) return;

            PhotonNetwork.LocalPlayer.AddProperty((PropName.CLASS_TYPE, ClassData.characterClass));

            LobbyAudioManager.PlaySfx(SfxType.RoomEnter);
        }
    }
}