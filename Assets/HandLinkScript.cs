using UnityEngine;
using System;
using System.Collections.Generic; 
using System.Collections;
using Newtonsoft.Json;

public class BoneRotationCopier : MonoBehaviour
{
    public GameObject sourceModel; // Reference to the model you want to copy bone rotations from.
    public GameObject targetModel; // Reference to the model you want to copy bone rotations to.
    IDictionary<Transform, Transform> boneMap = new Dictionary<Transform, Transform>();
    void Start()
    {
        IDictionary<string, BoneData> boneData = new Dictionary<string, BoneData>();
        boneMap.Add(sourceModel.transform, targetModel.transform);
        boneData.Add(sourceModel.name, new BoneData(sourceModel.transform.position, sourceModel.transform.rotation));
        // Iterate through each bone in the source model.
        foreach (Transform sourceBone in sourceModel.GetComponentsInChildren<Transform>())
        {
            foreach (Transform targetBone in targetModel.GetComponentsInChildren<Transform>())
            {
                if(sourceBone.name == targetBone.name){
                    boneMap.Add(sourceBone, targetBone);
                    boneData.Add(sourceBone.name, new BoneData(sourceBone.transform.position, sourceBone.transform.rotation));
                    // Debug.Log(sourceBone.name);
                    // targetBone.transform.localRotation = sourceBone.transform.localRotation;
                    continue;
                }
            }
        }
        string jsonData = JsonConvert.SerializeObject(boneData, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        Debug.Log(jsonData);
        // SaveData myData;
    }


    void Update()
    {
        // targetModel.transform.position = sourceModel.transform.position;
        // targetModel.transform.rotation = sourceModel.transform.rotation;
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            entry.Value.position = entry.Key.position;
            entry.Value.rotation = entry.Key.rotation;
        }
        
        // Iterate through each bone in the source model.
        // foreach (Transform sourceBone in sourceModel.GetComponentsInChildren<Transform>())
        // {
        //     foreach (Transform targetBone in targetModel.GetComponentsInChildren<Transform>())
        //     {
        //         if(sourceBone.name == targetBone.name){
        //             targetBone.transform.localRotation = sourceBone.transform.localRotation;
        //             continue;
        //         }
        //     }
        // }
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
