using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneClass
{
    // Aesthetic
    public string droneName;
    public Color droneColor;

    // Position
    public List<string> droneOrders;
    public Vector3 lastPosition;

    // Values
    public float batteryRemaining, batteryMax;
    public GameObject carriedObject;

    /// <summary>
    /// Drone Data storage class. Defaults to yellow at position 0 0 0 with a random battery percentage
    /// </summary>
    public DroneClass()
    {
        droneName = "Loading Drone " + Random.Range(1, 999) + "-" + (char)('A' + Random.Range(0, 26));
        droneColor = Color.yellow;
        droneOrders = new List<string>();
        lastPosition = Vector3.zero;
        batteryRemaining = Random.Range(94.1f, 99.4f);
        batteryMax = 100f;
    }

    /// <summary>
    /// Drone Data storage class
    /// </summary>
    /// <param name="nameOfDrone"></param>
    /// <param name="colorOfDrone"></param>
    /// <param name="lastSavedPosition"></param>
    /// <param name="batteryAmountRemaining"></param>
    /// <param name="batteryMaxValue"></param>
    public DroneClass(string nameOfDrone, Color colorOfDrone, Vector3 lastSavedPosition, float batteryAmountRemaining, float batteryMaxValue)
    {
        droneName = nameOfDrone;
        droneColor = colorOfDrone;
        lastPosition = lastSavedPosition;
        batteryRemaining = batteryAmountRemaining;
        batteryMax = batteryMaxValue;
    }

    /// <summary>
    /// Drone Data storage class
    /// </summary>
    /// <param name="nameOfDrone"></param>
    /// <param name="colorOfDrone"></param>
    /// <param name="lastSavedPosition"></param>
    /// <param name="batteryAmountRemaining"></param>
    /// <param name="batteryMaxValue"></param>
    /// <param name="heldItem"></param>
    public DroneClass(string nameOfDrone, Color colorOfDrone, Vector3 lastSavedPosition, float batteryAmountRemaining, float batteryMaxValue, GameObject heldItem)
    {
        droneName = nameOfDrone;
        droneColor = colorOfDrone;
        lastPosition = lastSavedPosition;
        batteryRemaining = batteryAmountRemaining;
        batteryMax = batteryMaxValue;
        carriedObject = heldItem;
    }
}
