using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCrawl : MonoBehaviour
{
    public float speed = 5.0f;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 获取水平输入 (A 和 D 键)
        float moveHorizontal = Input.GetAxis("Horizontal");

        // 计算移动向量，只在水平轴上移动
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, 0.0f);

        // 移动角色
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        // 更新动画参数
        if (animator != null)
        {
            animator.SetBool("isMovingLeft", moveHorizontal < 0);
            animator.SetBool("isMovingRight", moveHorizontal > 0);
        }
    }
}
