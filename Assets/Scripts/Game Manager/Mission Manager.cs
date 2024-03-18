using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy,
    Intermediate,
    Hunter,
    Professional,
    Chainkiller
}

public enum RewardType
{
    XP,
    Gold,
    Diamond,
    Item
}

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    public int onLoaded = 0;

    [System.Serializable]
    public class RewardData
    {
        public RewardType Type;
        public int Value;
    }

    [System.Serializable]
    public class missionData
    {
        [Header("Mission Information")]
        public string MissionName;
        public Difficulty Difficulty;

        [Header("Mission Objectives")]
        public List<string> Objectives;

        [Header("Mission Rewards")]
        public List<RewardData> Rewards; // List of rewards with type and value.

        [Header("Mission Status")]
        public bool Completed;

        [Header("Mission Int")]
        public int Level;
    }

    public new List<missionData> MissionInformation;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


}
