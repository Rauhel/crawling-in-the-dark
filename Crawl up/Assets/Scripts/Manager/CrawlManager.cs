using System;
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
    public bool hasLearnedCatCrawl = false;
    public bool hasLearnedSnakeCrawl = false;
    public bool hasLearnedChameleonCrawl = false;
    public bool hasLearnedTurtleCrawl = false;

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
        hasLearnedCatCrawl = false;
        hasLearnedSnakeCrawl = false;
        hasLearnedChameleonCrawl = false;
        hasLearnedTurtleCrawl = false;

        // 调试信息
        Debug.Log("CrawlManager initialized.");
        Debug.Log("hasLearnedBasicCrawl: " + hasLearnedBasicCrawl);
        Debug.Log("hasLearnedGeckoCrawl: " + hasLearnedGeckoCrawl);
        Debug.Log("hasLearnedCatCrawl: " + hasLearnedCatCrawl);
        Debug.Log("hasLearnedSnakeCrawl: " + hasLearnedSnakeCrawl);
        Debug.Log("hasLearnedChameleonCrawl: " + hasLearnedChameleonCrawl);
        Debug.Log("hasLearnedTurtleCrawl: " + hasLearnedTurtleCrawl);
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
            Debug.Log("Player has learned Gecko Crawl.");
        }
        else if (crawlType == "CatCrawl")
        {
            hasLearnedCatCrawl = true;
            Debug.Log("Player has learned Cat Crawl.");
        }
        else if (crawlType == "SnakeCrawl")
        {
            hasLearnedSnakeCrawl = true;
            Debug.Log("Player has learned Snake Crawl.");
        }
        else if (crawlType == "ChameleonCrawl")
        {
            hasLearnedChameleonCrawl = true;
            Debug.Log("Player has learned Chameleon Crawl.");
        }
        else if (crawlType == "TurtleCrawl")
        {
            hasLearnedTurtleCrawl = true;
            Debug.Log("Player has learned Turtle Crawl.");
        }

        OnLearnCrawl?.Invoke(crawlType);
    }
}