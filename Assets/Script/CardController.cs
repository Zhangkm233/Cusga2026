using Mono.Cecil;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class CardController : MonoBehaviour
{
    public TMP_Text cardName;
    public TMP_Text cardDescription;
    public TMP_Text cardType;

    public Card card;
    public GameObject cardCanvas;
    public Canvas parentCanvas;
    public int indexOfCards;
    public bool isSelected = false;
    
    // 拖拽相关变量
    public bool isDragging = false;
    private Vector3 originalPosition;
    private Vector3 offset;
    private Camera mainCamera;
    private Color originalColor;
    private TileController hoveredTile;

    private RectTransform rectTransform;
    private Vector2 originalAnchoredPosition;
    private Vector2 dragOffset; // 用于RectTransform拖拽

    public CardController(Card card) {
        this.card = card;
    }

    private void Start() {
        Initialize();
    }

    public void Initialize() {
        cardCanvas = transform.GetComponentInChildren<Canvas>().gameObject;
        if (cardName == null) {
            cardName = cardCanvas.transform.Find("CardName").GetComponent<TMP_Text>();
        }
        if (cardDescription == null) {
            cardDescription = cardCanvas.transform.Find("CardDes").GetComponent<TMP_Text>();
        }
        if (cardType == null) {
            cardType = cardCanvas.transform.Find("CardType").GetComponent<TMP_Text>();
        }
        if (cardCanvas == null) {
            cardCanvas = transform.GetComponentInChildren<Canvas>().gameObject;
        }

        // 初始化拖拽相关
        mainCamera = Camera.main;
        originalColor = GetComponent<SpriteRenderer>().color;
        //UpdateCard();
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void UpdateCard() {

        // 如果card已经被外部设置（比如UIManager），直接使用
        if (card == null) {
            if (DeckManager.Instance.hand.Count <= indexOfCards) {
                card = null;
            } else {
                card = DeckManager.Instance.hand[indexOfCards];
            }
        }

        if (card != null) {
            this.gameObject.SetActive(true);
            cardName.text = card.CardName;
            cardDescription.text = card.Description;
            cardType.text = GameData.HanizeCardType(card.CardType);
        } else {
            // 不要禁用GameObject，只是清空文本
            cardName.text = "  ";
            cardDescription.text = "   ";
            cardType.text = " ";
        }
        this.GetComponent<SpriteRenderer>().sortingOrder = indexOfCards;
        cardCanvas.GetComponent<Canvas>().sortingOrder = indexOfCards;
        if (isSelected) {
            GetComponent<SpriteRenderer>().color = new Color(1f,1f,0.5f,1f); // ѡ��ʱ��Ϊ��ɫ
            UpdateSortingOrder(50);
        } else {
            GetComponent<SpriteRenderer>().color = Color.white; // δѡ��ʱ�ָ�Ϊ��ɫ
            UpdateSortingOrder();
        }
    }

    public void UpdateSortingOrder() {
        this.GetComponent<SpriteRenderer>().sortingOrder = indexOfCards * 2 + 1;
        cardCanvas.GetComponent<Canvas>().sortingOrder = indexOfCards * 2 + 2;
        this.GetComponent<BoxCollider>().layerOverridePriority = indexOfCards * 2 + 2;
    }

    public void UpdateSortingOrder(int order) {
        this.GetComponent<SpriteRenderer>().sortingOrder = order + 1;
        cardCanvas.GetComponent<Canvas>().sortingOrder = order + 2;
        this.GetComponent<BoxCollider>().layerOverridePriority = order +2;
    }
    
    // 鼠标按下开始拖拽
    private void OnMouseDown() {
        //MouseDownTransform();
        MouseDownRect();
    }

    private void MouseDownTransform() {
        Debug.Log($"OnMouseDown called on card: {card?.CardName}, isDragging: {isDragging}");
        if (card != null && !isDragging) {
            isDragging = true;
            originalPosition = transform.position;
            Debug.Log($"开始拖拽卡片: {card.CardName}, 原始位置: {originalPosition}");

            // 修正相机坐标转换
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z; // 使用卡片的深度
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = transform.position.z; // 保持卡片的Z轴位置


            offset = transform.position - mouseWorldPos;
            Debug.Log($"鼠标偏移量: {offset}");
            UpdateSortingOrder(100); // 拖拽时置顶
        }
    }
    private void MouseDownRect() {
        //使用rectTransform的拖拽方式
        Debug.Log($"OnMouseDown RectTransform called on card: {card?.CardName}, isDragging: {isDragging}");
        if (card != null && !isDragging) {
            isDragging = true;
            originalPosition = transform.position;
            originalAnchoredPosition = rectTransform.anchoredPosition;
            Debug.Log($"开始拖拽卡片: {card.CardName}, 原始位置: {originalPosition}");

            Vector2 localPointerPosition;
            // 将屏幕坐标转换为父Canvas的本地坐标，计算偏移量
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                Input.mousePosition,
                mainCamera,
                out localPointerPosition)) {
                dragOffset = rectTransform.anchoredPosition - localPointerPosition;
                Debug.Log($"RectTransform偏移量计算: 卡牌位置{rectTransform.anchoredPosition}, 鼠标位置{localPointerPosition}, 偏移量{dragOffset}");
            } else {
                // 如果转换失败，退回到使用世界坐标计算
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,transform.position.z - mainCamera.transform.position.z));
                dragOffset = transform.position - mouseWorldPos;
                Debug.Log($"使用世界坐标计算偏移量: {dragOffset}");
            }

            UpdateSortingOrder(100); // 拖拽时置顶
            Debug.Log($"开始RectTransform拖拽卡片: {card.CardName}");
        }
    }



    // 鼠标拖拽中
    private void OnMouseDrag() {
        //MouseDragTransform();
        MouseDragRect();
    }

    private void MouseDragTransform() {

        if (isDragging) {
            // 修正相机坐标转换
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z; // 使用卡片的深度
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = transform.position.z; // 保持卡片的Z轴位置

            Vector3 newPosition = mouseWorldPos + offset;
            transform.position = new Vector3(newPosition.x,newPosition.y,originalPosition.z); // 保持Z轴位置不变

            // 检查当前悬停的格子
            CheckHoveredTile();
        }
    }

    private void MouseDragRect() {
        if (isDragging) {
            // 将屏幕坐标转换为父Canvas的本地坐标
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                Input.mousePosition,
                mainCamera,
                out localPointerPosition)) {
                // 应用位置（加上偏移量以保持鼠标在卡牌上的点击位置）
                rectTransform.anchoredPosition = localPointerPosition + dragOffset;
                //Debug.Log($"RectTransform拖拽中: 新位置{rectTransform.anchoredPosition}");
            } else {
                Debug.LogWarning("RectTransform拖拽坐标转换失败");
            }

            // 检查当前悬停的格子
            CheckHoveredTile();
        }
    }

    // 鼠标释放
    private void OnMouseUp() {
        if (isDragging) {
            isDragging = false;
            Debug.Log($"OnMouseUp called on card: {card?.CardName}");
            
            // 检查是否拖拽到棋盘格子上
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0;
            
            Debug.Log($"鼠标屏幕位置: {Input.mousePosition}, 世界位置: {mouseWorldPos}");
            
            // 使用3D射线检测，排除卡片层
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            Debug.Log($"3D射线检测，检测到 {hits.Length} 个对象");
            
            // 过滤掉卡片对象，只保留可能的Tile对象
            List<RaycastHit> tileHits = new List<RaycastHit>();
            foreach (RaycastHit raycastHit in hits) {
                if (raycastHit.collider != null && 
                    !raycastHit.collider.CompareTag("CardGameobject") && 
                    !raycastHit.collider.name.Contains("Card")) {
                    tileHits.Add(raycastHit);
                    Debug.Log($"过滤后的对象: {raycastHit.collider.name}, 标签: {raycastHit.collider.tag}");
                }
            }
            hits = tileHits.ToArray();
            Debug.Log($"过滤后剩余 {hits.Length} 个对象");
            
            // 调试：查找所有Tile对象
            GameObject[] allTiles = GameObject.FindGameObjectsWithTag("TileGameobject");
            Debug.Log($"场景中找到 {allTiles.Length} 个TileGameobject标签的对象");
            
            // 查找所有带有TileController组件的对象
            TileController[] tileControllers = FindObjectsByType<TileController>(FindObjectsSortMode.None);
            Debug.Log($"通过组件找到 {tileControllers.Length} 个TileController对象");
            
            foreach (GameObject tile in allTiles) {
                Collider tileCollider = tile.GetComponent<Collider>();
                Debug.Log($"Tile: {tile.name}, 位置: {tile.transform.position}, Collider启用: {tileCollider?.enabled}, 标签: {tile.tag}");
            }
            
            // 如果射线检测失败，尝试使用OverlapSphere
            if (hits.Length == 0) {
                Collider[] colliders = Physics.OverlapSphere(mouseWorldPos, 0.5f);
                Debug.Log($"3D OverlapSphere检测位置: {mouseWorldPos}, 检测到 {colliders.Length} 个对象");
                
                // 过滤掉卡片对象
                List<Collider> tileColliders = new List<Collider>();
                foreach (Collider collider in colliders) {
                    if (collider != null && 
                        !collider.CompareTag("CardGameobject") && 
                        !collider.name.Contains("Card")) {
                        tileColliders.Add(collider);
                        Debug.Log($"OverlapSphere过滤后的对象: {collider.name}, 标签: {collider.tag}");
                    }
                }
                colliders = tileColliders.ToArray();
                Debug.Log($"OverlapSphere过滤后剩余 {colliders.Length} 个对象");
                
                for (int i = 0; i < colliders.Length; i++) {
                    Debug.Log($"  OverlapSphere对象 {i}: {colliders[i]?.name}, 标签: {colliders[i]?.tag}, 启用: {colliders[i]?.enabled}");
                }
                
                // 尝试通过OverlapSphere找到Tile
                foreach (Collider collider in colliders) {
                    if (collider != null) {
                        TileController tileController = collider.GetComponent<TileController>();
                        if (tileController != null) {
                            Debug.Log($"通过3D OverlapSphere找到TileController: {collider.name}");
                            // 成功放置到格子上
                            PlaceCardOnTile(tileController);
                            return;
                        }
                    }
                }
            }
            
            // 如果仍然没有找到Tile，尝试直接通过组件查找
            if (allTiles.Length > 0) {
                Debug.Log("尝试通过组件直接查找TileController");
                foreach (GameObject tile in allTiles) {
                    TileController tileController = tile.GetComponent<TileController>();
                    if (tileController != null) {
                        // 计算距离，如果距离很近就认为可以放置
                        float distance = Vector3.Distance(mouseWorldPos, tile.transform.position);
                        Debug.Log($"Tile {tile.name} 距离鼠标位置: {distance}");
                        if (distance < 2.0f) { // 2个单位内的距离
                            Debug.Log($"通过距离检测找到TileController: {tile.name}");
                            PlaceCardOnTile(tileController);
                            return;
                        }
                    }
                }
            }
            
            for (int i = 0; i < hits.Length; i++) {
                Debug.Log($"  3D对象 {i}: {hits[i].collider?.name}, 标签: {hits[i].collider?.tag}");
            }
            
            // 首先尝试通过标签检测
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider != null && hit.collider.CompareTag("TileGameobject")) {
                    TileController tileController = hit.collider.GetComponent<TileController>();
                    if (tileController != null) {
                        Debug.Log($"通过3D射线找到TileController，准备放置卡片");
                        // 成功放置到格子上
                        PlaceCardOnTile(tileController);
                        return;
                    } else {
                        Debug.LogError("未找到TileController组件");
                    }
                } else {
                    // 如果标签检测失败，尝试通过组件检测
                    foreach (RaycastHit hit2 in hits) {
                        if (hit2.collider != null && !hit2.collider.name.Contains("Card")) {
                            TileController tileController = hit2.collider.GetComponent<TileController>();
                            if (tileController != null) {
                                Debug.Log($"通过3D组件找到TileController: {hit2.collider.name}");
                                // 成功放置到格子上
                                PlaceCardOnTile(tileController);
                                return;
                            }
                        }
                    }
                    Debug.Log($"未击中有效格子，击中对象: {hit.collider?.name}, 标签: {hit.collider?.tag}");
                }
            } else {
                Debug.Log("3D射线检测未击中任何对象");
            }

            ReturnToOriginPosition();

            // 没有放置到有效位置，返回原位置
            //transform.position = originalPosition;
            //UpdateSortingOrder(); // 恢复原来的层级
            
            // 清除悬停效果
            //ClearHoverEffect();
        }
    }
    
    // 将卡片放置到格子上
    private void PlaceCardOnTile(TileController tileController) {
        if (card != null) {
            // 检查坐标是否在有效范围内
            if (tileController.tileRow < 0 || tileController.tileRow >= 3 || 
                tileController.tileCol < 0 || tileController.tileCol >= 4) {
                Debug.LogError($"TileController {tileController.name} 坐标超出范围: ({tileController.tileRow}, {tileController.tileCol})");
                return;
            }
            
            // 确保TileController有land数据
            if (tileController.land == null) {
                tileController.land = tileController.GetLand();
            }
            
            if (tileController.land != null) {
                // 应用卡片效果到土地上
                switch (card.CardType) {
                    case CardType.MATERIAL:
                        card.ApplyEffect(tileController.land);
                        break;
                    case CardType.WEAPON:
                        // 武器卡片需要检查动物占用
                        if (MapManager.Instance.IsPositionBeenOccupiedByAnimal(tileController.land.MapRow,tileController.land.MapCol)) {
                            card.ApplyEffect(tileController.land);
                        } else {
                            Debug.Log($"武器卡片 {card.CardName} 不能放置在无动物的格子上");
                            ReturnToOriginPosition();
                            return;
                        }
                        break;
                    case CardType.SKILL:
                        card.ApplyEffect(tileController.land);
                        break;
                    default:
                        Debug.Log("卡片无法使用");
                        ReturnToOriginPosition();
                        return;
                }
            } else {
                Debug.LogError($"TileController {tileController.name} 没有land数据");
                ReturnToOriginPosition();
                return;
            }
            
            // 从手牌中移除这张卡片
            int cardIndexInHand = DeckManager.Instance.hand.IndexOf(card);
            Debug.Log($"尝试移除卡片 {card.CardName}，在手牌中的索引: {cardIndexInHand}，手牌数量: {DeckManager.Instance.hand.Count}");
            if (cardIndexInHand >= 0) {
                DeckManager.Instance.hand.RemoveAt(cardIndexInHand);
                Debug.Log($"成功移除卡片，手牌数量变为: {DeckManager.Instance.hand.Count}");
            } else {
                Debug.LogError($"未找到卡片 {card.CardName} 在手牌中");
            }

            // 隐藏卡片
            //this.gameObject.SetActive(false);

            transform.position = originalPosition;
            UpdateSortingOrder();
            ClearHoverEffect();

            // 更新UI
            UIManager.Instance.UpdateCards();
            
            Debug.Log($"卡片 {card.CardName} 已放置到格子 ({tileController.tileRow}, {tileController.tileCol}) 并消失");
        }
    }
    
    // 检查悬停的格子
    private void CheckHoveredTile() {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;
        
        // 使用3D射线检测
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        TileController currentTile = null;
        
        // 首先尝试通过标签检测
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider != null && hit.collider.CompareTag("TileGameobject")) {
                currentTile = hit.collider.GetComponent<TileController>();
            } else if (hit.collider != null) {
                // 如果标签检测失败，尝试通过组件检测
                currentTile = hit.collider.GetComponent<TileController>();
            }
        } else {
            // 如果射线检测失败，尝试使用OverlapSphere
            Collider[] colliders = Physics.OverlapSphere(mouseWorldPos, 0.1f);
            foreach (Collider collider in colliders) {
                if (collider != null) {
                    TileController tileController = collider.GetComponent<TileController>();
                    if (tileController != null) {
                        currentTile = tileController;
                        break;
                    }
                }
            }
        }
        
        // 如果悬停的格子发生变化
        if (currentTile != hoveredTile) {
            // 清除之前格子的高亮
            if (hoveredTile != null) {
                SetTileHighlight(hoveredTile, false);
            }
            
            // 设置新格子的高亮
            hoveredTile = currentTile;
            if (hoveredTile != null) {
                SetTileHighlight(hoveredTile, true);
            }
        }
    }
    
    // 设置格子高亮效果
    private void SetTileHighlight(TileController tile, bool highlight) {
        if (tile != null) {
            SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
            if (tileRenderer != null) {
                if (highlight) {
                    tileRenderer.color = new Color(0.8f, 1f, 0.8f, 1f); // 淡绿色高亮
                } else {
                    tileRenderer.color = Color.white; // 恢复原色
                }
            }
        }
    }
    
    // 清除悬停效果
    private void ClearHoverEffect() {
        if (hoveredTile != null) {
            SetTileHighlight(hoveredTile, false);
            hoveredTile = null;
        }
    }

    private void ReturnToOriginPosition() {
        transform.position = originalPosition;
        UpdateSortingOrder(); // 恢复原来的层级
        ClearHoverEffect();
    }
}
