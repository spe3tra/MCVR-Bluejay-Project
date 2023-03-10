using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneManager : MonoBehaviour
{

    // Drone interaction references
    public GameObject selectedDrone = null;
    public int selectedDroneIndex = 0;
    public List<GameObject> allDroneList = new List<GameObject>();
    public List<DroneClass> droneDataList = new List<DroneClass>();

    // UI
    public GameObject droneInfoPanel;


    // Color Changer
    Color droneColorSet = Color.white;
    public Slider sliderRed, sliderGreen, sliderBlue;
    public Image colorReference;



    // Start is called before the first frame update
    void Start()
    {
        // Temp set up data values for each drone
        foreach (GameObject drone in allDroneList)
        {
            DroneClass tempDroneData = new DroneClass();
            tempDroneData.lastPosition = drone.transform.position;
            droneDataList.Add(tempDroneData);
        }

        selectedDrone = allDroneList[selectedDroneIndex];
        GetColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called by color changing slider to input values
    public void ChangeDroneColorR(Slider slider)
    {
        droneColorSet.r = slider.value;
        ChangeDroneColorFinal();
    }

    public void ChangeDroneColorG(Slider slider)
    {
        droneColorSet.g = slider.value;
        ChangeDroneColorFinal();
    }

    public void ChangeDroneColorB(Slider slider)
    {
        droneColorSet.b = slider.value;
        ChangeDroneColorFinal();
    }

    // Set the drone color, and the 
    public void ChangeDroneColorFinal()
    {
        // Set color of UI element
        colorReference.color = droneColorSet;
        // Set saved drone color
        droneDataList[selectedDroneIndex].droneColor = droneColorSet;
        // set color of drone game object
        Renderer selectedDroneRenderer = selectedDrone.GetComponent<Renderer>();
        var colorBlock = new MaterialPropertyBlock();
        colorBlock.SetColor("_BaseColor", droneColorSet);
        selectedDroneRenderer.SetPropertyBlock(colorBlock);
    }

    public void GetColor()
    {
        Color currentDroneColor = droneDataList[selectedDroneIndex].droneColor;

        // Update sliders to current color
        colorReference.color = currentDroneColor;
        sliderRed.value = currentDroneColor.r;
        sliderGreen.value = currentDroneColor.g;
        sliderBlue.value = currentDroneColor.b;
    }
}
