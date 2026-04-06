using Colosseum.Authentication;
using Firebase;
using UnityEngine;

namespace Colosseum.Customizing.Custom
{
    public partial class CustomizingManager
    {
        /// <summary>
        /// UI Refresh를 하면서 토글버튼도 Apply한 성별에 맞게 변경하는 메서드
        /// </summary>
        private void ApplyToggleState()
        {
            // 이벤트 잠시 해제
            customUI.maleToggle.onValueChanged.RemoveListener(OnClickMaleToggle);
            customUI.femaleToggle.onValueChanged.RemoveListener(OnClickFemaleToggle);

            if (customUI.custom.isFemale)
                customUI.femaleToggle.isOn = true;
            else
                customUI.maleToggle.isOn = true;
            
            customUI.maleSpecialOption.gameObject.SetActive(customUI.maleToggle.isOn);
            customUI.femaleSpecialOption.gameObject.SetActive(customUI.femaleToggle.isOn);
            
            // 이벤트 다시 연결
            customUI.maleToggle.onValueChanged.AddListener(OnClickMaleToggle);
            customUI.femaleToggle.onValueChanged.AddListener(OnClickFemaleToggle);
        }
        
        /// <summary>
        /// Json파일에서 CustomizeData를 읽어와 UI에 반영하는 메서드
        /// </summary>
        /// <param name="data"></param>
        public void UIPanelRefresh(CustomizeData data)
        {
            DeactivateAllArmors();
            InitAllSelectors();
            InitCurrentIndexDir(data);
            ApplyToggleState();
            
            // 커스터마이징UI 패널의 SelectorUI를 초기화
            foreach (var selector in customUI.selectors)
            {
                if (currentIndex.TryGetValue(selector.part, out int savedIndex))
                {
                    currentIndex[selector.part] = savedIndex;
                    UpdateLabel(selector);
                }
            }
        }
        
        /// <summary>
        /// Firebase에 저장되어있는 커스터마이징 데이터를 가져와 커스터마이징을 하는 메서드
        /// </summary>
        /// <param name="data"></param>
        public void ApplyCustomization(CustomizeData data)
        {
            InitCurrentIndexDir(data);

            customUI.custom.isFemale = data.isFemale == 1;

            customUI.custom.femaleMeshRoot.gameObject.SetActive(customUI.custom.isFemale);
            customUI.custom.maleMeshRoot.gameObject.SetActive(!customUI.custom.isFemale);
            
            UIPanelRefresh(data);
            // 초기화하는 메서드에서 모든 부위의 첫번째 파츠를 true로 바꾸기 때문에 헤어의 경우 겹쳐서 출력이 되어 헤어의 첫번째 자식을 비활성화 
            customUI.custom.hairMeshRoot.GetChild(0).gameObject.SetActive(false);

            foreach (var custom in currentIndex)
            {
                CustomizePart(custom.Key,custom.Value);
            }
            customUI.bustSizeSlider.value = data.bustSize;
        }

        private async void ApplyFirebase()
        {
            Apply();
            try
            {
                GameDataManager.Instance.PlayerData.customizeData = customUI.custom.Data;
                await GameDataManager.Instance.PlayerData.SaveAsync();
            }
            catch (FirebaseException e)
            {
                Debug.LogError($"에러 메세지 : {e}");
            }
        }
    }
}