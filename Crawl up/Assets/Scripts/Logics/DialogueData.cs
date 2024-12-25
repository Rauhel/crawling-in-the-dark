using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 10)]
    public string content;
    public float displayTime = 2f;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Game/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] dialogueLines;
    public CrawlType triggerCrawlType;
} 