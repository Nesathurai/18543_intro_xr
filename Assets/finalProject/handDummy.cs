using UnityEngine;
using System;
using System.Collections.Generic; 
using System.Collections;
using System.IO;
using Newtonsoft.Json;

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
    // IDictionary<string, IDictionary<string, BoneData>> allPoses = new Dictionary<string, IDictionary<string, BoneData>>();
    IDictionary<string, IDictionary<string, BoneData>> allPoses;
    int saveCount = 0; 
    int placei = 0;
    public GameObject placeHolder; 
    Transform[] placeHolderChildren;
    void Start()
    {
        allPoses = compute.allPoses;
    }

    void Update()
    {
        // if(Input.GetKeyDown("u") || Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown("3")){
        //     int numPlaces = placeHolder.transform.childCount; 
        //     Debug.Log("child count: " + numPlaces);
        //     int i = 0;
        //     foreach(KeyValuePair<string, IDictionary<string, BoneData>> pose in allPoses){
        //         i = i % numPlaces;
        //         if(i == saveCount){
        //             Transform place = placeHolder.transform.GetChild(i);
        //             Debug.Log("placeholder: " + place.name);
        //             // GameObject newDummyHand = Instantiate(targetModel);
        //             // Debug.Log("made new dummy hand")
        //             foreach (Transform targetBone in sourceModel.gameObject.GetComponentsInChildren<Transform>())
        //             {
        //                 Debug.Log("setting bone " + targetBone.name);
        //                 if(pose.Value.ContainsKey(targetBone.name)){
        //                     targetBone.position = pose.Value[targetBone.name].position;
        //                     targetBone.rotation = pose.Value[targetBone.name].rotation;
        //                 }   
        //             }
        //             Vector3 offset = new Vector3(0, (float) 0.1, 0);
        //             targetModel.gameObject.transform.position = place.position + offset;
        //             targetModel.gameObject.transform.rotation = place.rotation;
        //             targetModel.gameObject.transform.Rotate(0,90,90);
        //             saveCount++;
        //             break;
        //         }
        //         i++;
        //     }
        //     if(saveCount == numPlaces){
        //         saveCount = 0;
        //     }
        // }
    }
    public void displayHand(string poseName){
        int numPlaces = placeHolder.transform.childCount; 
        placei = placei % numPlaces;
        Transform place = placeHolder.transform.GetChild(placei);
        // Debug.Log("placeholder: " + place.name);
        GameObject newDummyHand = Instantiate(targetModel);
        foreach (Transform targetBone in newDummyHand.gameObject.GetComponentsInChildren<Transform>())
        {
            Debug.Log("setting bone " + targetBone.name);
            if(allPoses[poseName].ContainsKey(targetBone.name)){
                targetBone.position = allPoses[poseName][targetBone.name].position;
                targetBone.rotation = allPoses[poseName][targetBone.name].rotation;
            }
        }
        Vector3 offset = new Vector3(0, (float) 0.1, 0);
        newDummyHand.gameObject.transform.position = place.position + offset;
        newDummyHand.gameObject.transform.rotation = place.rotation;
        newDummyHand.gameObject.transform.Rotate(0,90,-90);
        placei++;
    }

}
