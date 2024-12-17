using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool chameleon;
    public bool isMovingRight; // 添加 isMovingRight 变量
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

        // 订阅事件
        if (CrawlManager.Instance != null)
        {
            CrawlManager.Instance.OnLearnCrawl += OnLearnCrawl;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 检测按键输入
        if (Input.GetKey(KeyCode.A))
        {
            isMovingRight = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            isMovingRight = true;
        }

        if (Input.anyKey)
        {
            int keyCount = 0;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.F))
            {
                keyCount++;
            }
            if (Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.J))
            {
                keyCount++;
            }
            if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Z))
            {
                keyCount++;
            }

            if (keyCount >= 5 && CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedCatCrawl)
            {
                EnableCrawl(catCrawl);
            }
            else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.F)) && CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedGeckoCrawl)
            {
                EnableCrawl(geckoCrawl);
            }
            else if (Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.J) && CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedTurtleCrawl)
            {
                EnableCrawl(turtleCrawl);
            }
            else if ((Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Z)) && CrawlManager.Instance != null && CrawlManager.Instance.hasLearnedSnakeCrawl)
            {
                EnableCrawl(snakeCrawl);
            }
        }
    }

    private void EnableCrawl(MonoBehaviour crawlType)
    {
        basicCrawl.enabled = false;
        geckoCrawl.enabled = false;
        chameleonCrawl.enabled = false;
        turtleCrawl.enabled = false;
        catCrawl.enabled = false;
        snakeCrawl.enabled = false;

        crawlType.enabled = true;
    }

    private void OnLearnCrawl(string crawlType)
    {
        // 处理学习爬行方式的逻辑
        Debug.Log("Learned crawl type: " + crawlType);
    }
    
}