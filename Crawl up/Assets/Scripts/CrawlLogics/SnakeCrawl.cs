using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeCrawl : MonoBehaviour
{
    private List<KeyCode> rightKeys = new List<KeyCode>
    {
        KeyCode.M, KeyCode.N, KeyCode.B, KeyCode.V, KeyCode.C, KeyCode.X, KeyCode.Z
    };

    private List<KeyCode> leftKeys = new List<KeyCode>
    {
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M
    };

    private int currentIndex = 0;
    private bool isMovingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (isMovingRight && Input.GetKeyDown(rightKeys[currentIndex]))
            {
                MoveRight();
                currentIndex++;
                if (currentIndex >= rightKeys.Count)
                {
                    currentIndex = 0;
                }
            }
            else if (!isMovingRight && Input.GetKeyDown(leftKeys[currentIndex]))
            {
                MoveLeft();
                currentIndex++;
                if (currentIndex >= leftKeys.Count)
                {
                    currentIndex = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Z))
            {
                // 切换方向
                isMovingRight = !isMovingRight;
                currentIndex = 0;
            }
            else
            {
                return; // 按错键，直接返回
            }
        }
    }

    void MoveRight()
    {
        // 实现角色向右移动的逻辑
        transform.Translate(Vector3.right * 0.1f);
        // 播放对应的动画
        // animator.Play("MoveRightAnimation");
    }

    void MoveLeft()
    {
        // 实现角色向左移动的逻辑
        transform.Translate(Vector3.left * 0.1f);
        // 播放对应的动画
        // animator.Play("MoveLeftAnimation");
    }
}