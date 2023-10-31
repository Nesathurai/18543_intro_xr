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
    IDictionary<Transform, Transform> boneMap = new Dictionary<Transform, Transform>();
    // this stores the transform data to json for a single hand pose
    IDictionary<string, BoneData> currentPose = new Dictionary<string, BoneData>();
    // this stores the boneData for all hand poses
    IDictionary<string, IDictionary<string, BoneData>> allPoses = new Dictionary<string, IDictionary<string, BoneData>>();
    int saveCount = 0; 
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
        if(Input.GetKeyDown("s")){
            Debug.Log("STARTING SAVE");
            save();
            Debug.Log("ENDING SAVE");
        }
        // compare pose
        if(Input.GetKeyDown("c")){
            // get the closest pose 
            Debug.Log("STARTING LOAD");
            Debug.Log(loadAll());
            Debug.Log("ENDING LOAD");
            Debug.Log("STARTING COMPAREALL");
            Debug.Log("FOUND POSE: " + compareAll());
            Debug.Log("ENDING COMPAREALL");
        }
        
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            entry.Value.position = entry.Key.position;
            entry.Value.rotation = entry.Key.rotation;
        }
    }
    
    float compare(IDictionary<string, BoneData> onePose0, IDictionary<string, BoneData> onePose1){
        // returns true if bone differences less than some delta 
        float del = 0;
        foreach(KeyValuePair<string, BoneData> entry in onePose0)
        {
            // Debug.Log("Debugging0: " + entry.Key);
            // Debug.Log("Debugging1: " + onePose1[entry.Key]);
            del += (entry.Value.position - onePose1[entry.Key].position).magnitude;
            // to subtract quaternions must use inverse 
            del += (Quaternion.Inverse(entry.Value.rotation) * onePose1[entry.Key].rotation).eulerAngles.magnitude;
        }
        Debug.Log("DEL: " + del); 
        return del;
    }

    bool save(){
        string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\boneData" + saveCount + ".json";
        currentPose.Clear(); 
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            currentPose.Add(entry.Value.name, new BoneData(entry.Value.transform.position + new Vector3(0.1f,0.1f,0.1f), entry.Value.transform.rotation));
        }
        string jsonData = JsonConvert.SerializeObject(currentPose, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        File.WriteAllText(path, jsonData);
        Debug.Log("SAVED JSON\n"); 
        saveCount++; 
        return true; 
    }

    bool loadAll(){
        string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\";
        string[] files = Directory.GetFiles(path);
        Debug.Log("FILES: " + files);
        int count = 0; 
        foreach (string file in files) {
            Debug.Log("C: " + count);
            count++; 
            string loaded = File.ReadAllText(file); 
            var onePose = JsonConvert.DeserializeObject<IDictionary<string, BoneData>>(loaded);
            if(allPoses.ContainsKey(file)){
                allPoses[file] = onePose;
                // Debug.Log("FOUNd DUPLICATE");
            }
            else{
                allPoses.Add(file, onePose);
            }
            Debug.Log("FPATH: " + file);
            Debug.Log("FNAME: " + Path.GetFileNameWithoutExtension(file));
        }
        return true;
    }

    string compareAll(){
        float minDel = 999999999999.0f;
        string poseName = "NULL";
        foreach(KeyValuePair<string, IDictionary<string, BoneData>> pose in allPoses){
            float del = compare(pose.Value, currentPose);
            if(del < minDel){
                minDel = del;
                poseName = pose.Key;
            }
        }
        return poseName;
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
