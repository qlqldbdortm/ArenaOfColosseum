using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colosseum
{
    [CreateAssetMenu(fileName = "EmoticonData", menuName = "Emoticon/EmoticonData")]
    public class EmoticonData : ScriptableObject
    {
        [Header("이모티콘 정보")]
        [Tooltip("이모티콘 고유 ID(자동 생성)")]
        public string emoticonId;
        
        [Tooltip("이모티콘 이름")]
        public string emoticonName;

        [Tooltip("이모티콘 이미지")] 
        public Sprite icon;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(emoticonId))
            {
                emoticonId = System.Guid.NewGuid().ToString();
            }
        }
    }
}
