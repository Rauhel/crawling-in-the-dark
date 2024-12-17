using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool chameleon;
    private BasicCrawl basicCrawl;
    private GeckoCrawl geckoCrawl;
    private ChameleonCrawl chameleonCrawl;
    private TurtleCrawl turtleCrawl;
    private CatCrawl catCrawl;
    private SnakeCrawl snakeCrawl;

    // Start is called before the first frame update
    void Start()
    {
        basicCrawl = GetComponent<BasicCrawl>();
        geckoCrawl = GetComponent<GeckoCrawl>();
        chameleonCrawl = GetComponent<ChameleonCrawl>();
        turtleCrawl = GetComponent<TurtleCrawl>();
        catCrawl = GetComponent<CatCrawl>();
        snakeCrawl = GetComponent<SnakeCrawl>();


        // 检查是否成功获取组件
        if (basicCrawl == null)
        {
            Debug.LogError("BasicCrawl component not found on the player.");
        }
        if (geckoCrawl == null)
        {
            Debug.LogError("GeckoCrawl component not found on the player.");
        }
        if (chameleonCrawl == null)
        {
            Debug.LogError("ChameleonCrawl component not found on the player.");
        }
        if (turtleCrawl == null)
        {
            Debug.LogError("TurtleCrawl component not found on the player.");
        }
        if (catCrawl == null)
        {
            Debug.LogError("CatCrawl component not found on the player.");
        }
        if (snakeCrawl == null)
        {
            Debug.LogError("SnakeCrawl component not found on the player.");
        }


        // 订阅事件
        if (CrawlManager.Instance != null)
        {
            CrawlManager.Instance.OnLearnCrawl += OnLearnCrawl;
        }

        // 初始化爬行方式
        CheckCrawlAbilities();
    }

    // Update is called once per frame
    void Update()
    {
        // 这里可以添加其他逻辑
    }

    // 方法：处理学会新的爬行方式事件
    private void OnLearnCrawl(string crawlType)
    {
        CheckCrawlAbilities();
    }

    // 方法：检查玩家的爬行能力
    private void CheckCrawlAbilities()
    {
        // 检查玩家是否学会了 Basic Crawl
        if (CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedBasicCrawl)
        {
            if (basicCrawl != null)
            {
                basicCrawl.enabled = true;
            }
        }

        // 检查玩家是否学会了 Gecko Crawl
        if (CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedGeckoCrawl)
        {
            if (geckoCrawl != null)
            {
                geckoCrawl.enabled = true;
            }
        }

        // 检查玩家是否学会了 Chameleon Crawl
        if (CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedChameleonCrawl)
        {
            if (chameleonCrawl != null)
            {
                chameleonCrawl.enabled = true;
            }
        }

        // 检查玩家是否学会了 Turtle Crawl
        if (CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedTurtleCrawl)
        {
            if (turtleCrawl != null)
            {
                turtleCrawl.enabled = true;
            }
        }

        // 检查玩家是否学会了 Cat Crawl
        if (CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedCatCrawl)
        {
            if (catCrawl != null)
            {
                catCrawl.enabled = true;
            }
        }

        // 检查玩家是否学会了 Snake Crawl
        if (CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedSnakeCrawl)
        {
            if (snakeCrawl != null)
            {
                snakeCrawl.enabled = true;
            }
        }
    }
}