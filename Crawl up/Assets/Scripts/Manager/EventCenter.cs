using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 事件中心类
public class EventCenter : MonoBehaviour
{
    // 单例模式
    private static EventCenter instance;
    public static EventCenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EventCenter>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(EventCenter).ToString());
                    instance = singleton.AddComponent<EventCenter>();
                }
            }
            return instance;
        }
    }

    // 在Awake中实例化单例
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private static Dictionary<string, Action> eventTable = new Dictionary<string, Action>();

    // 订阅事件
    public void Subscribe(string eventType, Action listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] += listener;
        }
        else
        {
            eventTable[eventType] = listener;
        }
    }

    // 取消订阅事件
    public void Unsubscribe(string eventType, Action listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] -= listener;
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

    // 触发事件
    public void Publish(string eventType)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType]?.Invoke();
        }
    }
}