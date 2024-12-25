using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private bool isGrounded = false;
    private float fallTimer = 0f;
    public float maxFallTime = 3f;
    public float fallSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (!isGrounded)
        {
            Debug.Log("Player is falling");
            // 禁用玩家输入
            playerInput.enabled = false;
            // 施加下落速度
            rb.velocity = new Vector2(0, -fallSpeed);
            
            // 增加下落时间
            fallTimer += Time.deltaTime;
            
            // 检查是否超过最大下落时间
            if (fallTimer >= maxFallTime)
            {
                // 通知事件中心玩家死亡
                EventCenter.Instance.Publish(EventCenter.EVENT_PLAYER_DIED);
                fallTimer = 0f;  // 重置计时器
            }
        }
        else
        {
            // 重新启用玩家输入
            playerInput.enabled = true;
            // 重置下落计时器
            fallTimer = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
} 