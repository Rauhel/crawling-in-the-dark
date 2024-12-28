using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachinePathBase))]
public class PatrolPath : MonoBehaviour
{
    [SerializeField] private CinemachinePathBase path;
    private void Awake()
    {
        path = GetComponent<CinemachinePathBase>();
    }

    public Vector3 GetPositionAtDistance(float normalizedDistance)
    {
        return path.EvaluatePosition(normalizedDistance);
    }

    public float GetPathLength()
    {
        return path.PathLength;
    }

    public bool IsLooped()
    {
        return path.Looped;
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
                for (float t = 0; t < 0.999f; t += step)
                {
                    float nextT = Mathf.Min(t + step, 1f);
                    Gizmos.DrawLine(
                        path.EvaluatePosition(t),
                        path.EvaluatePosition(nextT)
                    );
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(path.EvaluatePosition(0), 0.3f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(path.EvaluatePosition(1), 0.3f);
            }
        }
    }
} 