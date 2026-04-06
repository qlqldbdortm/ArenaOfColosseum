using Colosseum.UI.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.Customizing
{
    public class CustomizingUI : MonoBehaviour
    {
        [Header("성별 변환 토글 버튼")]
        public Toggle maleToggle;
        public Toggle femaleToggle;

        [Header("UI에서 변환되는 캐릭터 이미지의 루트 트랜스폼")]
        public CustomCharacter custom;
        
        [Header("Armor Selectors (버튼 UI)")] 
        public PartSelectorUI[] selectors;

        [Header("성별에 따라 적용되는 옵션")] 
        public Transform maleSpecialOption;
        public Transform femaleSpecialOption;
        
        [Header("가슴 사이즈 조절 슬라이더")] 
        public Slider bustSizeSlider;
        public GameObject leftBust;
        public GameObject rightBust;

        [Header("Exit 버튼")]
        public Button exitButton;

        void Awake()
        {
            // 가슴사이즈 조절을 위한 위치 찾기
            leftBust = custom.skeletonRoot.Find("root/Hips/Spine/Chest/bust_01_L").gameObject;
            rightBust = custom.skeletonRoot.Find("root/Hips/Spine/Chest/bust_01_R").gameObject;

            //창을 닫을 때 나올 소리
            exitButton.onClick.AddListener(CloseCustomizeUISound);
        }

        void OnEnable()
        {
            LobbyAudioManager.PlaySfx(SfxType.PopupOpen);
        }
        void CloseCustomizeUISound()
        {
            LobbyAudioManager.PlaySfx(SfxType.PopupClose);
        }
    }
}
