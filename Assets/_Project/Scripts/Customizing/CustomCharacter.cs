using System;
using System.Collections.Generic;
using Colosseum.Authentication;
using UnityEngine;

namespace Colosseum.Customizing
{
    public class CustomCharacter : MonoBehaviour
    {
        [Header("UI에서 변환되는 캐릭터 이미지의 루트 트랜스폼")]
        public Transform skeletonRoot;
        public Transform maleMeshRoot;
        public Transform femaleMeshRoot;
        public Transform hairMeshRoot;
        public Transform facialHairRoot;
        public Transform weaponSwordRoot;
        public Transform weaponShieldRoot;
        public Transform weaponStaffRoot;
        public Transform weaponBowRoot;
        public GameObject leftBust;
        public GameObject rightBust;
        public bool isFemale = false;
        public CustomizeData Data { get; set; } = null;

        private void Awake()
        {
            leftBust = skeletonRoot.Find("root/Hips/Spine/Chest/bust_01_L").gameObject;
            rightBust = skeletonRoot.Find("root/Hips/Spine/Chest/bust_01_R").gameObject;
        }
    }
}