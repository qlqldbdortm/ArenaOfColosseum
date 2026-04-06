using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI
{
    public class TextFillBar: MonoBehaviour
    {
        [SerializeField] private Image fillArea;
        [SerializeField] private TextMeshProUGUI text;

        
        public float MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                OnValueChanged();
            }
        }
        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueChanged();
            }
        }


        private float maxValue = 1;
        private float value = 1;


        void Awake()
        {
            SetValueNotTween(value, maxValue);
        }
        
        void Reset()
        {
            fillArea = transform.Find("FillArea").GetComponent<Image>();
            text = GetComponentInChildren<TextMeshProUGUI>();
        }


        public void SetValueNotTween(float value, float maxValue)
        {
            this.value = value;
            MaxValue = maxValue;
            fillArea.fillAmount = value / maxValue;
        }
        public void ChangeValue(float value) => Value = value;
        
        
        private void OnValueChanged()
        {
            if(text != null)
            {
                text.text = $"{(int)Value:N0} / {(int)MaxValue:N0}";
            }
            fillArea.DOFillAmount(value / maxValue, 0.5f);
        }
    }
}