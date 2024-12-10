using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;

    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(PlayerManager).ToString());
                    instance = singleton.AddComponent<PlayerManager>();
                }
            }
            return instance;
        }
    }

    public int Health { get; private set; }
    public int Fragment { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {
        // Initialize player state
        Health = 3;
        Fragment = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Update player state
    }
}
