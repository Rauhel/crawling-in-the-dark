using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [Header("UI 设置")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    
    private string[] currentDialogueLines;
    private int currentLineIndex;
    private bool isInDialogue;
    private PlayerInput playerInput;
    private System.Action onDialogueComplete;  // 对话结束时的回调

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始时隐藏对话面板
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
            
        // 获取玩家输入组件引用
        playerInput = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (isInDialogue && Input.GetKeyDown(KeyCode.Q))
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue(string[] lines, System.Action onComplete = null)
    {
        currentDialogueLines = lines;
        currentLineIndex = 0;
        isInDialogue = true;
        onDialogueComplete = onComplete;

        // 暂停游戏
        Time.timeScale = 0f;
        
        // 禁用玩家输入
        if (playerInput != null)
            playerInput.enabled = false;

        // 显示对话面板和第一行对话
        dialoguePanel.SetActive(true);
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        if (currentLineIndex < currentDialogueLines.Length)
        {
            dialogueText.text = currentDialogueLines[currentLineIndex];
        }
    }

    private void DisplayNextLine()
    {
        currentLineIndex++;
        
        if (currentLineIndex >= currentDialogueLines.Length)
        {
            EndDialogue();
        }
        else
        {
            DisplayCurrentLine();
        }
    }

    private void EndDialogue()
    {
        isInDialogue = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        
        // 恢复游戏
        Time.timeScale = 1f;
        
        // 重新启用玩家输入
        if (playerInput != null)
            playerInput.enabled = true;

        // 调用对话结束回调
        onDialogueComplete?.Invoke();
    }

    public bool IsDialogueActive()
    {
        return isInDialogue;
    }
} 