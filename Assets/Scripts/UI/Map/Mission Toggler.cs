using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MissionInformationInitiator : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI[] ObjectivesPrefab;
    [SerializeField] private TMPro.TextMeshProUGUI MissionTittle;
    public GameObject pin;
    public GameObject DifficultyParent;
    public BoxCollider targetBoxCollider; // The BoxCollider for the top surface
    public GenerativeMissionManager missionManager;

    private Dictionary<Difficulty, Color> difficultyColors = new Dictionary<Difficulty, Color>
    {
        { Difficulty.Easy, new Color(188 / 255f, 255 / 255f, 171 / 255f, 1) },
        { Difficulty.Intermediate, new Color(253 / 255f, 255 / 255f, 171 / 255f, 1) },
        { Difficulty.Hunter, new Color(255 / 255f, 167 / 255f, 103 / 255f, 1) },
        { Difficulty.Professional, new Color(255 / 255f, 98 / 255f, 98 / 255f, 1) },
        { Difficulty.Chainkiller, new Color(255 / 255f, 10 / 255f, 10 / 255f, 1) }
    };

    private void Start()
    {
        if (targetBoxCollider == null)
        {
            Debug.LogError("targetBoxCollider is not assigned.");
            return;
        }

        missionManager = GenerativeMissionManager.instance;
        pin.transform.position = calculate_hologram_position();
        SetDifficultyColors();
    }

    private Vector3 calculate_hologram_position()
    {
        Bounds bounds = targetBoxCollider.bounds;

        // Initialize the PRNG with the seed
        System.Random prng = new System.Random(missionManager.missionData.seed);

        // Generate random positions within the bounds of the top surface
        float randomX = (float)(prng.NextDouble() * bounds.size.x + bounds.min.x);
        float randomY = (float)(prng.NextDouble() * bounds.size.y + bounds.min.y);

        // Set the z-coordinate to the top of the box collider
        float zPosition = bounds.max.z;

        return new Vector3(randomX, randomY, zPosition);
    }

    public void OnClick()
    {
        MissionTittle.text = missionManager.missionData.name;
        ObjectivesPrefab[0].text = "Defeat " + missionManager.missionData.total_enemy + " Enemy";
    }

    public void LoadTargetScene()
    {
        SceneManager.LoadScene("G_Scene");
    }

    private void SetDifficultyColors()
    {
        int totalEnemies = missionManager.missionData.total_enemy;
        Difficulty difficulty;

        if (totalEnemies <= 5)
        {
            difficulty = Difficulty.Easy;
        }
        else if (totalEnemies <= 10)
        {
            difficulty = Difficulty.Intermediate;
        }
        else if (totalEnemies <= 15)
        {
            difficulty = Difficulty.Hunter;
        }
        else if (totalEnemies <= 20)
        {
            difficulty = Difficulty.Professional;
        }
        else
        {
            difficulty = Difficulty.Chainkiller;
        }

        Color color = difficultyColors[difficulty];
        Image[] images = DifficultyParent.GetComponentsInChildren<Image>();

        foreach (Image image in images)
        {
            image.color = color;
        }
    }

    private enum Difficulty
    {
        Easy,
        Intermediate,
        Hunter,
        Professional,
        Chainkiller
    }
}
