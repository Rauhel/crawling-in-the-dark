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
    public float groundSpeed;
    public float waterSpeed;
    public float iceSpeed;
    public float treeSpeed;  // 竖直方向上的速度，用于Tree标签的物体
    public float slopeSpeed; // 斜坡上的速度
    public bool isActive;
    public bool canCrawl;
    public AnimationFrame[] animationFrames;  // 爬行动画帧数组
    public TransitionAnimationSettings transitionAnimation;  // 过渡动画设置
    public int TotalAnimFrames => animationFrames != null ? animationFrames.Length : 0;  // 总帧数属性
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