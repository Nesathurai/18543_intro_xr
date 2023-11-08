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
        // save pose
        if(Input.GetKeyDown("l")){
            Debug.Log("STARTING LOAD");
            load();
            Debug.Log("ENDING LOAD");
        }
    }
    
    void load(){
        // take a file from the disk and load it into a hand dummy 
        string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\";
        string[] files = Directory.GetFiles(path);
        Debug.Log("FILES: " + files);
        int count = 0; 
        foreach (string file in files) {
            Debug.Log("C: " + count);
            if(count == 0){
                Debug.Log("LOADING: " + file);
                string loaded = File.ReadAllText(file); 
                var onePose = JsonConvert.DeserializeObject<IDictionary<string, BoneData>>(loaded);
                foreach (Transform targetBone in targetModel.GetComponentsInChildren<Transform>())
                {
                    if(onePose.ContainsKey(targetBone.name)){
                        Debug.Log("LOADED__");
                        targetBone.position = onePose[targetBone.name].position;
                        targetBone.rotation = onePose[targetBone.name].rotation;
                    }
                }
            }
            count++; 
        }
    }
}
