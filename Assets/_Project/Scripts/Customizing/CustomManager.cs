using System.Collections.Generic;
using Colosseum.Authentication;
using Colosseum.Core;
using UnityEngine;

namespace Colosseum.Customizing
{
    public class CustomManager : Singleton<CustomManager>
    {
        public CustomizingUI customUI; // UI 참조
        private CustomHandler handler;

        protected override void Awake()
        {
            base.Awake();

            handler = FindObjectOfType<CustomHandler>();

            // UI 이벤트 연결
            customUI.bustSizeSlider.onValueChanged.AddListener(OnBustSizeChanged);
            customUI.femaleToggle.onValueChanged.AddListener(OnClickFemaleToggle);
            customUI.maleToggle.onValueChanged.AddListener(OnClickMaleToggle);
        }

        private void OnEnable()
        {
            DeactivateAllArmors();
            if (GameDataManager.Instance.IsLoggedIn)
            {
                ApplyCustomization(GameDataManager.Instance.PlayerData.customizeData);
            }
        }

        private void Start()
        {
            // 초기화
            InitAllSelectors();
        }

        // UI Event
        private void OnBustSizeChanged(float value)
        {
            DataManager.Instance.ApplyBustSize(value);
            customUI.leftBust.transform.localScale = new Vector3(value, value, value);
            customUI.rightBust.transform.localScale = new Vector3(value, value, value);
        }

        private void OnClickFemaleToggle(bool isOn)
        {
            if (isOn) DataManager.Instance.SetGender(true);
        }

        private void OnClickMaleToggle(bool isOn)
        {
            if (isOn) DataManager.Instance.SetGender(false);
        }

        //CustomPanel.cs에서 처리합니다.
        /*public async void OnClickSave()
        {
            await DataManager.Instance.SaveToFirebase();
        }*/ 

        // 부위 변경 요청
        public void ChangePart(CustomPart part, int index)
        {
            DataManager.Instance.UpdateIndex(part, index);
            handler.ApplyPart(part, index);
        }

        // --- 기존 유틸리티 (필요 시 유지) ---
        public void DeactivateAllArmors()
        {
            if (customUI.custom.femaleMeshRoot is not null) InitMesh(customUI.custom.femaleMeshRoot);
            if (customUI.custom.maleMeshRoot is not null) InitMesh(customUI.custom.maleMeshRoot);
            if (customUI.custom.hairMeshRoot is not null) InitMesh(customUI.custom.hairMeshRoot);
        }

        private void InitMesh(Transform meshRoot)
        {
            meshRoot.GetChild(0).gameObject.SetActive(true);
            for (int i = 1; i < meshRoot.childCount; i++)
            {
                meshRoot.GetChild(i).gameObject.SetActive(false);
            }
        }

        public void InitAllSelectors()
        {
            foreach (var selector in customUI.selectors)
            {
                InitSelector(selector);
            }
        }

        private void InitSelector(PartSelectorUI selector)
        {
            List<string> options = new List<string>();
            if (selector.part != CustomPart.Hair && selector.part != CustomPart.FacialHair)
                options.Add("None");

            Transform meshRoot = selector.part switch
            {
                CustomPart.Hair => customUI.custom.hairMeshRoot,
                CustomPart.FacialHair => customUI.custom.facialHairRoot,
                _ => customUI.custom.isFemale ? customUI.custom.femaleMeshRoot : customUI.custom.maleMeshRoot
            };

            foreach (Transform child in meshRoot)
            {
                if (child.name.StartsWith("Armor_") || child.name.StartsWith("Hair_") ||
                    child.name.StartsWith("FacialHair_"))
                {
                    options.Add(child.name);
                }
            }

            if (DataManager.Instance.CurrentIndex.TryGetValue(selector.part, out var index))
            {
                DataManager.Instance.UpdateIndex(selector.part, index);
                UpdateLabel(selector, options, index);
            }
            
            selector.leftButton.onClick.RemoveAllListeners();
            selector.rightButton.onClick.RemoveAllListeners();

            selector.leftButton.onClick.AddListener(() => ChangeOption(selector.part, -1, selector, options));
            selector.rightButton.onClick.AddListener(() => ChangeOption(selector.part, +1, selector, options));
        }

        private void ChangeOption(CustomPart part, int delta, PartSelectorUI selector, List<string> options)
        {
            int index = (DataManager.Instance.CurrentIndex[part] +
                         delta + options.Count) % options.Count;
            DataManager.Instance.UpdateIndex(part, index);

            UpdateLabel(selector, options, index);
            handler.ApplyPart(part, index);
        }

        private void UpdateLabel(PartSelectorUI selector, List<string> options, int index)
        {
            selector.label.text = options[index];
        }

        public void UIPanelRefresh()
        {
            InitAllSelectors();
            ApplyToggleState();

            // 커스터마이징UI 패널의 SelectorUI를 초기화
            foreach (var selector in customUI.selectors)
            {
                if (DataManager.Instance.CurrentIndex.TryGetValue(selector.part, out int savedIndex))
                {
                    DataManager.Instance.CurrentIndex[selector.part] = savedIndex;
                }
            }
        }

        public void ApplyCustomization(CustomizeData data)
        {
            DataManager.Instance.InitData(data);
            DataManager.Instance.IsFemale = data.isFemale == 1;

            customUI.custom.isFemale = DataManager.Instance.IsFemale;
            customUI.custom.femaleMeshRoot.gameObject.SetActive(customUI.custom.isFemale);
            customUI.custom.maleMeshRoot.gameObject.SetActive(!customUI.custom.isFemale);

            UIPanelRefresh();
            // 초기화하는 메서드에서 모든 부위의 첫번째 파츠를 true로 바꾸기 때문에 헤어의 경우 겹쳐서 출력이 되어 헤어의 첫번째 자식을 비활성화 
            customUI.custom.hairMeshRoot.GetChild(0).gameObject.SetActive(false);

            foreach (var custom in DataManager.Instance.CurrentIndex)
            {
                handler.ApplyPart(custom.Key, custom.Value);
            }

            customUI.bustSizeSlider.value = data.bustSize;
        }

        private void ApplyToggleState()
        {
            // 이벤트 잠시 해제
            customUI.maleToggle.onValueChanged.RemoveListener(OnClickMaleToggle);
            customUI.femaleToggle.onValueChanged.RemoveListener(OnClickFemaleToggle);

            if (DataManager.Instance.IsFemale)
            {
                customUI.maleToggle.isOn = false;
                customUI.femaleToggle.isOn = true;
            }
            else
            {
                customUI.maleToggle.isOn = true;
                customUI.femaleToggle.isOn = false;
            }

            customUI.maleSpecialOption.gameObject.SetActive(customUI.maleToggle.isOn);
            customUI.femaleSpecialOption.gameObject.SetActive(customUI.femaleToggle.isOn);

            // 이벤트 다시 연결
            customUI.maleToggle.onValueChanged.AddListener(OnClickMaleToggle);
            customUI.femaleToggle.onValueChanged.AddListener(OnClickFemaleToggle);
        }
    }
}