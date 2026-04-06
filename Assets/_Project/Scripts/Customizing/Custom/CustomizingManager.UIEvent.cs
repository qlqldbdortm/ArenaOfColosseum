using UnityEngine;

namespace Colosseum.Customizing.Custom
{
    public partial class CustomizingManager
    {
        /// <summary>
        /// 성별(남성) 토글이 눌렸을 때 실행되는 이벤트 메서드 (true일 때만 실행)
        /// </summary>
        /// <param name="isOn"></param>
        private void OnClickMaleToggle(bool isOn)
        {
            if (isOn)
            {
                SetGender(false);
            }
        }

        /// <summary>
        /// 성별(여성) 토글이 눌렸을 때 실행되는 이벤트 메서드 (true일 때만 실행)
        /// </summary>
        /// <param name="isOn"></param>
        private void OnClickFemaleToggle(bool isOn)
        {
            if (isOn)
            {
                SetGender(true);
            }
        }

        /// <summary>
        /// 여성형 캐릭터의 가슴 사이즈 조절 메서드
        /// </summary>
        /// <param name="size"></param>
        private void ChangeBustSize(float size)
        {
            customUI.leftBust.transform.localScale = new Vector3(size, size, size);
            customUI.rightBust.transform.localScale = new Vector3(size, size, size);
        }

        /// <summary>
        /// 성별 변환 토글 이벤트를 실행시키면 실제적으로 
        /// </summary>
        /// <param name="female"></param>
        private void SetGender(bool female)
        {
            customUI.custom.isFemale = female;
            
            customUI.maleToggle.interactable = customUI.custom.isFemale;
            customUI.femaleToggle.interactable = !customUI.custom.isFemale;
            
            customUI.maleSpecialOption.gameObject.SetActive(!customUI.custom.isFemale);
            customUI.femaleSpecialOption.gameObject.SetActive(customUI.custom.isFemale);
            
            customUI.custom.femaleMeshRoot.gameObject.SetActive(customUI.custom.isFemale);
            customUI.custom.maleMeshRoot.gameObject.SetActive(!customUI.custom.isFemale);
            
            // 캐릭터의 성별이 변환되면 기존에 해놨던 세팅을 초기화
            DeactivateAllArmors();
            InitAllSelectors(); 
            equipped.Clear();
        }
    }
}