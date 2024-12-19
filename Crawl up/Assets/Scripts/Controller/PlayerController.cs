using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
<<<<<<< HEAD
    public enum CrawlType
    {
        BasicCrawl,
        GeckoCrawl,
        TurtleCrawl,
        SnakeCrawl,
        CatCrawl,
        ChameleonCrawl
=======
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
>>>>>>> parent of 3f16b7c (Player Controller)
    }

    [System.Serializable]
    public class CrawlSettings
    {
<<<<<<< HEAD
        public CrawlType crawlType;
        public List<KeyCode> keySequence;
        public float groundSpeed;
        public float waterSpeed;
        public float iceSpeed;
        public float wallSpeed;
    }

    public List<CrawlSettings> crawlSettingsList;

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Handle left direction
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // Handle right direction
            transform.localScale = new Vector3(1, 1, 1);
=======
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
>>>>>>> parent of 3f16b7c (Player Controller)
        }
    }
}