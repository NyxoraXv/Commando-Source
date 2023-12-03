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

[ExecuteInEditMode]
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

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
        public string MissionID;
        public Difficulty Difficulty;

        [Header("Mission Objectives")]
        public List<string> Objectives;

        [Header("Mission Rewards")]
        public List<RewardData> Rewards;

        [Header("Mission Status")]
        public bool Completed;
    }

    public new List<missionData> MissionInformation;

    private void Awake()
    {
        Instance = this;
    }

    public missionData GetMissionByID(string missionID)
    {
        foreach (missionData mission in MissionInformation)
        {
            if (mission.MissionID == missionID)
            {
                return mission;
            }
        }

        // If no mission with the given ID is found, return null
        return null;
    }



}
