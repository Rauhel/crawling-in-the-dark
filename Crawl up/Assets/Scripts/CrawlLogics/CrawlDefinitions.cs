using UnityEngine;
using System.Collections.Generic;

public enum CrawlType
{
    Basic,
    Gecko,
    Turtle,
    Snake,
    Cat,
    Chameleon
}

[System.Serializable]
public class KeyList
{
    public KeyCode[] keySequence;
    public int requiredMinKeyCount = 1;
    public int requiredMaxKeyCount = 1;
}

[System.Serializable]
public class AnimationFrame
{
    public Sprite sprite;  // 动画帧使用的精灵
}

[System.Serializable]
public class TransitionAnimationSettings
{
    public Sprite[] transitionFrames = new Sprite[2];  // 过渡动画的两帧
}

[System.Serializable]
public class CrawlSettings
{
    public KeyList[] keyLists;

    [Header("速度设置")]
    [Tooltip("在地面上的移动速度")]
    public float groundSpeed = 1f;
    [Tooltip("在水中的移动速度")]
    public float waterSpeed = 0.7f;
    [Tooltip("在冰面上的移动速度")]
    public float iceSpeed = 1.5f;
    [Tooltip("在树上的移动速度")]
    public float treeSpeed = 0.8f;
    [Tooltip("在斜坡上的移动速度")]
    public float slopeSpeed = 0.9f;

    [Header("状态设置")]
    [Tooltip("是否是当前激活的爬行方式")]
    public bool isActive;
    [Tooltip("是否已解锁此爬行方式")]
    public bool canCrawl;

    [Header("动画设置")]
    [Tooltip("爬行动画的关键帧")]
    public AnimationFrame[] animationFrames;
    [Tooltip("切换爬行方式时的过渡动画")]
    public TransitionAnimationSettings transitionAnimation;

    public int TotalAnimFrames => animationFrames != null ? animationFrames.Length : 0;
}

[System.Serializable]
public class CrawlProgress
{
    public int currentKeyListIndex = 0;
    public float lastValidInputTime = 0f;
    public bool isInProgress = false;
    public bool isPlayingTransitionAnim = false;
    public int currentTransitionFrame = 0;  // 当前过渡动画帧索引
}

public static class CrawlSettingsInitializer
{
    public static void InitializeSettings(
        ref CrawlSettings basicCrawl,
        ref CrawlSettings geckoCrawl,
        ref CrawlSettings turtleCrawl,
        ref CrawlSettings snakeCrawl,
        ref CrawlSettings catCrawl,
        ref CrawlSettings chameleonCrawl)
    {
        // 初始化基础爬行
        if (basicCrawl == null)
        {
            basicCrawl = new CrawlSettings();
        }
        basicCrawl.keyLists = new KeyList[]
        {
            new KeyList { keySequence = new KeyCode[] { KeyCode.A}, requiredMinKeyCount = 1, requiredMaxKeyCount = 1 },
            new KeyList { keySequence = new KeyCode[] { KeyCode.D}, requiredMinKeyCount = 1, requiredMaxKeyCount = 1 }
        };
        basicCrawl.isActive = true;
        basicCrawl.canCrawl = true;

        // 初始化壁虎爬行
        if (geckoCrawl == null)
        {
            geckoCrawl = new CrawlSettings();
        }
        geckoCrawl.keyLists = new KeyList[]
        {
            new KeyList { keySequence = new KeyCode[] { KeyCode.W } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.R } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.S } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.F } }
        };
        geckoCrawl.isActive = false;
        geckoCrawl.canCrawl = false;

        // 初始化变色龙爬行
        if (chameleonCrawl == null)
        {
            chameleonCrawl = new CrawlSettings();
        }
        chameleonCrawl.keyLists = new KeyList[]
        {
            new KeyList { keySequence = new KeyCode[] { KeyCode.W } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.R } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.S } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.F } }
        };
        chameleonCrawl.isActive = false;
        chameleonCrawl.canCrawl = false;

        // 初始化乌龟爬行
        if (turtleCrawl == null)
        {
            turtleCrawl = new CrawlSettings();
        }
        turtleCrawl.keyLists = new KeyList[]
        {
            new KeyList { keySequence = new KeyCode[] { KeyCode.F, KeyCode.J }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 },
            new KeyList { keySequence = new KeyCode[] { KeyCode.D, KeyCode.K }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 },
            new KeyList { keySequence = new KeyCode[] { KeyCode.S, KeyCode.L }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 }
        };
        turtleCrawl.isActive = false;
        turtleCrawl.canCrawl = false;

        // 初始化蛇爬行
        if (snakeCrawl == null)
        {
            snakeCrawl = new CrawlSettings();
        }
        snakeCrawl.keyLists = new KeyList[]
        {
            new KeyList { keySequence = new KeyCode[] { KeyCode.M } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.N } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.B } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.V } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.C } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.X } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.Z } }
        };
        snakeCrawl.isActive = false;
        snakeCrawl.canCrawl = false;

        // 初始化猫爬行
        if (catCrawl == null)
        {
            catCrawl = new CrawlSettings();
        }
        catCrawl.keyLists = new KeyList[]
        {
            new KeyList { 
                keySequence = new KeyCode[] { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G }, 
                requiredMinKeyCount = 3, 
                requiredMaxKeyCount = 5 
            },
            new KeyList { 
                keySequence = new KeyCode[] { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Y }, 
                requiredMinKeyCount = 3, 
                requiredMaxKeyCount = 5 
            }
        };
        catCrawl.isActive = false;
        catCrawl.canCrawl = false;
    }
} 