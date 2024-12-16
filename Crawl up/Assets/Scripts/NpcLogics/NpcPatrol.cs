using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcPatrol : MonoBehaviour
{
    public Transform pointA; // 巡逻点A
    public Transform pointB; // 巡逻点B
    public float speed = 2.0f; // 移动速度
    public bool IsFight = false; // 是否进入战斗状态

    private Vector3 targetPoint; // 当前目标点
    private Animator animator; // 动画控制器

    // Start is called before the first frame update
    void Start()
    {
        targetPoint = pointB.position; // 初始目标点为B
        animator = GetComponent<Animator>(); // 获取动画控制器组件
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsFight)
        {
            // 播放巡逻动画
            animator.SetBool("IsPatrolling", true);

            // 移动到目标点
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

            // 检查是否到达目标点
            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                // 切换目标点
                if (targetPoint == pointB.position)
                {
                    targetPoint = pointA.position;
                }
                else
                {
                    targetPoint = pointB.position;
                }
            }
        }
        else
        {
            // 停止巡逻动画
            animator.SetBool("IsPatrolling", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null && !playerController.chameleon)
            {
                IsFight = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsFight = false;
        }
    }
}
