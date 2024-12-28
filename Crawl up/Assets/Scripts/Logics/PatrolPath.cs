using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [Header("巡逻设置")]
    [Tooltip("巡逻方向，默认为水平方向(1,0)")]
    public Vector2 patrolDirection = Vector2.right;  // 巡逻方向
    [Tooltip("正向巡逻距离（从起点开始向patrolDirection方向移动的距离）")]
    public float forwardDistance = 5f;    // 正向距离
    [Tooltip("反向巡逻距离（从起点开始向-patrolDirection方向移动的距离）")]
    public float backwardDistance = 5f;   // 反向距离

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
        Vector2 normalizedDirection = patrolDirection.normalized;
        
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

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Vector3 currentPos = transform.position;
            Vector2 normalizedDirection = patrolDirection.normalized;
            Vector3 forward = currentPos + new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * forwardDistance;
            Vector3 backward = currentPos - new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * backwardDistance;

            // 绘制巡逻路径
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(forward, backward);

            // 绘制路径起点（中心点）
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(currentPos, 0.3f);

            // 绘制端点
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(forward, 0.2f);
            Gizmos.DrawWireSphere(backward, 0.2f);
            
            // 绘制方向指示
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(currentPos, currentPos + new Vector3(normalizedDirection.x, normalizedDirection.y, 0));

            // 绘制路径名称
            UnityEditor.Handles.Label(currentPos + Vector3.up * 0.5f, gameObject.name);
        }
    }
} 