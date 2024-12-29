using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CrawlSurface : MonoBehaviour
{
    private static bool isTreeColliderDisabled = false;
    private static List<CrawlSurface> treeInstances = new List<CrawlSurface>();
    private static bool isKeyHandled = false;

    [Header("允许的爬行类型")]
    public bool allowBasicCrawl = true;
    public bool allowGeckoCrawl = false;
    public bool allowTurtleCrawl = false;
    public bool allowSnakeCrawl = false;
    public bool allowCatCrawl = false;
    public bool allowChameleonCrawl = false;

    private SpriteRenderer spriteRenderer;
    private Tilemap tilemap;
    private Color originalColor;

    void Awake()
    {
        // 如果是树，添加到静态列表中
        if (gameObject.CompareTag("Tree"))
        {
            treeInstances.Add(this);
            
            // 获取渲染器组件
            spriteRenderer = GetComponent<SpriteRenderer>();
            tilemap = GetComponent<Tilemap>();
            
            // 保存原始颜色
            if (spriteRenderer)
            {
                originalColor = spriteRenderer.color;
            }
            else if (tilemap)
            {
                originalColor = tilemap.color;
            }
        }
    }

    void OnDestroy()
    {
        // 从列表中移除
        if (gameObject.CompareTag("Tree"))
        {
            treeInstances.Remove(this);
        }
    }

    void Update()
    {
        if (gameObject.CompareTag("Tree"))
        {
            // 当P键被按下时
            if (Input.GetKeyDown(KeyCode.P) && !isKeyHandled)
            {
                isTreeColliderDisabled = !isTreeColliderDisabled;
                UpdateAllTreeColliders();
                isKeyHandled = true;
            }
            // 当P键被释放时
            else if (Input.GetKeyUp(KeyCode.P))
            {
                isKeyHandled = false;
            }
        }
    }

    private static void UpdateAllTreeColliders()
    {
        foreach (var tree in treeInstances)
        {
            if (tree != null)
            {
                // 获取所有类型的碰撞体
                var collider2D = tree.GetComponent<Collider2D>();
                var tilemapCollider = tree.GetComponent<TilemapCollider2D>();
                var compositeCollider = tree.GetComponent<CompositeCollider2D>();

                // 禁用/启用所有找到的碰撞体
                if (collider2D != null)
                {
                    collider2D.enabled = !isTreeColliderDisabled;
                }
                if (tilemapCollider != null)
                {
                    tilemapCollider.enabled = !isTreeColliderDisabled;
                }
                if (compositeCollider != null)
                {
                    compositeCollider.enabled = !isTreeColliderDisabled;
                }

                // 切换Layer
                if (isTreeColliderDisabled)
                {
                    // 禁用时切换到Default层
                    tree.gameObject.layer = LayerMask.NameToLayer("Default");
                }
                else
                {
                    // 启用时切换回Surface层
                    tree.gameObject.layer = LayerMask.NameToLayer("Surface");
                }

                // 更新透明度
                tree.UpdateTreeVisual(isTreeColliderDisabled);
            }
        }
        Debug.Log($"树的碰撞体状态: {(isTreeColliderDisabled ? "禁用" : "启用")}, Layer: {(isTreeColliderDisabled ? "Default" : "Surface")}");
    }

    private void UpdateTreeVisual(bool isPassable)
    {
        if (spriteRenderer)
        {
            Color newColor = originalColor;
            newColor.a = isPassable ? 0.5f : 1f;
            spriteRenderer.color = newColor;
        }
        else if (tilemap)
        {
            Color newColor = originalColor;
            newColor.a = isPassable ? 0.5f : 1f;
            tilemap.color = newColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 如果是树根，显示提示
            if (gameObject.CompareTag("Tree"))
            {
                PromptManager.Instance.ShowTreePrompt();
            }

            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                string currentCrawlType = playerInput.currentCrawlName;
                List<string> allowedTypes = GetAllowedCrawlTypes();
                
                bool canCrawl = allowedTypes.Contains(currentCrawlType);
                playerInput.canCrawlOnCurrentSurface = canCrawl;
                
                if (canCrawl)
                {
                    Debug.Log($"可以使用 {currentCrawlType} 在 {gameObject.tag} 上爬行");
                }
                else
                {
                    Debug.Log($"不能使用 {currentCrawlType} 在 {gameObject.tag} 上爬行");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 如果是树根，隐藏提示
            if (gameObject.CompareTag("Tree"))
            {
                PromptManager.Instance.ResetTreePrompt();
            }

            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.canCrawlOnCurrentSurface = false;
            }
        }
    }

    public void CheckCrawlabilityForType(string crawlType, PlayerInput playerInput)
    {
        List<string> allowedTypes = GetAllowedCrawlTypes();
        bool canCrawl = allowedTypes.Contains(crawlType);
        playerInput.canCrawlOnCurrentSurface = canCrawl;
        
        if (canCrawl)
        {
            Debug.Log($"可以使用 {crawlType} 在 {gameObject.tag} 上爬行");
        }
        else
        {
            Debug.Log($"不能使用 {crawlType} 在 {gameObject.tag} 上爬行");
        }
    }

    private List<string> GetAllowedCrawlTypes()
    {
        List<string> allowedTypes = new List<string>();
        if (allowBasicCrawl) allowedTypes.Add("Basic");
        if (allowGeckoCrawl) allowedTypes.Add("Gecko");
        if (allowTurtleCrawl) allowedTypes.Add("Turtle");
        if (allowSnakeCrawl) allowedTypes.Add("Snake");
        if (allowCatCrawl) allowedTypes.Add("Cat");
        if (allowChameleonCrawl) allowedTypes.Add("Chameleon");
        return allowedTypes;
    }
} 