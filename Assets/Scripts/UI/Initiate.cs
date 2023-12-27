using UnityEngine;

public class initiate : MonoBehaviour
{
    public GameObject MainMenu;

    private void Start()
    {
        Instantiate(MainMenu);
    }
}
