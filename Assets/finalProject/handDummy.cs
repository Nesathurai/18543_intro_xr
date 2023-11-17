using UnityEngine;
using System;
using System.Collections.Generic; 
using System.Collections;
using System.IO;
using Newtonsoft.Json;

public class handDummy : MonoBehaviour
{
    public GameObject sourceModel; // Reference to the model you want to copy bone rotations from.
    public GameObject targetModel; // Reference to the model you want to copy bone rotations to.
    // map from transform to transform between source and target hand
    IDictionary<Transform, Transform> boneMap = new Dictionary<Transform, Transform>();
    // this stores the transform data to json for a single hand pose
    IDictionary<string, BoneData> currentPose;
    public Compute compute; 
    // this stores the boneData for all hand poses
    IDictionary<string, IDictionary<string, BoneData>> allPoses = new Dictionary<string, IDictionary<string, BoneData>>();
    int saveCount = 0; 
    public GameObject placeHolder; 
    Transform[] placeHolderChildren;
    void Start()
    {
        // get the locations of the placeholders to replace 
        // for(int i = 0; i < placeHolder.transform.childCount; ++i){
        //     Transform place = placeHolder.transform.GetChild(i);
        //     // Debug.Log("placeholder: " + place.name);
        //     placeHolderChildren[i] = place;
        // }
        currentPose = compute.currentPose;
    }

    void Update()
    {
        if(Input.GetKeyDown("u") || Input.GetKeyDown(KeyCode.Keypad3)){
            for(int i = 0; i < placeHolder.transform.childCount; ++i){
                Transform place = placeHolder.transform.GetChild(i);
                Debug.Log("placeholder: " + place.name);
                // placeHolderChildren[i] = place;
                // if(allPoses.Count >= i){
                GameObject newDummyHand = Instantiate(targetModel);
                Debug.Log("made new dummy hand");
                // now go through the bones in new dummy hand and assign the correct positions 
                // boneMap.Add(sourceModel.transform, targetModel.transform);
                // Iterate through each bone in the source model.
                foreach (Transform sourceBone in sourceModel.GetComponentsInChildren<Transform>())
                {
                    foreach (Transform targetBone in newDummyHand.GetComponentsInChildren<Transform>())
                    {
                        if(sourceBone.name == targetBone.name){
                            Debug.Log("setting bone " + sourceBone.name);
                            targetBone.position = sourceBone.position;
                            targetBone.rotation = sourceBone.rotation;
                            continue;
                        }
                    }
                }
                newDummyHand.transform.position = place.position;
                newDummyHand.transform.rotation = place.rotation;

                // }
            }
        }
    }
    
    
}
