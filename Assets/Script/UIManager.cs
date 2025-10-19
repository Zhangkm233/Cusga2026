using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class UIManager : MonoBehaviour
{
    public GameObject[] cards;
    public TMP_Text DeckCount;
    public TMP_Text HandCount;
    public TMP_Text ExtraDrawCount;

    [Header("Card Dealing Animation")]
    [SerializeField] private RectTransform deckAnchor;
    [SerializeField] private float dealDuration = 0.35f;
    [SerializeField] private float dealInterval = 0.08f;
    [SerializeField] private SplineContainer dealSpline;
    [SerializeField, Range(0f, 1f)] private float dealSplineStart = 0f;
    [SerializeField] private AnimationCurve dealCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Header("Card Scale Animation")]
    [SerializeField] private bool enableDealScale = true;
    [SerializeField] private float dealStartScaleMultiplier = 0.6f;
    [SerializeField] private AnimationCurve dealScaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Hand Layout Curve")]
    [SerializeField] private bool arrangeCardsOnCurve = false;
    [SerializeField] private SplineContainer handSpline;
    [SerializeField] private AnimationCurve handCurveDistribution = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private RectTransform[] cardRects;
    private Vector2[] cardTargetAnchoredPositions;
    private Vector3[] cardTargetLocalScales;
    private CardController[] cardControllers;
    private Coroutine dealRoutine;
    private float[] cardTargetSplineParameters;

    public static UIManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    void Start() {
        EnsureCardArray();
        CacheCardReferences();
        UpdateCardsSortingOrder();
    }

    [ContextMenu("UpdateCards")]
    public void UpdateCards() {
        EnsureCardArray();
        CacheCardReferences();

        int activeCardCount = Mathf.Min(DeckManager.Instance.hand.Count,cards.Length);
        ApplyHandCurveLayout(activeCardCount);

        for (int i = 0;i < activeCardCount;i++) {
            if (cards[i] == null) {
                Debug.LogWarning($"cards[{i}] is null, skipping update.");
                continue;
            }
            CardController cardController = cardControllers[i];
            
            // 设置正确的索引
            cardController.indexOfCards = i;            
            // 总是设置card数据，但只在非拖拽时更新位置
            cardController.card = DeckManager.Instance.hand[i];
            if (!cardController.isDragging) {
                cardController.UpdateCard();
            } else {
                // 拖拽时只更新文本信息，不更新位置
                if (cardController.card != null) {
                    cardController.cardName.text = cardController.card.CardName;
                    cardController.cardDescription.text = cardController.card.Description;
                    cardController.cardType.text = GameData.HanizeCardType(cardController.card.CardType);
                }
            }
        }
        
        // 隐藏多余的卡片
        for (int i = DeckManager.Instance.hand.Count; i < cards.Length; i++) {
            if (cards[i] != null) {
                cards[i].SetActive(false);
            }
        }
        
        UpdateCardsSortingOrder();
    }

    public void UpdateCardsSortingOrder() {
        if (cards == null) {
            return;
        }
        foreach (var card in cards) {
            card.GetComponent<CardController>().UpdateSortingOrder();
        }
    }
    public void PlayDealAnimation() {
        EnsureCardArray();
        CacheCardReferences();
        int activeCardCount = Mathf.Min(DeckManager.Instance.hand.Count, cards.Length);
        ApplyHandCurveLayout(activeCardCount);

        if (dealRoutine != null) {
            StopCoroutine(dealRoutine);
        }
        dealRoutine = StartCoroutine(DealCardsRoutine());
    }
    
    /*
    public void UpdateTiles() {
        // ��ȡ����TileGameobject���󲢸������ǵ�״̬
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("TileGameobject");
        foreach (GameObject tile in tiles) {
            tile.GetComponent<TileController>().UpdateTile();
        }
    }
    */

    /*
    public void UpdateUI() {
        DeckCount.text = "�ƿ�: " + DeckManager.Instance.deck.Count;
        HandCount.text = "����: " + DeckManager.Instance.hand.Count;
        ExtraDrawCount.text = "�������: " + GameData.extraDrawNum;
    }
    */

    public void CancelAllSelect() {
        foreach (GameObject card in cards) {
            CardController cardController = card.GetComponent<CardController>();
            if (cardController != null) {
                cardController.isSelected = false;
                cardController.UpdateCard();
            }
        }
    }

    private IEnumerator DealCardsRoutine() {
        var hand = DeckManager.Instance.hand;
        if (hand == null || hand.Count == 0) {
            yield break;
        }

        int cardCount = Mathf.Min(hand.Count, cards.Length);

        // 预先隐藏所有卡牌，避免先全部出现
        for (int i = 0; i < cards.Length; i++) {
            if (cards[i] != null) {
                cards[i].SetActive(false);
            }
        }

        ApplyHandCurveLayout(cardCount);

        Vector3? deckWorldPosition = deckAnchor != null ? deckAnchor.TransformPoint(Vector3.zero) : (Vector3?)null;
        SplineContainer activeDealSpline = dealSpline != null ? dealSpline : (arrangeCardsOnCurve ? handSpline : null);
        bool useSplineForDeal = activeDealSpline != null;
        float pathStartT = Mathf.Clamp01(dealSplineStart);

        for (int i = 0; i < cardCount; i++) {
            if (cards[i] == null) {
                continue;
            }

            var controller = cardControllers[i];
            if (controller == null) {
                continue;
            }

            controller.indexOfCards = i;
            controller.card = hand[i];
            controller.UpdateCard();

            cards[i].SetActive(true);

            RectTransform rect = cardRects[i];
            Vector2 targetPos = cardTargetAnchoredPositions[i];
            if (rect == null) {
                continue;
            }
            RectTransform parentRect = rect.parent as RectTransform;
            float targetSplineT = cardTargetSplineParameters != null && i < cardTargetSplineParameters.Length ? cardTargetSplineParameters[i] : 1f;
            float clampedTargetSplineT = Mathf.Clamp01(targetSplineT);

            Vector2 startPos;
            if (useSplineForDeal && parentRect != null) {
                Vector3 worldStart = activeDealSpline.EvaluatePosition(pathStartT);
                startPos = WorldToAnchored(worldStart, parentRect);
            } else if (deckWorldPosition.HasValue && parentRect != null) {
                startPos = WorldToAnchored(deckWorldPosition.Value, parentRect);
            } else {
                startPos = targetPos;
            }
            rect.anchoredPosition = startPos;
            controller.UpdateSortingOrder();
            Vector3 targetScale = (cardTargetLocalScales != null && i < cardTargetLocalScales.Length) ? cardTargetLocalScales[i] : rect.localScale;
            Vector3 startScale = enableDealScale ? targetScale * dealStartScaleMultiplier : targetScale;
            rect.localScale = startScale;

            float elapsed = 0f;
            while (elapsed < dealDuration) {
                elapsed += Time.deltaTime;
                float normalized = Mathf.Clamp01(dealDuration <= Mathf.Epsilon ? 1f : elapsed / dealDuration);
                float eased = dealCurve != null ? dealCurve.Evaluate(normalized) : normalized;
                if (useSplineForDeal && parentRect != null) {
                    float currentT = Mathf.Lerp(pathStartT, clampedTargetSplineT, eased);
                    Vector3 worldPos = activeDealSpline.EvaluatePosition(currentT);
                    rect.anchoredPosition = WorldToAnchored(worldPos, parentRect);
                } else {
                    rect.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, eased);
                }
                if (enableDealScale) {
                    float scaleT = dealScaleCurve != null ? dealScaleCurve.Evaluate(normalized) : normalized;
                    rect.localScale = Vector3.LerpUnclamped(startScale, targetScale, scaleT);
                }
                yield return null;
            }
            rect.anchoredPosition = targetPos;
            rect.localScale = targetScale;

            if (dealInterval > 0f) {
                yield return new WaitForSeconds(dealInterval);
            }
        }

        UpdateCardsSortingOrder();
        dealRoutine = null;
    }

    private void EnsureCardArray() {
        if (cards != null && cards.Length > 0) {
            bool hasValidEntry = false;
            for (int i = 0; i < cards.Length; i++) {
                if (cards[i] != null) {
                    hasValidEntry = true;
                    break;
                }
            }
            if (hasValidEntry) {
                return;
            }
        }

        GameObject[] foundCards = GameObject.FindGameObjectsWithTag("CardGameobject");
        Array.Sort(foundCards, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        cards = foundCards;
    }

    private void CacheCardReferences() {
        if (cards == null || cards.Length == 0) {
            cardRects = Array.Empty<RectTransform>();
            cardTargetAnchoredPositions = Array.Empty<Vector2>();
            cardTargetLocalScales = Array.Empty<Vector3>();
            cardTargetSplineParameters = Array.Empty<float>();
            cardControllers = Array.Empty<CardController>();
            return;
        }

        int length = cards.Length;
        if (cardRects == null || cardRects.Length != length) {
            cardRects = new RectTransform[length];
            cardTargetAnchoredPositions = new Vector2[length];
            cardTargetLocalScales = new Vector3[length];
            cardTargetSplineParameters = new float[length];
            cardControllers = new CardController[length];
        }

        for (int i = 0; i < length; i++) {
            if (cards[i] == null) {
                cardRects[i] = null;
                cardControllers[i] = null;
                continue;
            }

            if (cardControllers[i] == null) {
                cardControllers[i] = cards[i].GetComponent<CardController>();
            }

            RectTransform rect = cards[i].GetComponent<RectTransform>();
            if (rect != null) {
                cardRects[i] = rect;
                cardTargetAnchoredPositions[i] = rect.anchoredPosition;
                cardTargetLocalScales[i] = rect.localScale;
            } else {
                cardRects[i] = null;
                cardTargetLocalScales[i] = Vector3.one;
            }
        }
    }

    private void ApplyHandCurveLayout(int activeCardCount) {
        if (!arrangeCardsOnCurve || cards == null || cards.Length == 0 || handSpline == null) {
            return;
        }

        int slotCount = cards.Length;
        float denominator = Mathf.Max(1, activeCardCount - 1);

        for (int i = 0; i < slotCount; i++) {
            if (cards[i] == null) {
                continue;
            }

            float normalizedIndex;
            if (activeCardCount <= 0) {
                normalizedIndex = slotCount <= 1 ? 0.5f : (float)i / Mathf.Max(1, slotCount - 1);
            } else if (activeCardCount == 1) {
                normalizedIndex = 0.5f;
            } else {
                int clampedIndex = Mathf.Clamp(i, 0, activeCardCount - 1);
                normalizedIndex = (float)clampedIndex / denominator;
            }

            float distribution = handCurveDistribution != null ? handCurveDistribution.Evaluate(Mathf.Clamp01(normalizedIndex)) : normalizedIndex;
            Vector3 worldPosition = handSpline != null ? (Vector3)handSpline.EvaluatePosition(distribution) : Vector3.zero;

            RectTransform rect = cardRects != null && i < cardRects.Length ? cardRects[i] : null;
            RectTransform parentRect = rect != null ? rect.parent as RectTransform : null;
            Vector2 targetPosition = parentRect != null ? WorldToAnchored(worldPosition, parentRect) : new Vector2(worldPosition.x, worldPosition.y);

            if (i < cardTargetAnchoredPositions.Length) {
                cardTargetAnchoredPositions[i] = targetPosition;
            }
            if (i < cardTargetSplineParameters.Length) {
                cardTargetSplineParameters[i] = Mathf.Clamp01(distribution);
            }

            if (rect != null) {
                CardController controller = cardControllers != null && i < cardControllers.Length ? cardControllers[i] : null;
                if (controller == null || !controller.isDragging) {
                    rect.anchoredPosition = targetPosition;
                }
            }
        }
    }

    private Vector2 WorldToAnchored(Vector3 worldPosition, RectTransform referenceRect) {
        if (referenceRect == null) {
            return new Vector2(worldPosition.x, worldPosition.y);
        }
        Vector3 localPoint = referenceRect.InverseTransformPoint(worldPosition);
        return new Vector2(localPoint.x, localPoint.y);
    }
}
