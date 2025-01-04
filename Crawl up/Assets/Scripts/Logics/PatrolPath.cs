#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [Header("巡逻设置")]
    [Tooltip("巡逻方向的角度，0度为水平向右，90度为向上")]
    [Range(0f, 360f)]
    public float patrolAngle = 0f;  // 巡逻方向的角度
    [Tooltip("正向巡逻距离（从起点开始向patrolDirection方向移动的距离）")]
    public float forwardDistance = 5f;    // 正向距离
    [Tooltip("反向巡逻距离（从起点开始向-patrolDirection方向移动的距离）")]
    public float backwardDistance = 5f;   // 反向距离
    [Tooltip("是否显示路径Gizmos")]
    public bool showGizmos = true;        // 是否显示Gizmos

    private Vector3 pathStartPosition;    // 路径起始位置
    private Vector3 forwardEndPoint;      // 正向终点
    private Vector3 backwardEndPoint;     // 反向终点

    private void Awake()
    {
        UpdatePathPoints();
    }

    private void UpdatePathPoints()
    {
        pathStartPosition = transform.position;
        
        // 根据角度计算方向
        float angleRad = patrolAngle * Mathf.Deg2Rad;
        Vector2 normalizedDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        
        // 计算端点
        forwardEndPoint = pathStartPosition + new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * forwardDistance;
        backwardEndPoint = pathStartPosition - new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * backwardDistance;
    }

    // 获取路径总长度
    public float GetTotalPathLength()
    {
        return forwardDistance + backwardDistance;
    }

    // 获取两个端点
    public void GetEndPoints(out Vector3 forward, out Vector3 backward)
    {
        forward = forwardEndPoint;
        backward = backwardEndPoint;
    }

    public Vector3 GetPositionAtProgress(float progress)
    {
        // 直接在两个端点之间进行插值
        return Vector3.Lerp(backwardEndPoint, forwardEndPoint, progress);
    }

    private void OnValidate()
    {
        UpdatePathPoints();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        UpdatePathPoints();  // 确保路径点是最新的

        // 绘制路径线
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(backwardEndPoint, forwardEndPoint);

        // 绘制端点
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pathStartPosition, 0.2f);  // 起点
        Gizmos.DrawWireSphere(forwardEndPoint, 0.2f);  // 终点

        // 绘制方向指示
        float angleRad = patrolAngle * Mathf.Deg2Rad;
        Vector2 normalizedDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        Gizmos.color = Color.white;
        Gizmos.DrawLine(pathStartPosition, 
            pathStartPosition + new Vector3(normalizedDirection.x, normalizedDirection.y, 0));
    }
#endif
} 