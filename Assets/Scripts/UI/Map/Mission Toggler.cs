using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MissionInformationInitiator : MonoBehaviour
{
    private MissionManager missionManager;

    [SerializeField] private TMPro.TextMeshProUGUI[] ObjectivesPrefab; // Prefab for the mission UI element
    [SerializeField] private TMPro.TextMeshProUGUI MissionTittle;
    private float verticalSpacing = 55.42f;
    private Vector3 spawnPosition = new Vector3(0f, 12.42f, 0f);
    public GameObject DifficultyParent;

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

                        Image[] DifficultyArray = DifficultyParent.GetComponentsInChildren<Image>();

                        // Define a dictionary to map difficulty enum values to colors
                        Dictionary<Difficulty, Color> difficultyColors = new Dictionary<Difficulty, Color>
                        {
                            { Difficulty.Easy, new Color(188 / 255f, 255 / 255f, 171 / 255f, 1) },
                            { Difficulty.Intermediate, new Color(253 / 255f, 255 / 255f, 171 / 255f, 1) },
                            { Difficulty.Hunter, new Color(255 / 255f, 167 / 255f, 103 / 255f, 1) },
                            { Difficulty.Professional, new Color(255 / 255f, 98 / 255f, 98 / 255f, 1) },
                            { Difficulty.Chainkiller, new Color(255 / 255f, 10 / 255f, 10 / 255f, 1) }
                        };

                        // Loop through the Image components in DifficultyArray
                        for (int j = 0; j < DifficultyArray.Length; j++)
                        {
                            // Assuming the order of Image components corresponds to the order of Difficulty enum values
                            if (j < (int)missionData.Difficulty) // Explicit cast to int
                            {
                                // Get the corresponding Difficulty enum value for this bar
                                Difficulty difficulty = (Difficulty)j;

                                // Check if the difficulty value exists in the dictionary
                                if (difficultyColors.ContainsKey(difficulty))
                                {
                                    // Set the color of the Image component based on the difficulty
                                    DifficultyArray[j].color = difficultyColors[difficulty];
                                }
                                else
                                {
                                    Debug.LogError($"Color not defined for difficulty level {difficulty.ToString()}");
                                }
                            }
                            else
                            {
                                // If there are more bars than objectives, you can set a default color or handle it as needed
                                DifficultyArray[j].color = Color.gray; // Default color
                            }
                        }
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
