using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachinePathBase))]
public class PatrolPath : MonoBehaviour
{
    private CinemachinePathBase path;
    private void Awake()
    {
        path = GetComponent<CinemachinePathBase>();
    }

    public Vector3 GetPositionAtDistance(float distance)
    {
        return path.EvaluatePosition(distance);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            var path = GetComponent<CinemachinePathBase>();
            if (path != null)
            {
                Gizmos.color = Color.green;
                float step = 0.1f;
                for (float t = 0; t < 1f; t += step)
                {
                    Gizmos.DrawLine(
                        path.EvaluatePosition(t),
                        path.EvaluatePosition(t + step)
                    );
                }
            }
        }
    }
} 