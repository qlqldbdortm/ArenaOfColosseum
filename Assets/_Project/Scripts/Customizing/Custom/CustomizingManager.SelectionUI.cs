using System.Collections.Generic;
using UnityEngine;

namespace Colosseum.Customizing.Custom
{
    public partial class CustomizingManager 
    {
        /// <summary>
        /// 셀렉터 전부 초기화하는 메서드
        /// </summary>
        private void InitAllSelectors()
        {
            foreach (var selector in customUI.selectors)
            {
                InitSelector(selector);
            }
        }
        
        /// <summary>
        /// 셀렉터를 초기화 하는 메서드<br/>
        /// 셀렉터 라벨 업데이트, 셀렉터 버튼의 이벤트 추가
        /// </summary>
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
                if (child.name.StartsWith("Armor_") || child.name.StartsWith("Hair_") || child.name.StartsWith("FacialHair_"))
                {
                    options.Add(child.name);
                }
            }

            partOptions[selector.part] = options;
            currentIndex[selector.part] = 0;

            UpdateLabel(selector);
            
            selector.leftButton.onClick.RemoveAllListeners();
            selector.rightButton.onClick.RemoveAllListeners();

            selector.leftButton.onClick.AddListener(() => { ChangeOption(selector.part, -1, selector); });
            selector.rightButton.onClick.AddListener(() => { ChangeOption(selector.part, +1, selector); });
        }
        
        private void ChangeOption(CustomPart part, int delta, PartSelectorUI selector)
        {
            var options = partOptions[part];
            int index = currentIndex[part];
            
            index = (index + delta + options.Count) % options.Count;
            currentIndex[part] = index;
            
            UpdateLabel(selector);
            OnOptionChanged(part, index);
        }

        private void UpdateLabel(PartSelectorUI selector)
        {
            int index = currentIndex[selector.part];
            selector.label.text = partOptions[selector.part][index];
        }
    }
}
