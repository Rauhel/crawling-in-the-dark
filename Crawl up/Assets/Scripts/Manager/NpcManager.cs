using UnityEngine;
using System.Collections.Generic;

public class NpcManager : MonoBehaviour
{
    // 单例模式
    public static NpcManager Instance { get; private set; }
    private List<NpcPatrol> allNpcs = new List<NpcPatrol>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // 订阅玩家死亡事件
        EventCenter.Instance.Subscribe(EventCenter.EVENT_PLAYER_DIED, OnPlayerDied);
    }

    private void OnDisable()
    {
        // 取消订阅事件
        EventCenter.Instance.Unsubscribe(EventCenter.EVENT_PLAYER_DIED, OnPlayerDied);
    }

    // 注册NPC
    public void RegisterNpc(NpcPatrol npc)
    {
        if (!allNpcs.Contains(npc))
        {
            allNpcs.Add(npc);
        }
    }

    // 取消注册NPC
    public void UnregisterNpc(NpcPatrol npc)
    {
        if (allNpcs.Contains(npc))
        {
            allNpcs.Remove(npc);
        }
    }

    // 当���家死亡时复活所有NPC
    private void OnPlayerDied()
    {
        ReviveAllNpcs();
    }

    // 复活所有NPC
    public void ReviveAllNpcs()
    {
        foreach (var npc in allNpcs)
        {
            if (npc != null)
            {
                npc.Revive();
            }
        }
        Debug.Log("所有NPC已复活！");
    }
} 