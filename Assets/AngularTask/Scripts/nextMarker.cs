using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextMarker : MonoBehaviour
{
    public GameObject nextMark;
    public GameObject lastMark;
    public Camera arCamera;
    public float YRotation; // Public variable for Y rotation
    public bool isAnticlockwise; // Public variable for rotation direction

    private bool isLastMarkerDeactivated = false;
    private bool hasRotatedCorrectly = false;
    private float initialYRotation;

    void Start()
    {
        initialYRotation = arCamera.transform.eulerAngles.y;
    }

    void Update()
    {
        if (!isLastMarkerDeactivated && lastMark != null && !lastMark.activeSelf)
        {
            isLastMarkerDeactivated = true;
            initialYRotation = arCamera.transform.eulerAngles.y; // Reset initial rotation when the last marker is deactivated
        }

        if (isLastMarkerDeactivated && !hasRotatedCorrectly)
        {
            hasRotatedCorrectly = CheckRotationCompletion();
        }

        if (hasRotatedCorrectly && IsPlayerFacingCorrectDirection())
        {
            nextMark.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    bool CheckRotationCompletion()
    {
        float currentYRotation = arCamera.transform.eulerAngles.y;
        float rotationDifference = isAnticlockwise ? (initialYRotation - currentYRotation) : (currentYRotation - initialYRotation);

        rotationDifference = (rotationDifference + 360f) % 360f;

        // Check if the player has completed the required rotation
        return rotationDifference >= YRotation;
    }

    bool IsPlayerFacingCorrectDirection()
    {
        float currentYRotation = arCamera.transform.eulerAngles.y;
        float targetYRotation = (initialYRotation + (isAnticlockwise ? -YRotation : YRotation)) % 360f;

        // Adjust targetYRotation to be within the range [0, 360]
        if (targetYRotation < 0)
            targetYRotation += 360f;

        float rotationDifference = Mathf.Abs(currentYRotation - targetYRotation);

        // Check for wrap-around case
        if (rotationDifference > 180f)
            rotationDifference = 360f - rotationDifference;

        // Return true if the difference is very close to 0 (exactly matching the target)
        return rotationDifference < 1f; // Adjust this threshold if necessary to allow for slight inaccuracies //try <0.1f
    }
}
