using UnityEngine;
using System.Collections.Generic; 


public class BoneRotationCopier : MonoBehaviour
{
    public GameObject sourceModel; // Reference to the model you want to copy bone rotations from.
    public GameObject targetModel; // Reference to the model you want to copy bone rotations to.
    IDictionary<Transform, Transform> boneMap = new Dictionary<Transform, Transform>();
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
                    // targetBone.transform.localRotation = sourceBone.transform.localRotation;
                    continue;
                }
            }
        }
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