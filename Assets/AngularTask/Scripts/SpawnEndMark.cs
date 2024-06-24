using System.Collections;
using UnityEngine;
using Wave.OpenXR.Toolkit.Samples;

public class SpawnEndMark : MonoBehaviour
{
    public GameObject endMarker;
    public GameObject nextAngle;
    public GameObject NextBlock;
    public GameObject BreakPanel;  // Reference to the break panel
    public GameObject CompletionPanel;  // Reference to the completion panel
    public float delay = 2f;
    public float checkTime = 6f;
    private GameObject currentEndMarker;
    private Camera mainCamera;
    private bool buttonIsInCooldown = false;
    private bool buttonPressedAgain = false;
    private bool isSpaceButtonDown = false;
    private RotationLogger rotationLogger;
    public float yRotation;
    public bool Anticlockwise;

    void Start()
    {
        mainCamera = Camera.main;
        GameObject rotationLoggerObject = GameObject.Find("RotationLoggerManager");
        if (rotationLoggerObject != null)
        {
            rotationLogger = rotationLoggerObject.GetComponent<RotationLogger>();
        }
    }

    void Update()
    {
        bool isVrTriggerDown = VRSInputManager.instance.GetButtonDown(VRSButtonReference.TriggerL);
        bool isJumpButtonDown = Input.GetButtonDown("Jump");

        if (Input.GetButtonUp("Jump"))
        {
            isSpaceButtonDown = false;
        }

        if (!buttonIsInCooldown && (isVrTriggerDown || (isJumpButtonDown && !isSpaceButtonDown)))
        {
            isSpaceButtonDown = true;

            if (rotationLogger != null && rotationLogger.DoesCsvExist())
            {
                Debug.Log("[SpawnEndMark] Logging rotation for marker: Main Camera Angle = " + mainCamera.transform.rotation.eulerAngles + ", Position = " + mainCamera.transform.position + ", yRotation = " + yRotation + ", Anticlockwise = " + Anticlockwise);
                rotationLogger.LogRotation("MarkerSpawned", mainCamera.transform.rotation.eulerAngles, mainCamera.transform.position, yRotation, Anticlockwise);
            }

            buttonPressedAgain = true;

            if (mainCamera != null)
            {
                if (currentEndMarker != null)
                {
                    Destroy(currentEndMarker);
                }

                Vector3 cameraRotation = mainCamera.transform.rotation.eulerAngles;
                currentEndMarker = Instantiate(endMarker, this.transform.position, Quaternion.Euler(0, cameraRotation.y, 0));
                Debug.Log("Current rotation set to: " + yRotation);

                StartCoroutine(ButtonCooldown());
                StartCoroutine(CheckButtonPress());
            }
        }
    }


    IEnumerator ButtonCooldown()
    {
        buttonIsInCooldown = true;
        yield return new WaitForSeconds(delay);
        buttonIsInCooldown = false;
    }

    IEnumerator CheckButtonPress()
    {
        buttonPressedAgain = false;
        yield return new WaitForSeconds(checkTime);
        if (!buttonPressedAgain)
        {
            Debug.Log("[SpawnEndMark] CheckButtonPress: No additional button press detected.");

            if (nextAngle != null)
            {
                Debug.Log("Activating Next Angle: " + nextAngle.name);
                nextAngle.SetActive(true);
                UIManager.Instance.StartFadeIn();
                gameObject.SetActive(false);
            }
            else if (NextBlock != null)
            {
                Debug.Log("Activating Next Block: " + NextBlock.name);
                if (NextBlock.GetComponent<BlockConfig>().requiresBreak)
                {
                    BreakPanel.SetActive(true); // Activate the break panel when required
                }
                else if (NextBlock.GetComponent<BlockConfig>().isFinalBlock)
                {
                    CompletionPanel.SetActive(true); // Activate the completion panel if it's the final block
                }
                else
                {
                    NextBlock.SetActive(true);
                    Destroy(transform.parent.gameObject);
                }
            }
        }
    }


    public void ContinueGame()
    {
        BreakPanel.SetActive(false); // Hide the break panel
        if (NextBlock != null)
        {
            NextBlock.SetActive(true); // Activate the next block directly here 
        }
    }
}