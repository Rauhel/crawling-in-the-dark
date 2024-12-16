using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using MyGame.Utilities;

public class State : MonoBehaviour
{
    //抽象基类，定义了所有状态类的基本结构
    public interface IState
    {
        void OnEnter();// 进入状态时的方法
        void OnUpdate();// 更新方法
        void OnFixdUpdate();// 固定更新方法
        void OnExit();// 退出状态时的方法
    }
    
    // 定义状态类型枚举
    public enum StateType
    {
        Idle, //待机
    }

    [SerializeField]
    public class Parameter
    {
        [Header("属性")]
        [HideInInspector] public Animator animator;
        
        [HideInInspector] public SpriteRenderer sr;
        [HideInInspector] public Rigidbody2D rb;
        // [HideInInspector] public PhysicsCheck check;

        [Header("受伤与死亡")]
        [HideInInspector] public bool isHurt; // 是否受伤
        [HideInInspector] public bool isDead; // 是否死亡
    }

}