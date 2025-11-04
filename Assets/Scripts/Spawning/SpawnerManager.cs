using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager Instance;
    private Spawner[] spawners;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        //TODO: Manually assign spawners to SpawnerManager at the start
        AssignAllSpawners();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignAllSpawners()
    {
        spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
    }

    public void EnableSilver()
    {
        foreach (Spawner spawner in spawners)
        {
            spawner.canSpawnSilver = true;
            spawner.ResetSpawns();
        }
    }

    public void EnableGold()
    {
        foreach (Spawner spawner in spawners)
        {
            spawner.canSpawnGold = true;
            spawner.ResetSpawns();
        }
    }
}
