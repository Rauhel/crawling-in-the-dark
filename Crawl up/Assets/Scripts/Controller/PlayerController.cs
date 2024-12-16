using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool chameleon;
    private BasicCrawl basicCrawl;
    private GeckoCrawl geckoCrawl;

    // Start is called before the first frame update
    void Start()
    {
        basicCrawl = GetComponent<BasicCrawl>();
        geckoCrawl = GetComponent<GeckoCrawl>();

        // 检查是否成功获取组件
        if (basicCrawl == null)
        {
            Debug.LogError("BasicCrawl component not found on the player.");
        }
        if (geckoCrawl == null)
        {
            Debug.LogError("GeckoCrawl component not found on the player.");
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
        Debug.Log("Player has learned " + crawlType + ".");
        CheckCrawlAbilities();
    }

    // 方法：检查玩家的爬行能力
    private void CheckCrawlAbilities()
    {
        Debug.Log("Checking crawl abilities...");
        // 检查玩家是否学会了 Basic Crawl
        if (CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedBasicCrawl)
        {
            Debug.Log("111");   
            if (basicCrawl != null)
            {
                Debug.Log("222");
                basicCrawl.enabled = true;
                Debug.Log("Basic Crawl is enabled.");
            }
        }
        else
        {
            if (basicCrawl != null)
            {
                Debug.Log("333");
                basicCrawl.enabled = false;
            }
        }

        // 检查玩家是否学会了 Gecko Crawl
        if (CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedGeckoCrawl)
        {
            if (geckoCrawl != null)
            {
                geckoCrawl.enabled = true;
                Debug.Log("Gecko Crawl is enabled.");
            }
        }
        else
        {
            if (geckoCrawl != null)
            {
                geckoCrawl.enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (CrawlManager.Instance != null)
        {
            CrawlManager.Instance.OnLearnCrawl -= OnLearnCrawl;
        }
    }

    // 方法：处理与标签为Water的GameObject接触
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            if (basicCrawl != null)
            {
                basicCrawl.enabled = false;
            }
            if (geckoCrawl != null)
            {
                geckoCrawl.enabled = false;
            }
        }
    }
}