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
    IDictionary<Transform, Transform> boneMap = new Dictionary<Transform, Transform>();
    IDictionary<string, IDictionary<Transform, Transform>> boneMaps = new Dictionary<string, IDictionary<Transform, Transform>>();
    IDictionary<string, BoneData> boneData = new Dictionary<string, BoneData>();
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
        // save pose
        if(Input.GetKeyDown("s") || Input.GetKey("s")){
            save();
        }
        // compare pose
        if(Input.GetKeyDown("f") || Input.GetKey("f")){
            string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\boneData2.json";
            string loaded = File.ReadAllText(path); 
            var boneData1 = JsonConvert.DeserializeObject<IDictionary<string, BoneData>>(loaded);
            Debug.Log("COMPARE: " + compare(boneData, boneData1, 0.1f));
        }
        
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            entry.Value.position = entry.Key.position;
            entry.Value.rotation = entry.Key.rotation;
        }
    }
    
    bool compare(IDictionary<string, BoneData> boneData0, IDictionary<string, BoneData> boneData1, float delta){
        // returns true if bone differences less than some delta 
        float del = 0;
        foreach(KeyValuePair<string, BoneData> entry in boneData0)
        {
            del += (entry.Value.position - boneData1[entry.Key].position).magnitude;
            // to subtract quaternions must use inverse 
            del += (Quaternion.Inverse(entry.Value.rotation) * boneData1[entry.Key].rotation).eulerAngles.magnitude;
        }
        Debug.Log(del); 
        return del < delta;
    }

    bool save(){
        if(Input.GetKeyDown("f") || Input.GetKey("f")){
            // IDictionary<string, BoneData> boneData = new Dictionary<string, BoneData>();
            string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\boneData2.json";
            foreach(KeyValuePair<Transform, Transform> entry in boneMap)
            {
                boneData.Add(entry.Value.name, new BoneData(entry.Value.transform.position + new Vector3(0.1f,0.1f,0.1f), entry.Value.transform.rotation));
            }
            string jsonData = JsonConvert.SerializeObject(boneData, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            File.WriteAllText(path, jsonData);
            Debug.Log("SAVED JSON\n"); 
        }
        return true; 
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
