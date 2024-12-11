using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BasicCrawl basicCrawl;
    private GeckoCrawl geckoCrawl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        basicCrawl = GetComponent<BasicCrawl>();
        geckoCrawl = GetComponent<GeckoCrawl>();

        // 检查玩家是否学会了 Basic Crawl
        if (CrawlManager.Instance.hasLearnedBasicCrawl)
        {
            Debug.Log("Player has learned Basic Crawl.");
            if (basicCrawl == null)
            {
                basicCrawl = gameObject.AddComponent<BasicCrawl>();
                Debug.Log("Basic Crawl is added to the player.");
            }
            basicCrawl.enabled = true;
        }
        else
        {
            if (basicCrawl != null)
            {
                basicCrawl.enabled = false;
            }
        }

        // 检查玩家是否学会了 Gecko Crawl
        if (CrawlManager.Instance.hasLearnedGeckoCrawl)
        {
            Debug.Log("Player has learned Gecko Crawl.");
            if (geckoCrawl == null)
            {
                geckoCrawl = gameObject.AddComponent<GeckoCrawl>();
                Debug.Log("Gecko Crawl is added to the player.");
            }
            geckoCrawl.enabled = true;
        }
        else
        {
            if (geckoCrawl != null)
            {
                geckoCrawl.enabled = false;
            }
        }
    }
}