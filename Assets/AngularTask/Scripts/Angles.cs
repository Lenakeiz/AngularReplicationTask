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
            Vector3 randomShift = GetRandomShiftWithinRadius(originalStartingPoint); // Get a random position within a radius
            GameObject prefabToUse = Anticlockwise[i] ? AngularTaskPrefabAntiClock : AngularTaskPrefabClock; // Select the correct prefab based on rotation direction
            angularInstances[i] = Instantiate(prefabToUse, originalStartingPoint + randomShift, Quaternion.identity, transform); // Instantiate the marker

            // Set rotation
            float rotation = Anticlockwise[i] ? -YRotations[i] : YRotations[i];
            RotateChildObjects(angularInstances[i], rotation);

            // Log the rotation and direction
            Debug.Log($"Marker {i}: Rotation = {rotation}, Anticlockwise = {Anticlockwise[i]}");

            // Set yRotation in SpawnEndMark
            angularInstances[i].GetComponent<SpawnEndMark>().yRotation = YRotations[i];

            // Set Anticlockwise in SpawnEndMark
            angularInstances[i].GetComponent<SpawnEndMark>().Anticlockwise = Anticlockwise[i];

            // Set YRotation and isAnticlockwise in nextMarker script
            nextMarker nm = angularInstances[i].GetComponent<nextMarker>(); //nm = nextMarker 
            if (nm != null)
            {
                nm.YRotation = YRotations[i];
                nm.isAnticlockwise = Anticlockwise[i]; // Ensure isAnticlockwise is set
            }

            if (i != 0)
            {
                angularInstances[i].SetActive(false); // Set all markers except the first one to inactive
            }

            if (i == 7) // If this is the last instance
            {
                angularInstances[i].GetComponent<SpawnEndMark>().NextBlock = NextBlockRef; // Link the last instance to the next block reference
            }
        }

        for (int i = 0; i < 7; i++)
        {
            angularInstances[i].GetComponent<SpawnEndMark>().nextAngle = angularInstances[i + 1]; // Link the next and previous angles
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