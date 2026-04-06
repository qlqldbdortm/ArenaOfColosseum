using Colosseum.Core;
using Colosseum.Authentication;
using System.Collections.Generic;
using UnityEngine;

namespace Colosseum.Customizing.Custom
{
    public partial class CustomizingManager : Singleton<CustomizingManager>
    {
        public CustomizingUI customUI;

        private Dictionary<CustomPart, GameObject> equipped = new Dictionary<CustomPart, GameObject>();
        private Dictionary<CustomPart, List<string>> partOptions = new Dictionary<CustomPart, List<string>>();

        public Dictionary<CustomPart, int> currentIndex = new Dictionary<CustomPart, int>()
        {
            { CustomPart.Hair, 0 },
            { CustomPart.Chest, 0 },
            { CustomPart.Leg, 0 },
            { CustomPart.Arm, 0 },
            { CustomPart.Waist, 0 },
            { CustomPart.FacialHair, 0 },
        };

        protected override void Awake()
        {
            base.Awake();

            // UI 이벤트 추가
            customUI.bustSizeSlider.onValueChanged.AddListener(ChangeBustSize);
            customUI.femaleToggle.onValueChanged.AddListener(OnClickFemaleToggle);
            customUI.maleToggle.onValueChanged.AddListener(OnClickMaleToggle);
            customUI.exitButton.onClick.AddListener(ApplyFirebase);
        }

        private void Start()
        {
            DeactivateAllArmors();
            InitAllSelectors();
        }

        /// <summary>
        /// 모든 Armor 비활성화
        /// </summary>
        private void DeactivateAllArmors()
        {
            if (customUI.custom.femaleMeshRoot is not null) InitMesh(customUI.custom.femaleMeshRoot);
            if (customUI.custom.maleMeshRoot is not null) InitMesh(customUI.custom.maleMeshRoot);
            if (customUI.custom.hairMeshRoot is not null) InitMesh(customUI.custom.hairMeshRoot);
        }

        /// <summary>
        /// 초기 Mesh만 남기고 모두 비활성화 시키는 메서드
        /// </summary>
        /// <param name="meshRoot"></param>
        private void InitMesh(Transform meshRoot)
        {
            meshRoot.GetChild(0).gameObject.SetActive(true);
            for (int i = 1; i < meshRoot.childCount; i++)
            {
                meshRoot.GetChild(i).gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Selector에서 옵션이 변경되었을 때 반영하는 메서드 (실제적으로 커스터마이징하는 부분)
        /// </summary>
        /// <param name="part">커스터마이징 하는 부위</param>
        /// <param name="index">커스터마이징 인덱스</param>
        private void OnOptionChanged(CustomPart part, int index)
        {
            if (part != CustomPart.Hair && part != CustomPart.FacialHair && index == 0)
            {
                Unequip(part);
                //return;
            }

            CustomizePart(part, index);
        }

        /// <summary>
        /// 특정 부위에 아머 장착
        /// </summary>
        private void CustomizePart(CustomPart part, int index)
        {
            string partName = GetStringCustomizePart(part, index);
            Transform root = GetRootTransform(part, partName);
            Transform target = root.Find(partName);

            root.gameObject.SetActive(true);
            
            if (part == CustomPart.Hair || part == CustomPart.FacialHair)
            {
                if (target)
                {
                    SwitchPart(part, target.gameObject);
                }
            }
            else
            {
                foreach (Transform child in root)
                {
                    if (child.name.EndsWith(part.ToString()))
                    {
                        SwitchPart(part, child.gameObject);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 커스터마이징 하는 부위의 이름을 반환하는 메서드 
        /// </summary>
        private string GetStringCustomizePart(CustomPart part, int index)
        {
            string value = part switch
            {
                CustomPart.Hair => $"Hair_{index + 1:D2}",
                CustomPart.FacialHair => $"FacialHair_{index:D2}",
                _ => index == 0 ? "Body" : $"Armor_{index:D3}" // index가 0인 경우에는 Body, 그외는 Armor_{index:D3}
            };
            return value;
        }

        /// <summary>
        /// 커스터마이징 하는 부위의 root을 반환하는 메서드 
        /// </summary>
        private Transform GetRootTransform(CustomPart part, string partName)
        {
            Transform root = part switch
            {
                CustomPart.Hair => customUI.custom.hairMeshRoot,
                CustomPart.FacialHair => customUI.custom.facialHairRoot,
                _ => customUI.custom.isFemale
                    ? customUI.custom.femaleMeshRoot.Find(partName)
                    : customUI.custom.maleMeshRoot.Find(partName)
            };
            return root;
        }

        /// <summary>
        /// 부위 교체 처리
        /// </summary>
        private void SwitchPart(CustomPart part, GameObject newPart)
        {
            if (equipped.TryGetValue(part, out GameObject oldPart) && oldPart != null)
            {
                if (oldPart != newPart)
                {
                    ParentActiveCheck(oldPart, part);
                }
            }

            newPart.SetActive(true);
            equipped[part] = newPart;
        }

        /// <summary>
        /// 특정 부위 해제
        /// </summary>
        private void Unequip(CustomPart part)
        {
            if (equipped.TryGetValue(part, out GameObject oldPart) && oldPart != null)
            {
                ParentActiveCheck(oldPart, part);
                equipped[part] = null;
            }
        }

        /// <summary>
        /// 이전 파츠의 부모의 자식들의 활성화 상태를 확인한 다음 전부 비활성화 되어있으면 부모도 비활성화 하는 메서드 <br/>
        /// 이전 파츠는 비활성화 처리한 다음 판단
        /// </summary>
        /// <param name="oldPart">바꾸기 전 파츠</param>
        /// <param name="partToCheck">바꾸기 전 파츠 부위</param>
        private void ParentActiveCheck(GameObject oldPart, CustomPart partToCheck)
        {
            var oldPartParent = oldPart.transform.parent;
            int childCount = oldPartParent.transform.childCount;
            bool isActive = false;
            oldPart.SetActive(false);

            // 이전 파츠의 부모가 가지고있는 자식 중 하나라도 활성화 된 상태가 있다면 부모도 활성화 된 상태 유지 아니면 비활성화
            if (partToCheck != CustomPart.Hair && partToCheck != CustomPart.FacialHair)
            {
                for (int i = 0; i < childCount; i++)
                {
                    if (oldPartParent.GetChild(i).gameObject.activeSelf)
                    {
                        isActive = true;
                        break; // 하나라도 true라면 활성화 된게 있으니 반복문 탈출
                    }
                }

                oldPartParent.gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// CurrentIndex를 초기화 하는 메서드 <br/>
        /// Apply 할 때 사용
        /// </summary>
        /// <param name="data"></param>
        private void InitCurrentIndexDir(CustomizeData data)
        {
            currentIndex = new Dictionary<CustomPart, int>()
            {
                { CustomPart.Hair, data.hair },
                { CustomPart.Chest, data.chest },
                { CustomPart.Leg, data.leg },
                { CustomPart.Arm, data.arm },
                { CustomPart.Waist, data.waist },
                { CustomPart.FacialHair, data.facialHair },
            };
        }

        private void Apply()
        {
            customUI.custom.Data = new CustomizeData
            {
                hair = currentIndex[CustomPart.Hair],
                chest = currentIndex[CustomPart.Chest],
                arm = currentIndex[CustomPart.Arm],
                waist = currentIndex[CustomPart.Waist],
                leg = currentIndex[CustomPart.Leg],
                facialHair = currentIndex[CustomPart.FacialHair],
                isFemale = customUI.custom.isFemale ? 1 : 0,
                bustSize = customUI.bustSizeSlider.value,
            };
        }
    }
}