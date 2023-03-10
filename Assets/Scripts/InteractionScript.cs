using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class InteractionScript : MonoBehaviour
{
    // Reference to self
    public static InteractionScript CORE;

    // Audio
    public List<AudioClip> aClips = new List<AudioClip>();
    private AudioSource aSource;
    public float UIVolume;

    // Arm Canvas
    private Transform lCanvas, rCanvas;
    private List<GameObject> UIElementsToAnimate = new List<GameObject>();
    public Text debugUIText;
    // Arm Debug
    private string prevDebugMessage;
    private int debugRepeatMessageCount = 0;

    private Transform tPlayer, lArm, rArm, headCamera;

    // Controls
    public InputActionReference leftTriggerActionRef, rightTriggerActionRef;

    // Drone order reticle
    public GameObject targetReticle;
    public bool isTargetting = false;
    public GameObject orderTargetImage, cancelTargetImage;
    public Text orderText;
    public Image targetImage;

    // Timer
    public Text timeText;
    int timeMinutes = 32;
    int timeSeconds = 0;
    int timeTracker = 0;

    private void Awake()
    {
        // Set static reference to self
        // If a duplicate exists, delete self
        if (CORE == null)
        {
            CORE = this;
        } else
        {
            Destroy(this.gameObject);
        }

        // Load settings
        PlayerPrefs.GetFloat("VolumeUI", 0.75f);
        timeMinutes = PlayerPrefs.GetInt("TimeMinutes", 32);
        timeSeconds = PlayerPrefs.GetInt("TimeSeconds", 0);
        prevDebugMessage = "Game awake";

        // Hide target reticle
        ToggleTargetting(false);

        // Setup controls
        leftTriggerActionRef.action.started += SearchForDroneLeft;
        rightTriggerActionRef.action.started += SearchForDroneRight;
    }


    // Start is called before the first frame update
    void Start()
    {
        // Get Component references
        aSource = GetComponent<AudioSource>();
        tPlayer = GameObject.Find("Player").transform;
        headCamera = tPlayer.Find("XR Origin").Find("CameraOffset").Find("Main Camera");
        lArm = tPlayer.Find("XR Origin").Find("CameraOffset").Find("LeftHand");
        lCanvas = lArm.Find("LArmCanvas");
        rArm = tPlayer.Find("XR Origin").Find("CameraOffset").Find("RightHand");
    }

    // Update is called once per frame
    void Update()
    {

        // if setting an order, update location of targetting
        RaycastHit hit;
        if (isTargetting)
        {
            hit = RaycastOut(rArm);
            try
            {
                targetReticle.SetActive(true);
                Vector3 newPos = Vector3.zero;
                newPos.y = 0;
                newPos += hit.point;

                // If touching the minimap or the ground, set color to white
                if (hit.transform.tag == "Minimap")
                {
                    transform.localScale = Vector3.one * 0.1f;
                    targetImage.color = Color.blue;
                    newPos.y = 0.34f;
                } else if (hit.transform.tag == "Ground")
                {
                    transform.localScale = Vector3.one;
                    targetImage.color = Color.white;
                    newPos.y = 0;
                } else
                {
                    // Otherwise set color to red
                    targetImage.color = Color.red;
                }

                // set reticle position
                targetReticle.transform.position = newPos;
            }
            catch
            {
                targetReticle.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log(headCamera.position);
        //Debug.Log(lArm.position);
        //Debug.Log(rArm.position);

        // Time tracker
        if (timeTracker >= 50)
        {
            // if greater than 20, decrease time
            timeTracker = 0;
            timeSeconds -= 1;
            // Save current time
            PlayerPrefs.SetInt("TimeSeconds", timeSeconds);
            if (timeSeconds == -1)
            {
                PlayerPrefs.SetInt("TimeSeconds", 59);
                timeSeconds = 59;
                timeMinutes -= 1;
                PlayerPrefs.SetInt("TimeMinutes", timeMinutes);
                if (timeMinutes <= -1)
                {
                    // When time runs out, game is over
                    PlayerPrefs.SetInt("TimeMinutes", 1);
                    ExitApplication();
                }
            }
            // Update UI
            if (timeSeconds >= 10)
            {
                timeText.text = timeMinutes + ":" + timeSeconds;
            } else
            {
                timeText.text = timeMinutes + ":0" + timeSeconds;
            }
            // Play sound
            PlaySoundLeft(5);
        } else
        {
            timeTracker += 1;
        }

        //Raycast look for object

        // Animate UI Elements loading in if needed
        if (UIElementsToAnimate.Count != 0)
        {
            foreach (GameObject g in UIElementsToAnimate)
            {
                if (g.transform.localScale.x < 1)
                {
                    g.transform.localScale += Vector3.one * 0.1f;
                }
                else
                {
                    g.transform.localScale = Vector3.one;
                    UIElementsToAnimate.Remove(g);
                    break;
                }
            }
        }
    }


    // Script to see if a drone is in the selection ray when trigger is pressed
    private void SearchForDroneLeft(InputAction.CallbackContext context)
    {
        SearchForDroneMain(lArm, "left");
    }

    private void SearchForDroneRight(InputAction.CallbackContext context)
    {
        if (isTargetting && targetReticle.activeSelf && targetImage.color != Color.red)
        {
            // If in targetting mode, add the position 
            GetComponent<DroneManager>().SetDroneOrders(targetReticle);
            PlaySound(3);
        } else if (isTargetting && targetReticle.activeSelf)
        {
            PlaySound(4);
        }
        SearchForDroneMain(rArm, "right");
    }

    private void SearchForDroneMain(Transform controller, string logStart)
    {
        // Shoot out raycast
        RaycastHit hit = RaycastOut(controller);
        // Start log message
        string logMessage = "Raycast from " + logStart + " arm";

        // Try and get game object the ray has collided with
        try
        {
            // Check if a drone was found
            if (hit.transform.tag == "Drone")
            {
                logMessage += ", drone found with tag " + hit.transform.tag;
                // Assign selected drone to drone manager
                GetComponent<DroneManager>().SelectDrone(hit.transform.gameObject);
            }
            else if (hit.transform.tag == "Subpart")
            {
                // If a sub part of a drone is hit
                logMessage += ", drone subpart found with tag " + hit.transform.tag;
                // Assign parent to drone manager
                GetComponent<DroneManager>().SelectDrone(hit.transform.parent.gameObject);
            }
            else
            {
                logMessage += ", object found with tag " + hit.transform.tag;
            }
        } catch
        {
            logMessage += ", error retrieving object";
        }

        UILog(logMessage);
    }

    RaycastHit RaycastOut(Transform controllerTransform)
    {
        RaycastHit hit;
        if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, out hit, 90))
        {
            Debug.DrawLine(transform.position, hit.point, Color.yellow);
        }

        return hit;
    }


    // Called to toggle a UI pannel on and off
    public void ToggleUI(GameObject UIToToggle)
    {
        UIToToggle.SetActive(!UIToToggle.activeSelf);
    }

    // Same as ToggleUI, but plays a specific sound
    public void ToggleUIWithSound(GameObject UIToToggle)
    {
        UIToToggle.SetActive(!UIToToggle.activeSelf);
        if (UIToToggle.activeSelf)
        {
            UIToToggle.transform.localScale = Vector3.one * 0.1f;
            PlaySound(0);
            UIElementsToAnimate.Add(UIToToggle);
        } else
        {
            PlaySound(1);
        }
    }

    // Play a specific sound on the aClips list
    public void PlaySound(int soundID)
    {
        aSource.Stop();
        transform.position = headCamera.position;
        aSource.PlayOneShot(aClips[soundID], UIVolume);
    }

    // Play a specific sound on the aClips list at the position of the left controller
    public void PlaySoundLeft(int soundID)
    {
        aSource.Stop();
        transform.position = lArm.position;
        aSource.PlayOneShot(aClips[soundID], UIVolume);
    }

    // Play a specific sound on the aClips list at the position of the right controller
    public void PlaySoundRight(int soundID)
    {
        aSource.Stop();
        transform.position = rArm.position;
        aSource.PlayOneShot(aClips[soundID], UIVolume);
    }

    // Called by the volume slider to adjust and save the sound volume of the UI elements
    public void SettingsUIVolume(Slider volume)
    {
        PlayerPrefs.SetFloat("VolumeUI", volume.value);
        PlayerPrefs.Save();
        UIVolume = volume.value;
    }

    // Quit Game
    public void ExitApplication()
    {
        Application.Quit();
    }

    // Toggle the targetting reticle
    public void ToggleTargetting()
    {
        ToggleTargetting(!isTargetting);
    }

    public void ToggleTargetting(bool newValue)
    {
        // if starting targetting
        if (newValue)
        {
            isTargetting = true;
            orderText.text = "Cancel";
            // hide arrow, show cancel image
            orderTargetImage.SetActive(false);
            cancelTargetImage.SetActive(true);
            // show game object
            targetReticle.SetActive(true);
        } else
        {
            isTargetting = false;
            orderText.text = "Order";
            // show arrow, hide cancel image
            orderTargetImage.SetActive(true);
            cancelTargetImage.SetActive(false);
            // hide game object
            targetReticle.SetActive(false);

        }
    }

    // Write a specific string to the left arm debug text log
    public void UILog(string debugMessage)
    {
        string finalUIDebug = debugMessage;
        if (prevDebugMessage == debugMessage)
        {
            debugRepeatMessageCount += 1;
            finalUIDebug = "(" + debugRepeatMessageCount + ") " + debugMessage;
        } else
        {
            debugRepeatMessageCount = 0;
            prevDebugMessage = debugMessage;
        }

        debugUIText.text = prevDebugMessage + ("\n") + finalUIDebug;
    }

}
