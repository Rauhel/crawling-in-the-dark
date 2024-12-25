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
public class CrawlSettings
{
    public KeyList[] keyLists;
    public float groundSpeed;
    public float waterSpeed;
    public float iceSpeed;
    public float wallSpeed;
    public bool isActive;
    public bool canCrawl;
}

[System.Serializable]
public class CrawlProgress
{
    public int currentKeyListIndex = 0;
    public float lastValidInputTime = 0f;
    public bool isInProgress = false;
    public bool isPlayingTransitionAnim = false;
} 