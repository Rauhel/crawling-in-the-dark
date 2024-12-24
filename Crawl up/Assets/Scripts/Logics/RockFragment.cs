using UnityEngine;

public class RockFragment : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.Instance.CollectFragment();
            // 碎片被收集后销毁
            Destroy(gameObject);
        }
    }
} 