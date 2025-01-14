using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class progressBarController : MonoBehaviour
{
    public Slider progressSlider;       // Reference to the slider in the UI
    private Transform objectToFollow;    // Object that the slider will follow
    public float offsetY = 2f;          // Vertical offset to position slider above the object

    private bool isWorking = false;     // Whether the object is currently being worked on
    private float workProgress = 0f;    // Track progress percentage
    public Station myStation;

    private void Start()
    {
        // Initially hide the slider
        progressSlider.gameObject.SetActive(false);
        progressSlider.value = 0f;
        objectToFollow = myStation.gameObject.transform;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(objectToFollow.position + Vector3.up * offsetY);
        progressSlider.transform.position = screenPosition;
    }

    private void Update()
    {
        //Debug.Log (myStation.isBeingWorkedOn);
        //progressSlider.gameObject.SetActive(false);
        if (myStation.isBeingWorkedOn)
        {
            progressSlider.gameObject.SetActive(true);
            // Position the slider above the object
            objectToFollow = myStation.gameObject.transform;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(objectToFollow.position + Vector3.up * offsetY);
            progressSlider.transform.position = screenPosition;

            // Update the progress value on the slider
            float sliderValue = myStation.workProgress / myStation.workDuration;
            progressSlider.value = sliderValue;
        } 
    }
}

