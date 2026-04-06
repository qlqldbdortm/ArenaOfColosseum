using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Colosseum
{
    public class FirstUISelecter : MonoBehaviour
    {
        [Header("페이지 UI 설정")]
        [Tooltip("페이지가 활성화될 때 자동으로 선택될 첫 번째 UI")]
        [SerializeField] protected GameObject firstSelectedUI;
        
        [Tooltip("페이지 활성화 시 자동으로 UI 선택 여부")]
        [SerializeField] protected bool autoSelectOnEnable = true;

        protected virtual void OnEnable()
        {
            if (autoSelectOnEnable)
            {
                SelectFirstUI();
            }
        }

        /// <summary>
        /// 첫 번째 UI 선택
        /// </summary>
        public virtual void SelectFirstUI()
        {
            if (firstSelectedUI != null && firstSelectedUI.activeInHierarchy)
            {
                // UI가 활성화되고 상호작용 가능한지 확인
                Selectable selectable = firstSelectedUI.GetComponent<Selectable>();
                if (selectable != null && selectable.interactable)
                {
                    EventSystem.current.SetSelectedGameObject(firstSelectedUI);
                    return;
                }
            }

            // firstSelectedUI가 없거나 유효하지 않으면 자동으로 첫 번째 Selectable 찾기
            TrySelectFirstValidSelectable();
        }

        /// <summary>
        /// 자동으로 첫 번째 유효한 Selectable 찾기
        /// </summary>
        protected bool TrySelectFirstValidSelectable()
        {
            Selectable[] selectables = GetComponentsInChildren<Selectable>(false);
            foreach (var selectable in selectables)
            {
                if (selectable.gameObject.activeInHierarchy && selectable.interactable)
                {
                    EventSystem.current.SetSelectedGameObject(selectable.gameObject);
                    return true;
                }
            }
            return false;
        }

        protected virtual void OnDisable()
        {
            // 페이지가 비활성화될 때 선택 해제 (선택사항)
            if (EventSystem.current != null && 
                EventSystem.current.currentSelectedGameObject != null &&
                EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform))
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        
    }
}
