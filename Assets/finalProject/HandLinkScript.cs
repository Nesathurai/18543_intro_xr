using UnityEngine;
using System;
using System.Collections.Generic; 
using System.Collections;
using System.IO;
using Newtonsoft.Json;

public class BoneRotationCopier : MonoBehaviour
{
    public GameObject sourceModel; // Reference to the model you want to copy bone rotations from.
    public GameObject targetModel; // Reference to the model you want to copy bone rotations to.
    // map from transform to transform between source and target hand
    public IDictionary<Transform, Transform> boneMap = new Dictionary<Transform, Transform>();
    
    void Start()
    {
        boneMap.Add(sourceModel.transform, targetModel.transform);
        // Iterate through each bone in the source model.
        foreach (Transform sourceBone in sourceModel.GetComponentsInChildren<Transform>())
        {
            foreach (Transform targetBone in targetModel.GetComponentsInChildren<Transform>())
            {
                if(sourceBone.name == targetBone.name){
                    boneMap.Add(sourceBone, targetBone);
                    continue;
                }
            }
        }
    }

    void Update()
    {
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            entry.Value.localPosition = entry.Key.localPosition;
            entry.Value.localRotation = entry.Key.localRotation;
            // entry.Value.position = entry.Key.position;
            // entry.Value.rotation = entry.Key.rotation;
        }
    }
    
}

public class BoneData {

    public BoneData(Vector3 Position, Quaternion Rotation){
        position = Position;
        rotation = Rotation;
    }

    public Vector3 position;
    public Quaternion rotation;
}
