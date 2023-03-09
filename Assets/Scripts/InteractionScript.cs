using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class InteractionScript : MonoBehaviour
{
   
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


    private void Awake()
    {
        PlayerPrefs.GetFloat("VolumeUI", 0.75f);
        prevDebugMessage = "Game awake";
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
        
    }

    private void FixedUpdate()
    {
        //Debug.Log(headCamera.position);
        //Debug.Log(lArm.position);
        //Debug.Log(rArm.position);
        // Animate UI Elements loading in
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

    public void Hello()
    {

    }

    public void ToggleUI(GameObject UIToToggle)
    {
        UIToToggle.SetActive(!UIToToggle.activeSelf);
    }

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

    public void WhatsUp(GameObject selectedObject)
    {

    }

    public void PlaySound(int soundID)
    {
        aSource.Stop();
        transform.position = headCamera.position;
        aSource.PlayOneShot(aClips[soundID], UIVolume);
    }

    public void PlaySoundLeft(int soundID)
    {
        aSource.Stop();
        transform.position = lArm.position;
        aSource.PlayOneShot(aClips[soundID], UIVolume);
    }

    public void PlaySoundRight(int soundID)
    {
        aSource.Stop();
        transform.position = rArm.position;
        aSource.PlayOneShot(aClips[soundID], UIVolume);
    }

    // Change Settings Values
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
