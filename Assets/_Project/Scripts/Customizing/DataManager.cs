using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colosseum.Authentication;
using Colosseum.Core;

namespace Colosseum.Customizing
{
    public class DataManager : Singleton<DataManager>
    {
        public CustomizeData CurrentData { get; private set; } = new();
        public Dictionary<CustomPart, int> CurrentIndex { get; private set; } = new();

        public bool IsFemale { get; set; }
        public float BustSize { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitIndex();
            DontDestroyOnLoad(gameObject);
        }
        
        public void InitIndex()
        {
            CurrentIndex = new Dictionary<CustomPart, int>
            {
                { CustomPart.Hair, CurrentData.hair },
                { CustomPart.Chest, CurrentData.chest },
                { CustomPart.Leg, CurrentData.leg },
                { CustomPart.Arm, CurrentData.arm },
                { CustomPart.Waist, CurrentData.waist },
                { CustomPart.FacialHair, CurrentData.facialHair },
            };
        }

        public void InitData(CustomizeData data)
        {
            CurrentData = data;
            InitIndex();
        }
        public void ClearData()
        {
            CurrentData.hair = 0;
            CurrentData.chest = 0;
            CurrentData.leg = 0;
            CurrentData.arm = 0;
            CurrentData.waist = 0;
            CurrentData.facialHair = 0;
            CurrentData.isFemale = IsFemale ? 1 : 0;
            InitIndex();
        }
        public void UpdateIndex(CustomPart part, int index)
        {
            CurrentIndex[part] = index;
        }

        public void SetGender(bool female)
        {
            var charInfo = CustomManager.Instance.customUI;
            charInfo.custom.isFemale = female;
            IsFemale = female;
            
            charInfo.maleToggle.interactable = charInfo.custom.isFemale;
            charInfo.femaleToggle.interactable = !charInfo.custom.isFemale;
            
            charInfo.maleSpecialOption.gameObject.SetActive(!charInfo.custom.isFemale);
            charInfo.femaleSpecialOption.gameObject.SetActive(charInfo.custom.isFemale);
            
            charInfo.custom.femaleMeshRoot.gameObject.SetActive(charInfo.custom.isFemale);
            charInfo.custom.maleMeshRoot.gameObject.SetActive(!charInfo.custom.isFemale);
            
            ClearData();
            // 캐릭터의 성별이 변환되면 기존에 해놨던 세팅을 초기화
            CustomManager.Instance.DeactivateAllArmors();
            CustomManager.Instance.InitAllSelectors(); 
        }

        public void ApplyBustSize(float size)
        {
            BustSize = size;
        }

        public void SaveCurrentData()
        {
            CurrentData = new CustomizeData
            {
                hair = CurrentIndex[CustomPart.Hair],
                chest = CurrentIndex[CustomPart.Chest],
                arm = CurrentIndex[CustomPart.Arm],
                waist = CurrentIndex[CustomPart.Waist],
                leg = CurrentIndex[CustomPart.Leg],
                facialHair = CurrentIndex[CustomPart.FacialHair],
                isFemale = IsFemale ? 1 : 0,
                bustSize = BustSize,
            };
        }

        public async Task SaveToFirebase()
        {
            SaveCurrentData();
            GameDataManager.Instance.PlayerData.customizeData = CurrentData;
            await GameDataManager.Instance.PlayerData.SaveAsync();
        }
        
        public void ApplyCustomization(CustomHandler handler)
        {
            foreach (var custom in CurrentIndex)
            {
                handler.ApplyPart(custom.Key, custom.Value);
            }
        }
    }
}