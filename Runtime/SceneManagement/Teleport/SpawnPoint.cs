﻿using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    public string spawnName;
    public GameObject SpawnedObject { get; set; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = transform.position + new Vector3(0f, 0.5f, 0f);
        Gizmos.DrawWireCube(center, Vector3.one);
        Gizmos.DrawLine(center, center + transform.forward * 2);
    }
}
