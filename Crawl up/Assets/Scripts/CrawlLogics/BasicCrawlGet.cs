using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCrawlGet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LearnBasicCrawl", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 方法：学习 Basic Crawl
    private void LearnBasicCrawl()
    {
        CrawlManager.Instance.LearnCrawl("BasicCrawl");
    }
}
