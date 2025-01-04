using UnityEngine;

public class NpcVisualFeedback : MonoBehaviour
{
    public GameObject rangeIndicatorPrefab;
    private GameObject indicator;
    private NpcPatrol npcPatrol;

    void Start()
    {
        npcPatrol = GetComponent<NpcPatrol>();
        if (npcPatrol != null && rangeIndicatorPrefab != null)
        {
            indicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity);
            indicator.transform.SetParent(transform);
            UpdateRangeIndicator();
        }
    }

    void Update()
    {
        // 每帧更新指示器的位置和大小
        UpdateRangeIndicator();
    }

    void UpdateRangeIndicator()
    {
        if (indicator != null && npcPatrol != null)
        {
            // 获取NPC的朝向和偏移量
            float actualOffset = transform.localScale.x < 0 ? npcPatrol.horizontalOffset : -npcPatrol.horizontalOffset;
            Vector3 offsetPosition = transform.position + new Vector3(actualOffset, 0, 0);
            
            // 更新指示器的位置
            indicator.transform.position = offsetPosition;
            
            // 考虑NPC的缩放比例来计算指示器大小
            float npcScale = Mathf.Abs(transform.localScale.x);  // 假设x和y的缩放是一样的
            float horizontalScale = npcPatrol.horizontalDetectionRange * 2 / npcScale;
            float verticalScale = npcPatrol.verticalDetectionRange * 2 / npcScale;
            indicator.transform.localScale = new Vector3(horizontalScale, verticalScale, 1);
        }
    }

    void OnDestroy()
    {
        // 范围指示器会随着父对象自动销毁，不需要手动处理
    }
} 