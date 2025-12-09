using System.Data;
using TMPro;
using UnityEngine;

public class DescriptionManager : MonoBehaviour
{
    //管理卡片/地块描述框的显示与隐藏
    //地块的显示还没做
    public static DescriptionManager Instance { get; private set; }
    public GameObject descriptionPanel; //描述框背景（主物体）
    public TMP_Text descriptionText; //描述文本组件
    public Canvas parentCanvas; //父画布
    private Camera mainCamera;
    public RectTransform panelRectTransform;

    public bool isDragging = false;
    public Card draggingCard = null;
    public bool isShowingDescription = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        if (descriptionPanel == null) {
            Debug.LogWarning("Description Panel is not assigned.");
        }
        if (descriptionText == null) {
            Debug.LogWarning("Description Text is not assigned.");
        }
        HideDescription();
        mainCamera = Camera.main;
        panelRectTransform = descriptionPanel.GetComponent<RectTransform>();

    }

    private void Update() {
        if (isShowingDescription) {
            MoveDescriptionToMouse();
        }
    }

    public void ShowDescriptionOfLand(TileController tileController) {
        if(isDragging) {
            ShowDescriptionOfDraggingCardOnLand(tileController);
            return;
        }

        //根据地块数据生成描述文本
        if (tileController == null) {
            Debug.LogWarning("TileController component is not assigned.");
            return;
        }
        string displayText = $"({tileController.tileRow}, {tileController.tileCol})";
        if (tileController.land == null) {
            descriptionText.text = displayText + "\n 无";
            return;
        } else {
            LandScriptableObject landSO = DatabaseManager.Instance.GetLandSOByType(tileController.land.LandType);
            displayText += $"{landSO.LandName}\n";
            displayText += $"被动效果：{landSO.LandPassiveEffectInfo}({tileController.land.EnergyCounter}/{landSO.LandRequiredEnergy})\n";
            displayText += $"额外效果：{landSO.LandExtraEffectInfo}\n";
            displayText += $"等级:{landSO.LandLevel}\n";
        }
        if (tileController.land.storageCardType != MaterialType.NULL) {
            displayText += $"{GameData.HanizeMaterial(tileController.land.storageCardType)}:{tileController.land.storageCardNum}\n";
        }
        if (MapManager.Instance.AnimalMap != null) {
            Animal animal = MapManager.Instance.GetAnimalAt(tileController.tileRow,tileController.tileCol);
            if (animal != null) {
                displayText += $"{animal.AnimalName} HP:{animal.Health}\n";
            }
        }
        if (tileController.land.EnergyCounter != 0) {
            displayText += $"能量:{tileController.land.EnergyCounter}\n";
        }
        if (tileController.land.Soild != 0) {
            displayText += $"加固:{tileController.land.Soild}\n";
        }
        if (tileController.land.Hunterarea != 0) {
            displayText += $"猎圈:{tileController.land.Hunterarea}\n";
        }

        ShowDescription(displayText);
    }

    public void ShowDescriptionOfDraggingCardOnLand(TileController tileController) {
        //显示卡牌拖拽到地形上的描述
        //*需要投入多少相同的卡牌以产生效果
        //*所产生的效果是什么（举例：投入3木头以升级到 小镇）
        //*地块当前投入的资源（如果没有投入过资源就没有这一项，如果投入过且类型不同则变为红色）

        Land land = tileController.land;
        if (draggingCard == null) {
            Debug.LogWarning("No card is being dragged.");
            return;
        }
        string displayText = $"将{draggingCard.CardName}放置到{GameData.HanizeLandType(land.LandType)}上\n";
        
        LandScriptableObject landSO = DatabaseManager.Instance.GetLandSOByType(tileController.land.LandType);
        if(draggingCard.CardType == CardType.MATERIAL) {
            if(((MaterialCard)draggingCard).MaterialType == MaterialType.WOOD) {
            //显示所需数量和效果
                displayText += landSO.LandWoodUpgradeNeed + "个木头以";
                displayText += landSO.LandWoodUpgradeInfo + "\n";
            } else if(((MaterialCard)draggingCard).MaterialType == MaterialType.HAY){
                displayText += landSO.LandHayUpgradeNeed + "个干草以";
                displayText += landSO.LandHayUpgradeInfo + "\n";
            } else if(((MaterialCard)draggingCard).MaterialType == MaterialType.STONE) {
                displayText += landSO.LandStoneUpgradeNeed + "个石头以";
                displayText += landSO.LandStoneUpgradeInfo + "\n";
            } else if(((MaterialCard)draggingCard).MaterialType == MaterialType.MEAT) {
                displayText += landSO.LandMeatUpgradeNeed + "个肉以";
                displayText += landSO.LandMeatUpgradeInfo + "\n";
            }

            if (land.storageCardType != MaterialType.NULL) {
                if(land.storageCardType != ((MaterialCard)draggingCard).MaterialType) {
                    displayText += $"<color=red>当前已投入{land.storageCardNum}个{GameData.HanizeMaterial(land.storageCardType)}，与拖拽卡牌类型不同</color>\n";
                } else {
                    displayText += $"当前已投入{land.storageCardNum}个{GameData.HanizeMaterial(land.storageCardType)}，\n再投入{land.storageCardNum + 1}个即可达成效果\n";
                }
            }
        } else if(draggingCard.CardType == CardType.WEAPON) {
            if (MapManager.Instance.GetAnimalAt(tileController.tileRow, tileController.tileCol) != null) {
                displayText += "使用武器攻击地块上的生物\n";
            } else {
                displayText += "<color=red>地块上没有生物，无法使用武器</color>\n";
            }
        } else if(draggingCard.CardType == CardType.SKILL) {
            //技能卡
        }
        
        //还没实现
        ShowDescription(displayText);
    }

    public void ShowDescription(string description) {
        isShowingDescription = true;
        descriptionText.text = description;

        descriptionPanel.SetActive(true);
    }

    public void HideDescription() {
        isShowingDescription = false;
        descriptionPanel.SetActive(false);
    }

    public void MoveDescriptionToMouse() {

        //将描述框移动到鼠标位置
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition,
            mainCamera,
            out localPointerPosition)) {

            //根据鼠标相对于屏幕的左右，更改描述框的位置

            if(Input.mousePosition.x > Screen.width / 2) {
                //鼠标在屏幕右侧，描述框显示在左侧
                localPointerPosition.x -= panelRectTransform.rect.width / 2 + 20;
            } else {
                //鼠标在屏幕左侧，描述框显示在右侧
                localPointerPosition.x += panelRectTransform.rect.width / 2 + 20;
            }

            panelRectTransform.anchoredPosition = localPointerPosition;
        } else {
            Debug.LogWarning("RectTransform拖拽坐标转换失败");
        }
    }
}
