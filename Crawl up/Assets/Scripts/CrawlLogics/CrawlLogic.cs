using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlController : MonoBehaviour
{
    public float speed = 5.0f; // 移动速度
    public bool isMovingRight = true; // 是否向右移动

    // 按键序列和爬行动物类型
    public enum CrawlType
    {
        Cat,
        Chameleon,
        Gecko,
        Snake,
        Turtle,
        Basic
    }

    public CrawlType crawlType;

    private PlayerController playerController;
    private HashSet<KeyCode> leftKeys;
    private HashSet<KeyCode> rightKeys;
    private int leftKeyCount;
    private int rightKeyCount;
    private bool hasPressedLeft;
    private bool hasPressedRight;
    private float timer;
    private float interval;
    private float gracePeriod;
    private string currentSequence;
    private int currentIndex;
    private bool inputCorrect;
    private List<KeyCode> inputSequence;
    private List<KeyCode> upSequence1;
    private List<KeyCode> upSequence2;
    private List<KeyCode> downSequence1;
    private List<KeyCode> downSequence2;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController script not found on the same GameObject.");
        }

        InitializeKeys();
        InitializeSequences();
    }

    void InitializeKeys()
    {
        switch (crawlType)
        {
            case CrawlType.Cat:
                leftKeys = new HashSet<KeyCode> { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B };
                rightKeys = new HashSet<KeyCode> { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.N, KeyCode.M };
                break;
            case CrawlType.Chameleon:
                break;
            case CrawlType.Gecko:
                break;
            case CrawlType.Snake:
                leftKeys = new HashSet<KeyCode> { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B };
                rightKeys = new HashSet<KeyCode> { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.N, KeyCode.M };
                break;
            case CrawlType.Turtle:
                leftKeys = new HashSet<KeyCode> { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B };
                rightKeys = new HashSet<KeyCode> { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.N, KeyCode.M };
                break;
            case CrawlType.Basic:
                leftKeys = new HashSet<KeyCode> { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B };
                rightKeys = new HashSet<KeyCode> { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.N, KeyCode.M };
                break;
        }
    }

    void InitializeSequences()
    {
        switch (crawlType)
        {
            case CrawlType.Gecko:
                upSequence1 = new List<KeyCode> { KeyCode.W, KeyCode.R, KeyCode.S, KeyCode.F };
                upSequence2 = new List<KeyCode> { KeyCode.R, KeyCode.W, KeyCode.F, KeyCode.S };
                downSequence1 = new List<KeyCode> { KeyCode.S, KeyCode.F, KeyCode.W, KeyCode.R };
                downSequence2 = new List<KeyCode> { KeyCode.F, KeyCode.S, KeyCode.R, KeyCode.W };
                break;
                // Initialize other types' sequences here
        }
    }

    void Update()
    {
        switch (crawlType)
        {
            case CrawlType.Cat:
                CatCrawlUpdate();
                break;
            case CrawlType.Chameleon:
                ChameleonCrawlUpdate();
                break;
            case CrawlType.Gecko:
                GeckoCrawlUpdate();
                break;
            case CrawlType.Snake:
                SnakeCrawlUpdate();
                break;
            case CrawlType.Turtle:
                TurtleCrawlUpdate();
                break;
            case CrawlType.Basic:
                BasicCrawlUpdate();
                break;
        }
    }

    void CatCrawlUpdate()
    {
        // CatCrawl.cs logic here
    }

    void ChameleonCrawlUpdate()
    {
        // ChameleonCrawl.cs logic here
    }

    void GeckoCrawlUpdate()
    {
        // GeckoCrawl.cs logic here
    }

    void SnakeCrawlUpdate()
    {
        // SnakeCrawl.cs logic here
    }

    void TurtleCrawlUpdate()
    {
        // TurtleCrawl.cs logic here
    }

    void BasicCrawlUpdate()
    {
        // BasicCrawl.cs logic here
    }

    // Add other methods and logic from the original scripts here, making sure to include them in the correct Update method based on crawlType
}
