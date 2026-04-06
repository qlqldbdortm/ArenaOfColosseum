using System.IO;
using Colosseum.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.Customizing.Custom
{
    public class CharacterSpawner : MonoBehaviour
    {
        [Header("캐릭터 프리팹")] 
        public GameObject characterPrefab;

        [Header("JSON 파일 이름 (프로젝트 루트에 저장됨)")] 
        public string jsonFileName = "customizeData.json";
        
        [Header("UI 버튼")] 
        public Button generateJsonButton;
        public Button applyJsonButton;

        private GameObject currentCharacter;
        private readonly string projectRoot = Directory.GetParent(Application.dataPath)?.FullName+ "/Assets/";
        
        private void Start()
        {
            // 버튼 이벤트 연결
            if (generateJsonButton != null)
                generateJsonButton.onClick.AddListener(GenerateJson);

            if (applyJsonButton != null)
                applyJsonButton.onClick.AddListener(ApplyJsonToCharacter);
        }

        private void GenerateJson()
        {
            string path = Path.Combine(projectRoot, jsonFileName);
            var customizeData = CustomizingManager.Instance;

            CustomizeData data = new CustomizeData
            {
                hair = customizeData.currentIndex[CustomPart.Hair],
                facialHair = customizeData.currentIndex[CustomPart.FacialHair],
                chest = customizeData.currentIndex[CustomPart.Chest],
                arm = customizeData.currentIndex[CustomPart.Arm],
                waist = customizeData.currentIndex[CustomPart.Waist],
                leg = customizeData.currentIndex[CustomPart.Leg],
                isFemale = customizeData.customUI.custom.isFemale ?  1 : 0,
                bustSize = customizeData.customUI.bustSizeSlider.value,
            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log("JSON 파일 생성 완료: " + path);
        }

        private void ApplyJsonToCharacter()
        {
            // string path = Path.Combine(projectRoot, jsonFileName);
            //
            // if (!File.Exists(path))
            // {
            //     Debug.LogError("JSON 파일이 존재하지 않습니다. 먼저 Generate JSON 버튼으로 생성하세요.");
            //     return;
            // }
            //
            // string jsonText = File.ReadAllText(path);
            // Debug.Log(jsonText);
            // CustomizeData jsonData = JsonUtility.FromJson<CustomizeData>(jsonText);
            
            CustomizeData userData = GameDataManager.Instance.PlayerData.customizeData;

            // 캐릭터 스폰
            if (currentCharacter != null)
            {
                Destroy(currentCharacter);
            }
            
            currentCharacter = Instantiate(characterPrefab);
            
            CustomizeData customizeData = new CustomizeData() {
                hair = userData.hair,
                facialHair = userData.facialHair,
                chest = userData.chest,
                arm = userData.arm,
                waist = userData.waist,
                leg = userData.leg,
                isFemale = userData.isFemale,
                bustSize = userData.bustSize
            };

            CustomizingManager.Instance.UIPanelRefresh(customizeData);
            CustomizingManager.Instance.ApplyCustomization(customizeData);
            
            ApplyCustomizationToCharacter(currentCharacter.transform, customizeData);
            
            Debug.Log("JSON 데이터 적용 완료");
        }
        
        private void ApplyCustomizationToCharacter(Transform root, CustomizeData data)
        {
            // 캐릭터 루트에 맞춘 부모/메쉬 검색
            Transform maleMesh = root.Find("Mesh/Male");
            Transform femaleMesh = root.Find("Mesh/Female");
            Transform hair = root.Find("Mesh/Hair");
            Transform facialHair = root.Find("Mesh/Male/Facial_Hair");

            bool isFemale = data.isFemale == 1;

            maleMesh.gameObject.SetActive(!isFemale);
            femaleMesh.gameObject.SetActive(isFemale);
            
            for (int i = 0; i < hair.childCount; i++)
            {
                hair.GetChild(i).gameObject.SetActive(i == data.hair);
            }

            for (int i = 0; i < facialHair.childCount; i++)
            {
                if (!isFemale)
                {
                    facialHair.GetChild(i).gameObject.SetActive(i == data.facialHair);
                }
            }

            // 나머지 Armor Part 
            Transform meshRoot = isFemale ? femaleMesh : maleMesh;
            ApplyArmor(meshRoot, CustomPart.Chest, data.chest, isFemale);
            ApplyArmor(meshRoot, CustomPart.Arm, data.arm, isFemale);
            ApplyArmor(meshRoot, CustomPart.Waist, data.waist, isFemale);
            ApplyArmor(meshRoot, CustomPart.Leg, data.leg,  isFemale);

            // 가슴 사이즈
            Transform leftBust = root.Find("Skeleton_P09_Body/root/Hips/Spine/Chest/bust_01_L");
            Transform rightBust = root.Find("Skeleton_P09_Body/root/Hips/Spine/Chest/bust_01_R");
            leftBust.localScale = new Vector3(data.bustSize, data.bustSize, data.bustSize);
            rightBust.localScale = new Vector3(data.bustSize, data.bustSize, data.bustSize);
        }

        private void ApplyArmor(Transform meshRoot, CustomPart part, int index, bool isFemale)
        {
            string sex = isFemale ? "Fem" : "Male";
            Transform armorRoot = meshRoot.Find($"Armor_{index:D3}");
            string armor = $"{sex}_Armor_{index:D3}_{part.ToString()}";
            
            // Body인 경우는 armor가 없기 때문에 ? 를 사용해서 검사 
            armorRoot?.gameObject.SetActive(true);
            armorRoot?.Find(armor).gameObject.SetActive(true);
        }
    }
    
}