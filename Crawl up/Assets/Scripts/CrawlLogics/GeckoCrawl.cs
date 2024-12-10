using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckoCrawl : MonoBehaviour
{
    public float speed = 20.0f;
    private Animator animator;
    private List<KeyCode> inputSequence = new List<KeyCode>();
    private List<KeyCode> upSequence1 = new List<KeyCode> { KeyCode.W, KeyCode.R, KeyCode.S, KeyCode.F };
    private List<KeyCode> upSequence2 = new List<KeyCode> { KeyCode.R, KeyCode.W, KeyCode.F, KeyCode.S };
    private List<KeyCode> downSequence1 = new List<KeyCode> { KeyCode.S, KeyCode.F, KeyCode.W, KeyCode.R };
    private List<KeyCode> downSequence2 = new List<KeyCode> { KeyCode.F, KeyCode.S, KeyCode.R, KeyCode.W };

    private float inputTimer = 0.0f;
    private float inputTimeout = 1.0f; // 1秒超时

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 检测按键输入
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.W)) inputSequence.Add(KeyCode.W);
            if (Input.GetKeyDown(KeyCode.R)) inputSequence.Add(KeyCode.R);
            if (Input.GetKeyDown(KeyCode.S)) inputSequence.Add(KeyCode.S);
            if (Input.GetKeyDown(KeyCode.F)) inputSequence.Add(KeyCode.F);

            // 重置输入计时器
            inputTimer = 0.0f;

            // 保持输入序列的长度不超过4
            if (inputSequence.Count > 4)
            {
                inputSequence.RemoveAt(0);
            }

            // 检查输入序列是否匹配向上爬行的序列
            if (MatchesSequence(inputSequence, upSequence1) || MatchesSequence(inputSequence, upSequence2))
            {
                Move(Vector3.up);
                PlayAnimation("MoveUp");
                inputSequence.Clear(); // 移动后清空序列
            }
            // 检查输入序列是否匹配向下爬行的序列
            else if (MatchesSequence(inputSequence, downSequence1) || MatchesSequence(inputSequence, downSequence2))
            {
                Move(Vector3.down);
                PlayAnimation("MoveDown");
                inputSequence.Clear(); // 移动后清空序列
            }
        }
        else
        {
            // 更新输入计时器
            inputTimer += Time.deltaTime;

            // 如果超过超时时间，清空输入序列
            if (inputTimer >= inputTimeout)
            {
                inputSequence.Clear();
                inputTimer = 0.0f;
            }
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

    private void Move(Vector3 direction)
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void PlayAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }
}
