using UnityEngine;
using System;
using System.Collections.Generic; 
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using TMPro;

public class HandDummy : MonoBehaviour
{
    public GameObject sourceModel; // Reference to the model you want to copy bone rotations from.
    public GameObject targetModel; // Reference to the model you want to copy bone rotations to.
    // map from transform to transform between source and target hand
    IDictionary<Transform, Transform> boneMap = new Dictionary<Transform, Transform>();
    // this stores the transform data to json for a single hand pose
    IDictionary<string, BoneData> currentPose;
    public Compute compute; 
    // this stores the boneData for all hand poses
    IDictionary<string, IDictionary<string, BoneData>> allPoses;
    int saveCount = 0; 
    int placei = 0;
    public GameObject placeHolder; 
    Transform[] placeHolderChildren;
    public Vector3 manualOffset; 
    void Start()
    {
        manualOffset = new Vector3((float) 0.0, (float) 0.0, (float) 0.0);
        allPoses = compute.allPoses;
    }

    // void Update()
    // {
    // }
    public void displayHand(string poseName){
        int numPlaces = placeHolder.transform.childCount; 
        // place like thumbs up, contains the thumbs up frame, and the text frame
        placei = placei % numPlaces;
        Transform place = placeHolder.transform.GetChild(placei);
        // Debug.Log("placeholder: " + place.name);
        GameObject newDummyHand = Instantiate(targetModel);
        foreach (Transform targetBone in newDummyHand.gameObject.GetComponentsInChildren<Transform>())
        {
            // Debug.Log("setting bone " + targetBone.name);
            if(allPoses[poseName].ContainsKey(targetBone.name)){
                targetBone.position = allPoses[poseName][targetBone.name].position;
                targetBone.rotation = allPoses[poseName][targetBone.name].rotation;
            }
        }
        Vector3 offset = new Vector3(0, (float) 0.1, 0);
        newDummyHand.gameObject.transform.position = place.position + offset + manualOffset;
        newDummyHand.gameObject.transform.rotation = place.rotation;
        newDummyHand.gameObject.transform.Rotate(0,90,-90);

        // update text info frame
        place.GetChild(1).GetChild(1).GetChild(0).GetComponentInChildren<TMP_Text>().text = poseName;
        placei++;
    }
}
