using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Win : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collided");
        GameManager.PlayerWin();
    }
}
