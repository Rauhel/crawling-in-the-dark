using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    private Transform target;  // 要跟随的目标（玩家）
    private Vector3 offset;    // 与目标的偏移量

    void Start()
    {
        // 获取玩家transform
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target != null)
        {
            // 记录初始偏移量
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 只更新位置，保持自身的旋转
            transform.position = target.position + offset;
            // 确保旋转保持为0
            transform.rotation = Quaternion.identity;
        }
    }
} 