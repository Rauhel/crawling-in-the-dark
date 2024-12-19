using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckoCrawl : MonoBehaviour
{
    public float speed = 20.0f;
    public bool resetOnWrongKey = true; // 按错键是否重置序列
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
                MoveAlongCollider(Vector3.up);
            }
        }

        // 更新输入计时器
        inputTimer += Time.deltaTime;
        if (inputTimer > inputTimeout)
        {
            inputSequence.Clear();
        }

        if (resetOnWrongKey && !MatchesSequence(inputSequence, upSequence1) && !MatchesSequence(inputSequence, upSequence2))
        {
            inputSequence.Clear();
        }
    }

    private bool MatchesSequence(List<KeyCode> inputSequence, List<KeyCode> targetSequence)
    {
        if (inputSequence.Count != targetSequence.Count) return false;

        for (int i = 0; i < inputSequence.Count; i++)
        {
            if (inputSequence[i] != targetSequence[i]) return false;
        }

        return true;
    }

    private void MoveAlongCollider(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Vector3 moveDirection = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }
}