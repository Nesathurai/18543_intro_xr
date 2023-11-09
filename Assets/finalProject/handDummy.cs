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
    IDictionary<string, BoneData> currentPose = new Dictionary<string, BoneData>();
    // this stores the boneData for all hand poses
    IDictionary<string, IDictionary<string, BoneData>> allPoses = new Dictionary<string, IDictionary<string, BoneData>>();
    int saveCount = 0; 
    void Start()
    {

    }

    void Update()
    {
        
    }
    
    
}
