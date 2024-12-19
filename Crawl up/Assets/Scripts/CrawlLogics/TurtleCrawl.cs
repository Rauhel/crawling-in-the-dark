using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleCrawl : MonoBehaviour
{
    public float speed = 5.0f;
    private Vector3 moveDirection = Vector3.right; // 初始方向为右
    private List<KeyCode> inputSequence = new List<KeyCode>();
    private List<KeyCode> sequence1 = new List<KeyCode> { KeyCode.F, KeyCode.J };
    private List<KeyCode> sequence2 = new List<KeyCode> { KeyCode.D, KeyCode.K };
    private List<KeyCode> sequence3 = new List<KeyCode> { KeyCode.S, KeyCode.L };

    private float inputTimer = 0.0f;
    private float inputTimeout = 1.0f; // 1秒超时
    private int currentSequenceIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // 检测按键输入
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.J) ||
                Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.K) ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.L))
            {
                inputSequence.Add(Input.inputString[0]);
                Debug.Log("Input sequence: " + string.Join(", ", inputSequence));
                inputTimer = 0.0f;
            }
        }

        // 检查输入超时
        inputTimer += Time.deltaTime;
        if (inputTimer > inputTimeout)
        {
            inputSequence.Clear();
            Debug.Log("Input sequence cleared due to timeout");
        }

        // 检查输入序列是否匹配
        if (inputSequence.Count == sequence1.Count)
        {
            if (IsSequenceMatch(inputSequence, sequence1))
            {
                moveDirection = Vector3.forward;
                Debug.Log("Sequence 1 matched: Moving forward");
            }
            else if (IsSequenceMatch(inputSequence, sequence2))
            {
                moveDirection = Vector3.back;
                Debug.Log("Sequence 2 matched: Moving backward");
            }
            else if (IsSequenceMatch(inputSequence, sequence3))
            {
                moveDirection = Vector3.up;
                Debug.Log("Sequence 3 matched: Moving up");
            }
            inputSequence.Clear();
        }

        // 移动乌龟
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private bool IsSequenceMatch(List<KeyCode> inputSequence, List<KeyCode> sequence)
    {
        for (int i = 0; i < sequence.Count; i++)
        {
            if (inputSequence[i] != sequence[i])
            {
                return false;
            }
        }
        return true;
    }
}