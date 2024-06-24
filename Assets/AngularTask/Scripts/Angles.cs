using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angles : MonoBehaviour
{
    public GameObject AngularTaskPrefabClock; // Prefab for clock-wise rotation markers
    public GameObject AngularTaskPrefabAntiClock; // Prefab for anti-clockwise rotation markers
    public GameObject NextBlockRef; // Reference to the next block object
    public float[] YRotations = new float[8]; // Array of Y rotations for each marker
    public bool[] Anticlockwise = new bool[8]; // Direction array for marker rotation

    private GameObject[] angularInstances = new GameObject[8]; // Array to store instances of markers
    private Vector3 originalStartingPoint = Vector3.zero; // Starting point for positioning markers

    void Start()
    {
        originalStartingPoint = new Vector3(0, 0, 0); // Initialize starting point

        for (int i = 0; i < 8; i++)
        {
            // Get a random position within a radius
            Vector3 randomShift = GetRandomShiftWithinRadius(originalStartingPoint);

            // Determine which prefab to use based on Anticlockwise array
            GameObject prefabToUse = Anticlockwise[i] ? AngularTaskPrefabAntiClock : AngularTaskPrefabClock;
            if (prefabToUse != null)
            {
                // Instantiate the chosen prefab at specified positions
                angularInstances[i] = Instantiate(prefabToUse, originalStartingPoint + randomShift, Quaternion.identity, transform);
                angularInstances[i].GetComponent<SpawnEndMark>().Anticlockwise = Anticlockwise[i];

                // If not the first instance, set active to false 
                if (i != 0)
                {
                    angularInstances[i].SetActive(false); // Set all markers except the first one to inactive
                }

                // Set rotation
                RotateChildObjects(angularInstances[i], Anticlockwise[i] ? -YRotations[i] : YRotations[i]);

                // Set yRotation in SpawnEndMark
                angularInstances[i].GetComponent<SpawnEndMark>().yRotation = YRotations[i];
            }

            // If it's the last (8th) instance, assign the NextBlockRef to it
            if (i == 7 && angularInstances[i] != null)
            {
                angularInstances[i].GetComponent<SpawnEndMark>().NextBlock = NextBlockRef; // Link the last instance to the next block reference // I don't think this is interfering with block structure with breaks?
            }

            // Assign configurations for breaks and completion
            // Do I modify this code chunk and move it under the prev condition: if NextBlockRef name contains (or a more efficient method) ... etc
            BlockConfig config = angularInstances[i].GetComponent<BlockConfig>() ?? angularInstances[i].AddComponent<BlockConfig>();
            if (angularInstances[i].name.Contains("A2") || angularInstances[i].name.Contains("B2"))
            {
                config.requiresBreak = true;
            }
            if (angularInstances[i].name.Contains("C2"))
            {
                config.isFinalBlock = true;
            }
        }

        // Set nextAngle and prevAngle fields (linking the next and prev angles)
        for (int i = 0; i < 7; i++)
        {
            var spawnEndMark = angularInstances[i].GetComponent<SpawnEndMark>();
            if (spawnEndMark != null)
            {
                spawnEndMark.nextAngle = angularInstances[i + 1];
            }
        }
    }

    private Vector3 GetRandomShiftWithinRadius(Vector3 origin)
    {
        Vector2 randomCircle = Random.insideUnitCircle * 1.0f; // Calculate a random position within a 1-unit circle
        return new Vector3(randomCircle.x, 0, randomCircle.y); // Return the new position
    }

    void RotateChildObjects(GameObject parent, float rotation)
    {
        Transform center = parent.transform.Find("Center");
        if (center != null)
        {
            center.Rotate(0, rotation, 0);
        }

        Transform startCenter = parent.transform.Find("StartCenter");
        if (startCenter != null)
        {
            startCenter.Rotate(0, rotation, 0);
        }

        Transform firstCenter = parent.transform.Find("FirstCenter");
        if (firstCenter != null)
        {
            firstCenter.Rotate(0, rotation * 2, 0);
        }
    }
}