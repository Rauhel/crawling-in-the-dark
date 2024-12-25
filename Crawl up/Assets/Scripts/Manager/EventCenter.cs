using System;
using System.Collections.Generic;
using UnityEngine;

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

    // 定义事件类型
    public const string EVENT_LEARNED_BASIC_CRAWL = "learned_basic_crawl";
    public const string EVENT_LEARNED_GECKO_CRAWL = "learned_gecko_crawl";
    public const string EVENT_LEARNED_TURTLE_CRAWL = "learned_turtle_crawl";
    public const string EVENT_LEARNED_SNAKE_CRAWL = "learned_snake_crawl";
    public const string EVENT_LEARNED_CAT_CRAWL = "learned_cat_crawl";
    public const string EVENT_LEARNED_CHAMELEON_CRAWL = "learned_chameleon_crawl";
    public const string EVENT_PLAYER_DIED = "player_died";
    public const string EVENT_NPC_DIED = "npc_died";
    public const string EVENT_REVIVE_ALL_NPCS = "revive_all_npcs";

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