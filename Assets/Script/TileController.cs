using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using DG.Tweening;
/// <summary>
/// 地块数据 挂载在每个Tile物体上
/// </summary>
public class TileController : MonoBehaviour
{
    public Land land;
    public int tileRow;
    public int tileCol;

    [Header("UpdateAnimation Settings")]
    public float jumpHeight = 2f;      // 跳多高
    public float jumpDuration = 0.5f;  // 上升+下降时间

    [Header("Squash & Stretch Settings")]
    public float squashScaleY = 0.6f;  // 挤压高度
    public float squashScaleX = 1.25f; // 挤压宽度
    public float squashDuration = 0.12f;

    public float stretchScaleY = 1.25f; // 回弹拉长
    public float stretchScaleX = 0.8f;
    public float stretchDuration = 0.12f;

    private Vector3 originalScale;
    private bool isUpdateAnimationPlaying;

    private void Start() {
        // 延迟初始化，等待MapManager准备就绪
        StartCoroutine(DelayedInit());
        originalScale = transform.localScale;

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            PlayUpdateAnimation();
        }
    }

    private System.Collections.IEnumerator DelayedInit() {
        // 等待MapManager初始化完成
        while (MapManager.Instance == null || MapManager.Instance.LandMap == null || MapManager.Instance.LandMap.Count == 0) {
            yield return null;
        }

        // 检查坐标是否在有效范围内
        if (tileRow < 0 || tileRow >= MapManager.Instance.RowCount || tileCol < 0 || tileCol >= MapManager.Instance.ColCount) {
            Debug.LogError($"TileController {gameObject.name} 坐标超出范围: ({tileRow}, {tileCol})，地图大小: 3x4");
            yield break;
        }
        
        UpdateLand();
        MapManager.Instance.OnLandChanged.AddListener((row,col) => {
            if (row == tileRow && col == tileCol) {
                PlayUpdateAnimation();
                UpdateLand();
            }
        });
    }


    public Land GetLand() {
        // 检查坐标是否在有效范围内
        if (tileRow < 0 || tileRow >= MapManager.Instance.RowCount || tileCol < 0 || tileCol >= MapManager.Instance.ColCount) {
            Debug.LogError($"TileController {gameObject.name} 坐标超出范围: ({tileRow}, {tileCol})，地图大小: 3x4");
            return null;
        }
        return MapManager.Instance.GetLandAt(tileRow,tileCol);
    }

    void UpdateLand() {
        land = GetLand();
    }

    
    // 接受卡片放置
    public void OnCardPlaced(Card card) {
        if (land != null && card != null) {
            Debug.Log($"卡片 {card.CardName} 放置到格子 ({tileRow}, {tileCol})");
            
            // 应用卡片效果
            if (card is MaterialCard materialCard) {
                land.MaterialEffect(materialCard);
            } else {
                card.ApplyEffect(land);
            }
        }
    }

    public bool IsThisPlaceExist() {
        return MapManager.Instance.IsPlaceLegal(tileRow,tileCol);
    }

    private void OnMouseEnter() {
        // 显示地块描述
        if (DescriptionManager.Instance.isShowingDescription == false && GameManager.Instance.stateMachine.CurrentPhase == GamePhase.PlayerTurn) {
            DescriptionManager.Instance.ShowDescriptionOfLand(this);
        }
    }

    private void OnMouseExit() {
        // 隐藏地块描述
        DescriptionManager.Instance.HideDescription();
    }
    public void PlayUpdateAnimation()
    {
        if (isUpdateAnimationPlaying) return;
        isUpdateAnimationPlaying = true;

        float originalY = transform.position.y;

        DG.Tweening.Sequence seq = DOTween.Sequence();

        // 1. 跳上去
        seq.Append(transform.DOMoveY(originalY + jumpHeight, jumpDuration * 0.5f)
            .SetEase(Ease.OutQuad));

        // 2. 下落
        seq.Append(transform.DOMoveY(originalY, jumpDuration * 0.5f)
            .SetEase(Ease.InQuad));

        // 3. 落地瞬间：挤压（Squash）
        seq.Append(transform.DOScale(
                new Vector3(originalScale.x * squashScaleX, originalScale.y * squashScaleY, originalScale.z),
                squashDuration
            )
            .SetEase(Ease.OutQuad)
        );

        // 4. 回弹：拉长
        seq.Append(transform.DOScale(
                new Vector3(originalScale.x * stretchScaleX, originalScale.y * stretchScaleY, originalScale.z),
                stretchDuration
            )
            .SetEase(Ease.OutQuad)
        );

        // 5. 恢复正常比例
        seq.Append(transform.DOScale(originalScale, 0.08f));

        // 6. 动画结束后允许再次播放
        seq.OnComplete(() => isUpdateAnimationPlaying = false);
    }
}
