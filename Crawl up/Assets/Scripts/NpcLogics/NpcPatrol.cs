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

    // Start is called before the first frame update
    void Start()
    {
        targetPoint = pointB.position; // 初始目标点为B
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsFight)
        {
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
