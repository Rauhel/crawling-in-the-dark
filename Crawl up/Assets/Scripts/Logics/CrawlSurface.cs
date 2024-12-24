using UnityEngine;
using System.Collections.Generic;

public class CrawlSurface : MonoBehaviour
{
    [System.Serializable]
    public class CrawlTypeMapping
    {
        public string surfaceTag;
        public List<string> allowedCrawlTypes = new List<string>();
    }

    public List<CrawlTypeMapping> crawlTypeMappings = new List<CrawlTypeMapping>()
    {
        new CrawlTypeMapping 
        { 
            surfaceTag = "Ground", 
            allowedCrawlTypes = new List<string> { "Basic", "Snake", "Cat" } 
        },
        new CrawlTypeMapping 
        { 
            surfaceTag = "Wall", 
            allowedCrawlTypes = new List<string> { "Gecko", "Chameleon" } 
        },
        // 可以根据需要添加更多映射
    };

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCrawlability(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckCrawlability(collision);
    }

    private void CheckCrawlability(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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
} 