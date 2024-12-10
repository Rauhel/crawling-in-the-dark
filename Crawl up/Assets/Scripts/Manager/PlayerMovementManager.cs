using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementStrategy
{
    void Move(GameObject player);
}

public class PlayerMovementManager : MonoBehaviour
{
    private Dictionary<string, IMovementStrategy> movementStrategies;
    private IMovementStrategy currentMovementStrategy;

    void Start()
    {
        movementStrategies = new Dictionary<string, IMovementStrategy>
        {
            { "CrabWalk", new CrabWalk() },
            { "LeopardRun", new LeopardRun() },
            { "WormCrawl", new WormCrawl() }
        };

        // 默认行动方式
        currentMovementStrategy = movementStrategies["CrabWalk"];
    }

    void Update()
    {
        if (currentMovementStrategy != null)
        {
            currentMovementStrategy.Move(gameObject);
        }
    }

    public void SetMovementStrategy(string strategyName)
    {
        if (movementStrategies.ContainsKey(strategyName))
        {
            currentMovementStrategy = movementStrategies[strategyName];
        }
        else
        {
            Debug.LogWarning("Movement strategy not found: " + strategyName);
        }
    }
}

public class CrabWalk : IMovementStrategy
{
    public void Move(GameObject player)
    {
        // 实现螃蟹爬的逻辑
        Debug.Log("Crab walking");
    }
}

public class LeopardRun : IMovementStrategy
{
    public void Move(GameObject player)
    {
        // 实现豹子爬的逻辑
        Debug.Log("Leopard running");
    }
}

public class WormCrawl : IMovementStrategy
{
    public void Move(GameObject player)
    {
        // 实现虫子爬的逻辑
        Debug.Log("Worm crawling");
    }
}
