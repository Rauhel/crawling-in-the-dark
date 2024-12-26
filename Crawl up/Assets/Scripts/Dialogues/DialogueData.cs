using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueData : MonoBehaviour
{
    public static DialogueData Instance { get; private set; }

    // 用于存储所有对话的字典
    private Dictionary<string, Dictionary<int, List<string>>> dialogues = 
        new Dictionary<string, Dictionary<int, List<string>>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadDialogues();
            Debug.Log("DialogueData 初始化成功");
        }
        else
        {
            Debug.Log("已存在 DialogueData 实例，销毁重复对象");
            Destroy(gameObject);
        }
    }

    private void LoadDialogues()
    {
        // 加载CSV文件
        TextAsset csvFile = Resources.Load<TextAsset>("Dialogues/NpcDialogues");
        if (csvFile == null)
        {
            Debug.LogError("找不到对话文件！请确保文件位于 Resources/Dialogues/NpcDialogues.csv");
            return;
        }

        Debug.Log("成功加载对话文件");
        string[] lines = csvFile.text.Split('\n');
        bool isFirstLine = true;

        foreach (string line in lines)
        {
            // 跳过表头
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            // 解析CSV行
            string[] values = line.Trim().Split(',');
            if (values.Length < 4) continue;

            string npcType = values[0];
            int dialogueIndex = int.Parse(values[1]);
            string content = values[3];

            // 确保NPC类型存在
            if (!dialogues.ContainsKey(npcType))
            {
                dialogues[npcType] = new Dictionary<int, List<string>>();
            }

            // 确保对话索引存在
            if (!dialogues[npcType].ContainsKey(dialogueIndex))
            {
                dialogues[npcType][dialogueIndex] = new List<string>();
            }

            // 添加对话内容
            dialogues[npcType][dialogueIndex].Add(content);
        }
    }

    // 获取特定NPC的特定索引的对话
    public string[] GetDialogue(string npcType, int dialogueIndex)
    {
        if (dialogues.ContainsKey(npcType) && 
            dialogues[npcType].ContainsKey(dialogueIndex))
        {
            return dialogues[npcType][dialogueIndex].ToArray();
        }
        return null;
    }
} 