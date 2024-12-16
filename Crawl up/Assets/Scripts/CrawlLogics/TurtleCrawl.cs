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
        // 检测方向输入 (A 和 D 键)
        if (Input.GetKeyDown(KeyCode.A) && !Input.anyKeyDown)
        {
            moveDirection = Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && !Input.anyKeyDown)
        {
            moveDirection = Vector3.right;
        }

        // 检测按键输入
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.F)) inputSequence.Add(KeyCode.F);
            if (Input.GetKeyDown(KeyCode.J)) inputSequence.Add(KeyCode.J);
            if (Input.GetKeyDown(KeyCode.D)) inputSequence.Add(KeyCode.D);
            if (Input.GetKeyDown(KeyCode.K)) inputSequence.Add(KeyCode.K);
            if (Input.GetKeyDown(KeyCode.S)) inputSequence.Add(KeyCode.S);
            if (Input.GetKeyDown(KeyCode.L)) inputSequence.Add(KeyCode.L);

            // 重置输入计时器
            inputTimer = 0.0f;

            // 保持输入序列的长度不超过2
            if (inputSequence.Count > 2)
            {
                inputSequence.RemoveAt(0);
            }

            // 检查输入序列是否匹配当前序列
            if (MatchesSequence(inputSequence, GetCurrentSequence()))
            {
                Move();
                inputSequence.Clear(); // 移动后清空序列
                currentSequenceIndex = (currentSequenceIndex + 1) % 3; // 切换到下一个序列
            }
        }
        else
        {
            // 更新输入计时器
            inputTimer += Time.deltaTime;

            // 如果超过超时时间，清空输入序列并重置序列索引
            if (inputTimer >= inputTimeout)
            {
                inputSequence.Clear();
                inputTimer = 0.0f;
                currentSequenceIndex = 0; // 重置序列索引
            }
        }
    }

    private List<KeyCode> GetCurrentSequence()
    {
        switch (currentSequenceIndex)
        {
            case 0:
                return sequence1;
            case 1:
                return sequence2;
            case 2:
                return sequence3;
            default:
                return sequence1;
        }
    }

    private bool MatchesSequence(List<KeyCode> input, List<KeyCode> sequence)
    {
        if (input.Count != sequence.Count) return false;
        for (int i = 0; i < input.Count; i++)
        {
            if (input[i] != sequence[i]) return false;
        }
        return true;
    }

    private void Move()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }
}
