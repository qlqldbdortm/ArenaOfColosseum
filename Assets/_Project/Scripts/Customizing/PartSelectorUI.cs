using TMPro;
using UnityEngine.UI;

namespace Colosseum.Customizing
{
    [System.Serializable]
    public class PartSelectorUI
    {
        public CustomPart part;
        public Button leftButton;
        public Button rightButton;
        public TMP_Text label; // 현재 선택 표시
    }
}
