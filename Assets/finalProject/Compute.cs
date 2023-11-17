using UnityEngine;
using System;
using System.Collections.Generic; 
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using TMPro;

public class Compute : MonoBehaviour
{
    public BoneRotationCopier handLinkScript;
    public Oculus.Interaction.Samples.PoseUseSample poseDisplay;
    
    public Oculus.Interaction.Samples.CreateVisuals createVisuals;
    
    // this stores the boneData for all hand poses
    public IDictionary<string, IDictionary<string, BoneData>> allPoses = new Dictionary<string, IDictionary<string, BoneData>>();

    // this stores the transform data to json for a single hand pose
    public IDictionary<Transform, Transform> boneMap = new Dictionary<Transform, Transform>();
    // this stores the transform data to json for a single hand pose
    public IDictionary<string, BoneData> currentPose = new Dictionary<string, BoneData>();
    private GameObject targetModel; // Reference to the model you want to copy bone rotations to.
    int saveCount = 0; 
    public TextMeshPro text;
    public Transform hmd;
    // Start is called before the first frame update
    void Start()
    {
        boneMap = handLinkScript.boneMap;
        targetModel = handLinkScript.targetModel;
        // generate text in create visuals 
        createVisuals.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // save pose
        if(Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.Keypad1)){
            Debug.Log("STARTING SAVE");
            save();
            Debug.Log("ENDING SAVE");
        }

        // load all poses and compare 
        if(Input.GetKeyDown("c") || Input.GetKeyDown(KeyCode.Keypad2)){
            // get the closest pose 
            Debug.Log("STARTING LOAD");
            Debug.Log(loadAll());
            Debug.Log("ENDING LOAD");
            Debug.Log("STARTING COMPAREALL");
            Debug.Log("FOUND POSE: " + compareAll());
            Debug.Log("ENDING COMPAREALL");
        }
    }
    bool save(){
        string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\boneData" + saveCount + ".json";
        currentPose.Clear(); 
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            // currentPose.Add(entry.Value.name, new BoneData(entry.Value.transform.position + new Vector3(0.1f,0.1f,0.1f), entry.Value.transform.rotation));
            currentPose.Add(entry.Value.name, new BoneData(entry.Value.transform.position, entry.Value.transform.rotation));
        }
        // now add this to all poses (so that the display text can pop up)
        string fname = "boneData"+saveCount.ToString();
        if(allPoses.ContainsKey(fname)){
            allPoses[fname] = currentPose;
        }
        else{
            allPoses.Add(fname, currentPose);
        }
        string jsonData = JsonConvert.SerializeObject(currentPose, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        File.WriteAllText(path, jsonData);
        // Debug.Log("SAVED JSON\n"); 
        load(fname);
        createVisuals.ManualUpdate();
        createVisuals.ShowVisuals(fname);
        saveCount++; 
        return true; 
    }
    bool load(string fname){
        string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\" + fname + ".json";
        string loaded = File.ReadAllText(path); 
        var onePose = JsonConvert.DeserializeObject<IDictionary<string, BoneData>>(loaded);
        if(allPoses.ContainsKey(fname)){
            allPoses[fname] = onePose;
        }
        else{
            allPoses.Add(fname, onePose);
        }
        Debug.Log("FNAME load: " + fname);
        return true;
    }
    float compare(IDictionary<string, BoneData> onePose0, IDictionary<string, BoneData> onePose1){
        // good reference: https://www.youtube.com/watch?v=lBzwUKQ3tbw
        // returns true if bone differences less than some delta 
        float del = 0;
        foreach(KeyValuePair<string, BoneData> entry in onePose0)
        {
            // TODO: do inverse transform here?
            // Vector3 p0 = hmd.InverseTransformPoint(entry.Value.position);
            // Vector3 p1 = hmd.InverseTransformPoint(onePose1[entry.Key].position);
            // Debug.Log(hmd);
            // Debug.Log("val0: " + entry.Value.position);
            // Debug.Log("val1: " + onePose1[entry.Key].position);
            // Debug.Log("invt0: " + p0);
            // Debug.Log("invt1: " + p1);
            // float d = (float) Math.Pow(Vector3.Distance(p0, p1), 2);
            // Debug.Log("del: " + d); 
            // del += d;
            // Debug.Log("pos: " + (entry.Value.position - onePose1[entry.Key].position).magnitude);
            // Debug.Log("rot: " + (Quaternion.Inverse(entry.Value.rotation) * onePose1[entry.Key].rotation).eulerAngles.magnitude);
            // Single rot = (onePose1[entry.Key].rotation * Quaternion.Inverse(entry.Value.rotation)).normalized.eulerAngles.magnitude % 360;
            // Vector3 curr = entry.Value.Transform.InverseTransformPoint(onePose1[entry.Key].position);
            // Debug.Log("curr: " + curr);
            // del += Vector3.Distance(curr, onePose1[entry.Key].position);
            // Debug.Log(entry.Key + " -> " + Math.Pow((entry.Value.position - onePose1[entry.Key].position).magnitude, 2).ToString() + " | " + rot);
            del += (Single) Math.Pow((entry.Value.position - onePose1[entry.Key].position).magnitude, 2);
            // Debug.Log("dist: " + Vector3.Distance(entry.Value.position, onePose1[entry.Key].position));
            // del += (Single) Math.Pow(Vector3.Distance(entry.Value.position, onePose1[entry.Key].position),2);
            // to subtract quaternions must use inverse 
            // del += rot;
        }
        // Debug.Log("DEL: " + del); 
        return del;
    }
    
    bool loadAll(){
        string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\";
        string[] files = Directory.GetFiles(path);
        int count = 0; 
        foreach (string file in files) {
            count++; 
            string fname = Path.GetFileNameWithoutExtension(file);
            string loaded = File.ReadAllText(file); 
            var onePose = JsonConvert.DeserializeObject<IDictionary<string, BoneData>>(loaded);
            if(allPoses.ContainsKey(fname)){
                allPoses[fname] = onePose;
            }
            else{
                allPoses.Add(fname, onePose);
            }
            Debug.Log("FNAME: " + fname);
        }
        
        return true;
    }

    string compareAll(){
        float minDel = 999999999999.0f;
        string poseName = "NULL";
        // grab current pose to compare against
        currentPose.Clear(); 
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            currentPose.Add(entry.Value.name, new BoneData(entry.Value.transform.position, entry.Value.transform.rotation));
        }
        foreach(KeyValuePair<string, IDictionary<string, BoneData>> pose in allPoses){
            float del = compare(pose.Value, currentPose);
            if(del < minDel){
                minDel = del;
                poseName = pose.Key;
            }
        }
        createVisuals.ManualUpdate();
        createVisuals.ShowVisuals(poseName);
        return poseName;
    }
}
