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
            allowedCrawlTypes = new List<string> { "Basic", "Gecko", "Turtle", "Snake", "Cat", "Chameleon" } 
        },
        new CrawlTypeMapping 
        { 
            surfaceTag = "Wall", 
            allowedCrawlTypes = new List<string> { "Basic", "Gecko", "Snake", "Cat" } 
        },
        new CrawlTypeMapping 
        { 
            surfaceTag = "Water", 
            allowedCrawlTypes = new List<string> { "Turtle", "Snake" } 
        },
        // new CrawlTypeMapping 
        // { 
        //     surfaceTag = "Ice", 
        //     allowedCrawlTypes = new List<string> { "Snake", "Chameleon" } 
        // }
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