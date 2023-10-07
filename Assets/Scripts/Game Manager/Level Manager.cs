using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public event Action<int> onXPAdded;

    public int maxLevels = 5;
    public int currentLevel = 1;
    public int CurrentXP = 0;

    [HideInInspector]
    public int[] maxXPPerLevel;

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

    private void Start()
    {
        initializeMaxXPPerLevel();
    }

    void initializeMaxXPPerLevel()
    {
        maxXPPerLevel = new int[maxLevels];

        maxXPPerLevel[0] = 1000;
        maxXPPerLevel[1] = 1500;
        maxXPPerLevel[2] = 2000;
        maxXPPerLevel[3] = 3000;
        maxXPPerLevel[4] = 5000;
    }

    public void addXP(int XP)
    {
        CurrentXP += XP;
        CheckForLevelUp();
        onXPAdded?.Invoke(XP);
    }

    private void CheckForLevelUp()
    {
        while (currentLevel < maxLevels && CurrentXP >= maxXPPerLevel[currentLevel])
        {
            CurrentXP -= maxXPPerLevel[currentLevel];
            currentLevel++;
        }
    }
}
