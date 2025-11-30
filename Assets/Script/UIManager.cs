using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class UIManager : MonoBehaviour
{
    public GameObject[] cards;
    public GameObject cardPrefab;

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
    [SerializeField] private bool alignHandToSplineTangent = true;
    [SerializeField] private bool alignDealToSplineTangent = true;

    private RectTransform[] cardRects;
    private Vector2[] cardTargetAnchoredPositions;
    private Vector3[] cardTargetLocalScales;
    private CardController[] cardControllers;
    private Coroutine dealRoutine;
    private float[] cardTargetSplineParameters;
    private float[] cardTargetAngles;

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
        DeckManager.Instance.ShowHand();
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

    private IEnumerator DealCardsRoutine() {
        Debug.Log("开始发牌动画");
        GameData.IsCardAnimationPlaying = true;
        var hand = DeckManager.Instance.hand;
        if (hand == null || hand.Count == 0) {
            Debug.Log("手牌为空，跳过发牌动画");
            // 隐藏所有卡牌
            for (int i = 0;i < cards.Length;i++) {
                if (cards[i] != null) {
                    cards[i].SetActive(false);
                }
            }
            GameData.IsCardAnimationPlaying = false;
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
            //RectTransform parentRect = GameObject.Find("CardCanvas").GetComponent<RectTransform>();
            float targetSplineT = cardTargetSplineParameters != null && i < cardTargetSplineParameters.Length ? cardTargetSplineParameters[i] : 1f;
            float clampedTargetSplineT = Mathf.Clamp01(targetSplineT);
            float targetAngle = cardTargetAngles != null && i < cardTargetAngles.Length ? cardTargetAngles[i] : rect.localEulerAngles.z;
            float startAngle = targetAngle;

            Vector2 startPos;
            if (useSplineForDeal && parentRect != null) {
                Vector3 worldStart = activeDealSpline.EvaluatePosition(pathStartT);
                startPos = WorldToAnchored(worldStart, parentRect);
                if (alignDealToSplineTangent) {
                    Vector3 worldTangentStart = (Vector3)activeDealSpline.EvaluateTangent(pathStartT);
                    Vector2 localTangentStart = WorldTangentToLocal(worldTangentStart, parentRect);
                    startAngle = DirectionToAngle(localTangentStart, startAngle);
                }
            } else if (deckWorldPosition.HasValue && parentRect != null) {
                startPos = WorldToAnchored(deckWorldPosition.Value, parentRect);
            } else {
                startPos = targetPos;
            }
            rect.anchoredPosition = startPos;
            rect.localRotation = Quaternion.Euler(0f,0f,startAngle);
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
                    if (alignDealToSplineTangent) {
                        Vector3 worldTangent = (Vector3)activeDealSpline.EvaluateTangent(currentT);
                        Vector2 localTangent = WorldTangentToLocal(worldTangent, parentRect);
                        float currentAngle = DirectionToAngle(localTangent, targetAngle);
                        rect.localRotation = Quaternion.Euler(0f,0f,currentAngle);
                    }
                } else {
                    rect.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, eased);
                    if (alignHandToSplineTangent) {
                        float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, eased);
                        rect.localRotation = Quaternion.Euler(0f,0f,currentAngle);
                    }
                }
                if (enableDealScale) {
                    float scaleT = dealScaleCurve != null ? dealScaleCurve.Evaluate(normalized) : normalized;
                    rect.localScale = Vector3.LerpUnclamped(startScale, targetScale, scaleT);
                }
                yield return null;
            }
            rect.anchoredPosition = targetPos;
            rect.localRotation = Quaternion.Euler(0f,0f,targetAngle);
            rect.localScale = targetScale;

            if (dealInterval > 0f) {
                yield return new WaitForSeconds(dealInterval);
            }
        }

        UpdateCardsSortingOrder();
        dealRoutine = null;
        GameData.IsCardAnimationPlaying = false;
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
        if(foundCards.Length <= DeckManager.Instance.hand.Count) {
            //实例化足够的卡牌对象
            int cardsToInstantiate = DeckManager.Instance.hand.Count - foundCards.Length;
            for (int i = 0;i < cardsToInstantiate;i++) {
                Instantiate(cardPrefab,this.transform);
            }
            foundCards = GameObject.FindGameObjectsWithTag("CardGameobject");
        }
        Array.Sort(foundCards, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        cards = foundCards;
    }

    private void CacheCardReferences() {
        if (cards == null || cards.Length == 0) {
            cardRects = Array.Empty<RectTransform>();
            cardTargetAnchoredPositions = Array.Empty<Vector2>();
            cardTargetLocalScales = Array.Empty<Vector3>();
            cardTargetSplineParameters = Array.Empty<float>();
            cardTargetAngles = Array.Empty<float>();
            cardControllers = Array.Empty<CardController>();
            return;
        }

        int length = cards.Length;
        if (cardRects == null || cardRects.Length != length) {
            cardRects = new RectTransform[length];
            cardTargetAnchoredPositions = new Vector2[length];
            cardTargetLocalScales = new Vector3[length];
            cardTargetSplineParameters = new float[length];
            cardTargetAngles = new float[length];
            cardControllers = new CardController[length];
        }

        for (int i = 0; i < length; i++) {
            if (cards[i] == null) {
                cardRects[i] = null;
                cardControllers[i] = null;
                cardTargetAngles[i] = 0f;
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
                cardTargetAngles[i] = rect.localEulerAngles.z;
            } else {
                cardRects[i] = null;
                cardTargetLocalScales[i] = Vector3.one;
                cardTargetAngles[i] = 0f;
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
            float fallbackAngle = rect != null ? rect.localEulerAngles.z : 0f;
            float targetAngle = fallbackAngle;

            if (alignHandToSplineTangent && parentRect != null) {
                Vector3 worldTangent = (Vector3)handSpline.EvaluateTangent(distribution);
                Vector2 localTangent = WorldTangentToLocal(worldTangent, parentRect);
                targetAngle = DirectionToAngle(localTangent, fallbackAngle);
            }

            if (i < cardTargetAnchoredPositions.Length) {
                cardTargetAnchoredPositions[i] = targetPosition;
            }
            if (i < cardTargetSplineParameters.Length) {
                cardTargetSplineParameters[i] = Mathf.Clamp01(distribution);
            }
            if (i < cardTargetAngles.Length) {
                cardTargetAngles[i] = targetAngle;
            }

            if (rect != null) {
                CardController controller = cardControllers != null && i < cardControllers.Length ? cardControllers[i] : null;
                if (controller == null || !controller.isDragging) {
                    rect.anchoredPosition = targetPosition;
                    if (alignHandToSplineTangent) {
                        rect.localRotation = Quaternion.Euler(0f,0f,targetAngle);
                    }
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

    private Vector2 WorldTangentToLocal(Vector3 worldTangent, RectTransform referenceRect) {
        if (referenceRect == null) {
            return new Vector2(worldTangent.x, worldTangent.y);
        }
        Vector3 localVector = referenceRect.InverseTransformVector(worldTangent);
        return new Vector2(localVector.x, localVector.y);
    }

    private float DirectionToAngle(Vector2 direction, float fallbackAngle) {
        if (direction.sqrMagnitude < 1e-6f) {
            return fallbackAngle;
        }
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
