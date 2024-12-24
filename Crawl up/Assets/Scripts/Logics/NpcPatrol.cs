using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcPatrol : MonoBehaviour
{
    [System.Serializable]
    public class DetectionSettings
    {
        public string[] detectableCrawlTypes;  // 可以检测到的爬行类型
        public float minDetectableSpeed = 0f;  // 最小可检测速度
        public float maxDetectableSpeed = 10f; // 最大可检测速度
    }

    public DetectionSettings detectionSettings;
    private bool IsFight = false;

    void CheckPlayerDetection(PlayerInput playerInput)
    {
        if (playerInput == null) return;

        // 获取玩家当前速度
        float currentSpeed = GetPlayerCurrentSpeed(playerInput);
        
        // 检查速度是否在可检测范围内
        bool speedInRange = (currentSpeed >= detectionSettings.minDetectableSpeed && 
                           currentSpeed <= detectionSettings.maxDetectableSpeed);

        // 检查爬行类型
        bool crawlTypeDetectable = System.Array.Exists(
            detectionSettings.detectableCrawlTypes, 
            type => type.Equals(playerInput.currentCrawlName, System.StringComparison.OrdinalIgnoreCase)
        );

        // 如果速度在范围内且爬行类型可被检测，则发现玩家
        if (speedInRange && crawlTypeDetectable)
        {
            IsFight = true;
            Debug.Log($"发现玩家！速度: {currentSpeed}, 爬行类型: {playerInput.currentCrawlName}");
        }
    }

    private float GetPlayerCurrentSpeed(PlayerInput playerInput)
    {
        // 根据当前表面类型返回对应的速度
        // 这里假设使用groundSpeed，您可以根据实��情况修改
        return playerInput.currentCrawlSettings.groundSpeed;
    }

    // 在原有的Update或其他检测方法中调用
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            CheckPlayerDetection(playerInput);
        }
    }
}
