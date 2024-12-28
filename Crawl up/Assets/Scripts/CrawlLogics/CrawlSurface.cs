using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CrawlTypeMapping
{
    public string surfaceTag;
    public List<string> allowedCrawlTypes;
}

public class CrawlSurface : MonoBehaviour
{
    public List<CrawlTypeMapping> crawlTypeMappings;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 如果是树根，显示提示
            if (gameObject.CompareTag("Tree"))
            {
                PromptManager.Instance.ShowTreePrompt();
            }

            PlayerInput playerInput = collision.gameObject.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                string currentTag = gameObject.tag;
                string currentCrawlType = playerInput.currentCrawlName;

                // 查找当前表面对应的映射
                CrawlTypeMapping mapping = crawlTypeMappings.Find(m => m.surfaceTag == currentTag);
                
                if (mapping != null)
                {
                    bool canCrawl = mapping.allowedCrawlTypes.Contains(currentCrawlType);
                    playerInput.canCrawlOnCurrentSurface = canCrawl;
                    
                    if (canCrawl)
                    {
                        Debug.Log($"可以使用 {currentCrawlType} 在 {currentTag} 上爬行");
                    }
                    else
                    {
                        Debug.Log($"不能使用 {currentCrawlType} 在 {currentTag} 上爬行");
                    }
                }
                else
                {
                    Debug.LogWarning($"未找到标签 {currentTag} 的爬行类型映射");
                    playerInput.canCrawlOnCurrentSurface = false;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 如果是树根，隐藏提示
            if (gameObject.CompareTag("Tree"))
            {
                PromptManager.Instance.HideTreePrompt();
            }

            PlayerInput playerInput = collision.gameObject.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.canCrawlOnCurrentSurface = false;
            }
        }
    }

    // 添加新方法，供外部直接检查特定爬行类型
    public void CheckCrawlabilityForType(string crawlType, PlayerInput playerInput)
    {
        string currentTag = gameObject.tag;
        
        // 查找当前表面对应的映射
        CrawlTypeMapping mapping = crawlTypeMappings.Find(m => m.surfaceTag == currentTag);
        
        if (mapping != null)
        {
            bool canCrawl = mapping.allowedCrawlTypes.Contains(crawlType);
            playerInput.canCrawlOnCurrentSurface = canCrawl;
            
            if (canCrawl)
            {
                Debug.Log($"可以使用 {crawlType} 在 {currentTag} 上爬行");
            }
            else
            {
                Debug.Log($"不能使用 {crawlType} 在 {currentTag} 上爬行");
            }
        }
        else
        {
            Debug.LogWarning($"未找到标签 {currentTag} 的爬行类型映射");
            playerInput.canCrawlOnCurrentSurface = false;
        }
    }
} 