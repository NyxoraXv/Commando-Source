using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfinerManager : MonoBehaviour
{
    private Cinemachine.CinemachineConfiner cinemachineConfiner;
    private PolygonCollider2D collider;
    private void Start()
    {
        cinemachineConfiner = gameObject.GetComponent<Cinemachine.CinemachineConfiner>();
        collider = GameObject.FindGameObjectWithTag("Confiner").GetComponent<PolygonCollider2D>();

        cinemachineConfiner.m_BoundingShape2D = collider;
    }
}
