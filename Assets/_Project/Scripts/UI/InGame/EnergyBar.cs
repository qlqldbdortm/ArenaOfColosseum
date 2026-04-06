using Colosseum.Data;
using Colosseum.LifeCycle;
using Colosseum.Network;
using Colosseum.Unit;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.InGame
{
    /// <summary>
    /// 모든 유저의 캐릭터 위에 붙일 체력/스태미너 바가 포함된 정보창
    /// </summary>
    public class EnergyBar: MonoBehaviour, IInit<Unit.Unit>
    {
        [Header("Emoticon")]
        [SerializeField] private Image emoticon;
        [SerializeField] private Sprite[] emoteSprites;
        
        [Header("Bar")]
        [SerializeField] private TextFillBar hpBar;
        [SerializeField] private TextFillBar staminaBar;
        
        [Header("Other UI")]
        [SerializeField] private TextMeshProUGUI nickName;
        [SerializeField] private Image classIcon;


        private Unit.Unit target = null;
        private Camera mainCamera = null;

        private Sequence scaleSequence = null;
        private Sequence colorSequence = null;
        
        
        public void OnInit(Unit.Unit unit)
        {
            emoticon.color = Color.clear;
            hpBar.SetValueNotTween(unit.CurrentHp, unit.MaxHp);
            staminaBar.SetValueNotTween(unit.CurrentStamina, unit.MaxStamina);
            Connect(unit);
            
            unit.OnEmotion += OnEmotion;
        }
        
        private void Connect(Unit.Unit unit)
        {
            mainCamera = Camera.main;
            target = unit;
            
            target.OnHpChanged += hpBar.ChangeValue;
            target.OnStaminaChanged += staminaBar.ChangeValue;
            nickName.text = unit.Nickname;
            nickName.color = unit.Team == TeamType.Left
                ? new Color(0.35f, 0.6f, 1f)
                : new Color(1f, 0.35f, 0.35f);

            var charClass = unit.photonView.Owner.CustomProperties.GetValueOrDefault(PropName.CLASS_TYPE, CharacterClass.None);
            classIcon.sprite = ClassDataManager.GetData(charClass).classIcon;
        }

        private void OnEmotion(int emote)
        {
            emoticon.sprite = emoteSprites[emote];
            emoticon.color = Color.white;

            scaleSequence?.Kill();
            scaleSequence = DOTween.Sequence();
            scaleSequence.Append(emoticon.transform.DOScale(new Vector2(1.25f, 0.75f), 0.25f));
            scaleSequence.Append(emoticon.transform.DOScale(new Vector2(0.75f, 1.25f), 0.25f));
            scaleSequence.Append(emoticon.transform.DOScale(Vector2.one, 0.25f));
            scaleSequence.Play();
            
            colorSequence?.Kill();
            colorSequence = DOTween.Sequence();
            colorSequence.AppendInterval(1f);
            colorSequence.Append(emoticon.DOColor(Color.clear, 0.25f));
            colorSequence.Play();
        }
    }
}