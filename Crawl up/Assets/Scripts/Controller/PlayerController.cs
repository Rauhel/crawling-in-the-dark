using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum CrawlType
    {
        BasicCrawl,
        GeckoCrawl,
        TurtleCrawl,
        SnakeCrawl,
        CatCrawl,
        ChameleonCrawl
    }

    [System.Serializable]
    public class CrawlSettings
    {
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
        }
    }
}