using UnityEngine;
using System.Collections.Generic;

public class MissionToggler : MonoBehaviour
{
    private MissionManager missionManager;

    [SerializeField] private TMPro.TextMeshProUGUI[] ObjectivesPrefab; // Prefab for the mission UI element
    [SerializeField] private TMPro.TextMeshProUGUI MissionTittle;
    private float verticalSpacing = 55.42f;
    private Vector3 spawnPosition = new Vector3(0f, 12.42f, 0f);

    private void Start()
    {
        missionManager = MissionManager.Instance;
    }

    public void OnClick(string missionNameToFind)
    {
        if (missionManager != null)
        {
            MissionManager.missionData missionData = missionManager.MissionInformation.Find(m => m.MissionName == missionNameToFind);

            if (missionData != null)
            {
                // Extract mission objectives
                List<string> missionObjectives = missionData.Objectives;

                // Check if the number of objectives matches the length of ObjectivesPrefab
                if (missionObjectives.Count == ObjectivesPrefab.Length)
                {
                    MissionTittle.text = missionData.MissionName;
                    for (int i = 0; i < missionObjectives.Count; i++)
                    {
                        ObjectivesPrefab[i].text = missionObjectives[i];
                    }
                }
                else
                {
                    Debug.LogError("Number of objectives does not match the length of ObjectivesPrefab.");
                }
            }
            else
            {
                Debug.LogError($"Mission with name '{missionNameToFind}' not found.");
            }
        }
        else
        {
            Debug.LogError("MissionManager instance is not found.");
        }
    }
}
