using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public int Level { get; private set; }
    public int Health { get; private set; }
    public int Stamina { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {
        // Initialize player state
        Level = 1;
        Health = 100;
        Stamina = 100;
    }

    // Update is called once per frame
    void Update()
    {
        // Update player state
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
    }

    public void UseStamina(int amount)
    {
        Stamina -= amount;
        if (Stamina < 0) Stamina = 0;
    }

    public void GainExperience(int exp)
    {
        // Logic to increase level based on experience
    }
}
