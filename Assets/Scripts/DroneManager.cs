using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class DroneManager : MonoBehaviour
{

    // Drone interaction references
    public GameObject selectedDrone = null;
    public int selectedDroneIndex = 0;
    public List<GameObject> allDroneList = new List<GameObject>();
    public List<DroneClass> droneDataList = new List<DroneClass>();

    // UI
    public GameObject droneInfoPanel;
    public Text droneInfoNameText;


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
        if (selectedDroneIndex != -1)
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
        } else
        {
            InteractionScript.CORE.UILog("No Drone Selected to change color");
        }
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

    // Select a drone, and retrieve it's index
    public void SelectDrone(GameObject droneToSelect)
    {
        if (selectedDrone == droneToSelect)
        {
            InteractionScript.CORE.ToggleTargetting();
            selectedDrone = droneToSelect;
        } else
        {
            selectedDrone = droneToSelect;
            InteractionScript.CORE.ToggleTargetting(false);
        }
        selectedDrone = droneToSelect;
        selectedDroneIndex = allDroneList.IndexOf(droneToSelect);
        // Get drone info from index
        droneInfoNameText.text = droneDataList[selectedDroneIndex].droneName;
        // Enable interacter screen
        if (!droneInfoPanel.activeInHierarchy)
        {
            InteractionScript.CORE.ToggleUI(droneInfoPanel);
            InteractionScript.CORE.PlaySound(2);
        } else
        {
            InteractionScript.CORE.PlaySound(2);
        }
        GetColor();
    }

    // deselect the current drone
    public void DeselectDrone()
    {
        selectedDrone = null;
        selectedDroneIndex = -1;
        InteractionScript.CORE.ToggleTargetting(false);
    }

    // Set the Navmesh point of a drone
    public void SetDroneOrders(GameObject moveToPoint)
    {
        droneDataList[selectedDroneIndex].nextWaypoint = moveToPoint.transform.position;
        NavMeshAgent droneNav = allDroneList[selectedDroneIndex].GetComponent<NavMeshAgent>();
        droneNav.destination = moveToPoint.transform.position;
    }

    // Enable disable the Navmesh Agent
    public void ToggleNavmesh()
    {
        NavMeshAgent droneNavMesh = selectedDrone.GetComponent<NavMeshAgent>();
        droneNavMesh.enabled = !droneNavMesh.enabled;
    }

    public void ToggleNavmesh(GameObject droneToToggle)
    {
        NavMeshAgent droneNavMesh = droneToToggle.GetComponent<NavMeshAgent>();
        droneNavMesh.enabled = !droneNavMesh.enabled;
    }
}
