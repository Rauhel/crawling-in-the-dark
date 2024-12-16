using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlManager : MonoBehaviour
{
    private static CrawlManager instance;

    public static CrawlManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CrawlManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(CrawlManager).ToString());
                    instance = singleton.AddComponent<CrawlManager>();
                }
            }
            return instance;
        }
    }

    // Event for learning a new crawl type
    public event Action<string> OnLearnCrawl;

    // 玩家学会的爬行方式
    public bool hasLearnedBasicCrawl = false;
    public bool hasLearnedGeckoCrawl = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 初始化爬行方式
        hasLearnedBasicCrawl = false;
        hasLearnedGeckoCrawl = false;

        // 调试信息
        Debug.Log("CrawlManager initialized.");
        Debug.Log("hasLearnedBasicCrawl: " + hasLearnedBasicCrawl);
        Debug.Log("hasLearnedGeckoCrawl: " + hasLearnedGeckoCrawl);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 方法：学习一种爬行方式
    public void LearnCrawl(string crawlType)
    {
        if (crawlType == "BasicCrawl")
        {
            hasLearnedBasicCrawl = true;
            Debug.Log("Player has learned Basic Crawl.");
        }
        else if (crawlType == "GeckoCrawl")
        {
            hasLearnedGeckoCrawl = true;
        }

        // Trigger the event
        Debug.Log("CrawlManager: Triggering OnLearnCrawl event for " + crawlType);
        OnLearnCrawl?.Invoke(crawlType);
    }

    private void OnDestroy()
    {
        // 取消订阅所有事件
        OnLearnCrawl = null;
    }
}