using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [Header("UI 设置")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject interactionPrompt;  // 交互提示UI
    
    private string[] currentDialogueLines;
    [SerializeField] private int currentLineIndex = 0;
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

        // 初始时隐藏所有UI
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
            
        // 获取玩家输入组件引用
        playerInput = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (isInDialogue && Input.GetKeyDown(KeyCode.Q))
        {
            DisplayNextLine();
            Debug.Log("DisplayNextLine");
        }
    }

    public void StartDialogue(string[] lines, System.Action onComplete = null)
    {
        Debug.Log("StartDialogue");
        
        // 检查对话面板引用
        if (dialoguePanel == null)
        {
            Debug.LogError("对话面板引用为空！");
            return;
        }
        
        // 检查对话文本引用
        if (dialogueText == null)
        {
            Debug.LogError("对话文本引用为空！");
            return;
        }

        // 检查对话内容
        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("对话内容为空！");
            return;
        }

        currentDialogueLines = lines;
        currentLineIndex = -1;
        isInDialogue = true;
        onDialogueComplete = onComplete;

        // 暂停游戏
        Time.timeScale = 0f;
        
        // 禁用玩家输入
        if (playerInput != null)
            playerInput.enabled = false;

        Debug.Log($"对话面板当前状态: {dialoguePanel.activeSelf}");
        dialoguePanel.SetActive(true);
        Debug.Log($"对话面板设置后状态: {dialoguePanel.activeSelf}");
        
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        if (currentLineIndex < currentDialogueLines.Length)
        {
            Debug.Log($"显示对话: {currentDialogueLines[currentLineIndex]}");
            dialogueText.text = currentDialogueLines[currentLineIndex];
        }
    }

    private void DisplayNextLine()
    {
        currentLineIndex++;
        Debug.Log($"currentLineIndex: {currentLineIndex}");
        
        if (currentLineIndex >= currentDialogueLines.Length)
        {
            EndDialogue();
            Debug.Log("EndDialogue");
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

    // 显示交互提示
    public void ShowInteractionPrompt()
    {
        if (interactionPrompt != null && !isInDialogue)
        {
            interactionPrompt.SetActive(true);
            Debug.Log("显示交互提示: 按Q交互");
        }
    }

    // 隐藏交互提示
    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
            Debug.Log("隐藏交互提示");
        }
    }
} 