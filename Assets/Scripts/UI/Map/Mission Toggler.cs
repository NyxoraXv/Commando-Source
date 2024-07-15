using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MissionInformationInitiator : MonoBehaviour
{
    private MissionManager missionManager;
    private string CurrentLevel = "";

    [SerializeField] private TMPro.TextMeshProUGUI[] ObjectivesPrefab; // Prefab for the mission UI element
    [SerializeField] private TMPro.TextMeshProUGUI MissionTittle;
    public GameObject pin;
    public GameObject DifficultyParent;
    public Vector3[] HologramClampPosition = new Vector3[4];

    private void Start()
    {
        missionManager = MissionManager.Instance;
        missionManager.fetch(); 
        pin.transform.localPosition=(calculate_hologram_position());
    }

    private Vector3 calculate_hologram_position()
    {
        mission_data mission = missionManager.mission;

        HologramClampPosition[0] = new Vector3(-1f, -1f, -1f);
        HologramClampPosition[1] = new Vector3(1f, -1f, -1f);
        HologramClampPosition[2] = new Vector3(-1f, 1f, 1f);
        HologramClampPosition[3] = new Vector3(1f, 1f, 1f);

        // Initialize the PRNG with the seed
        System.Random prng = new System.Random(mission.seed);

        // Select a position based on the seed
        int index = prng.Next(HologramClampPosition.Length);
        Vector3 selectedPosition = HologramClampPosition[index];

        // Set the position of the GameObject
        return (selectedPosition);
    }

    public void OnClick()
    {
        MissionTittle.text = missionManager.mission.name;

    }
    public void LoadTargetScene()
    {
        SceneManager.LoadScene(CurrentLevel);
    }
}
