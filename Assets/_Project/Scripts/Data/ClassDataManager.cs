using System.Collections.Generic;
using Colosseum.Core;
using Colosseum.Unit;
using UnityEditor;
using UnityEngine;

namespace Colosseum.Data
{
    public class ClassDataManager: Singleton<ClassDataManager>
    {
        public static IEnumerable<CharacterClass> Classes => Instance.dataDic.Keys;
        
        
        [SerializeField] private ClassData[] data = { };
        [SerializeField] public Sprite defaultIcon;
        
        
        private readonly Dictionary<CharacterClass, ClassData> dataDic = new();
        

        #if UNITY_EDITOR
        void Reset()
        {
            string[] guids = AssetDatabase.FindAssets("t:ClassData");
            data = new ClassData[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                data[i] = AssetDatabase.LoadAssetAtPath<ClassData>(path);
            }
        }
        #endif

        protected override void Awake()
        {
            base.Awake();

            foreach (var classData in data)
            {
                CharacterClass classType = classData.characterClass;

                if (classType == CharacterClass.None)
                {
                    Debug.LogError($"{classData.name}의 Class Type이 None임.");
                    continue;
                }
                if (!dataDic.TryAdd(classType, classData))
                {
                    Debug.LogError($"{classData.name}의 {classType}이 중복존재.");
                    continue;
                }
            }
        }
        
        public static ClassData GetData(CharacterClass classType) => Instance.dataDic.GetValueOrDefault(classType);
    }
}