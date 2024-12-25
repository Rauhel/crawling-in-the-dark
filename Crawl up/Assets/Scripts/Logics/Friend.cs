using UnityEngine;

public class Friend : MonoBehaviour
{
    private void OnDialogueStart()
    {
        // 当开始对话时，设置该位置为安全点
        PlayerManager.Instance.SetSafePoint(transform.position);
        // ... 其他对话相关代码 ...
    }
} 