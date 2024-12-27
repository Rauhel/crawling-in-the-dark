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
public class CrawlSettings
{
    public KeyList[] keyLists;
    public float groundSpeed;
    public float waterSpeed;
    public float iceSpeed;
    public float wallSpeed;
    public bool isActive;
    public bool canCrawl;
    public AnimationFrame[] animationFrames;  // 动画帧数组
    public int TotalAnimFrames => animationFrames != null ? animationFrames.Length : 0;  // 总帧数属性
}

[System.Serializable]
public class CrawlProgress
{
    public int currentKeyListIndex = 0;
    public float lastValidInputTime = 0f;
    public bool isInProgress = false;
    public bool isPlayingTransitionAnim = false;
} 