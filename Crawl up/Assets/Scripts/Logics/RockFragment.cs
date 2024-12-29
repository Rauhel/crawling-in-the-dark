using UnityEngine;

public class RockFragment : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 收集碎片
            PlayerManager.Instance.CollectFragment();
            
            // 设置当前位置为安全点
            PlayerManager.Instance.SetSafePoint(transform.position);
            Debug.Log($"收集碎片并设置安全点: {transform.position}");
            
            // 碎片被收集后销毁
            Destroy(gameObject);
        }
    }
} 