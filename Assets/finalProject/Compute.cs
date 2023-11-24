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
    public HandDummy handDummy; 
    public OVRCameraRig ovrCameraRig;
    public Oculus.Interaction.Input.FromOVRHandDataSource fromovrhanddatasource; 
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
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            ovrCameraRig.manualOffset += new Vector3((float)0.0, (float)-0.1, (float)0.0);
            fromovrhanddatasource.manualOffset += new Vector3((float)0.0, (float)-0.1, (float)0.0);
            handDummy.manualOffset += new Vector3((float)0.0, (float)-0.1, (float)0.0);
        }
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            ovrCameraRig.manualOffset += new Vector3((float)0.0, (float)0.1, (float)0.0);
            fromovrhanddatasource.manualOffset += new Vector3((float)0.0, (float)0.1, (float)0.0);
            handDummy.manualOffset += new Vector3((float)0.0, (float)0.1, (float)0.0);
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            ovrCameraRig.manualOffset += new Vector3((float)0.1, (float)0.0, (float)0.0);
            fromovrhanddatasource.manualOffset += new Vector3((float)0.1, (float)0.0, (float)0.0);
            handDummy.manualOffset += new Vector3((float)0.1, (float)0.0, (float)0.0);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            ovrCameraRig.manualOffset += new Vector3((float)-0.1, (float)0.0, (float)0.0);
            fromovrhanddatasource.manualOffset += new Vector3((float)-0.1, (float)0.0, (float)0.0);
            handDummy.manualOffset += new Vector3((float)-0.1, (float)0.0, (float)0.0);
        }

        // save pose
        if(Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown("1")){
            save();
        }

        // load all poses and compare 
        if(Input.GetKeyDown("c") || Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown("2")){
            // get the closest pose 
            Debug.Log(loadAll());
            string poseFound = compareAll();
            Debug.Log("FOUND POSE: " + poseFound);
            if(poseFound != "NULL"){
                handDummy.displayHand(poseFound);
            }
        }
    }
    bool save(){
        string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\boneData" + saveCount + ".json";
        currentPose.Clear(); 

        // find hand root 
        GameObject root = new GameObject(); 
        // get the wrist bone 
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            if((entry.Value.name == "handDummy")||(entry.Value.name == "AllanHandScanRigged")){
                // Debug.Log("Found root: " + entry.Value.name);
                root.transform.position = entry.Value.position;
                root.transform.rotation = entry.Value.rotation;
                root.transform.localPosition = entry.Value.position;
                root.transform.localRotation = entry.Value.rotation;
                break; 
            }
        }

        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            currentPose.Add(entry.Value.name, new BoneData(root.transform.InverseTransformPoint(entry.Value.position), entry.Value.transform.position, entry.Value.transform.rotation));
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
        // Debug.Log("FNAME load: " + fname);
        return true;
    }
    float compare(IDictionary<string, BoneData> onePose0, IDictionary<string, BoneData> onePose1){
        // good reference: https://www.youtube.com/watch?v=lBzwUKQ3tbw
        
        float del = 0;
        foreach(KeyValuePair<string, BoneData> entry in onePose0)
        {
            float d = (float) Math.Pow(Vector3.Distance(entry.Value.localPosition, onePose1[entry.Key].localPosition), 2);
            del += d; 
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
            // Debug.Log("FNAME: " + fname);
        }
        return true;
    }

    string compareAll(){
        float minDel = 999999999999.0f;
        string poseName = "NULL";
        // grab current pose to compare against
        GameObject root = new GameObject(); 
        // get the wrist bone 
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            if((entry.Value.name == "handDummy")||(entry.Value.name == "AllanHandScanRigged")){
                // Debug.Log("Found root: " + entry.Value.name);
                root.transform.position = entry.Value.position;
                root.transform.rotation = entry.Value.rotation;
                root.transform.localPosition = entry.Value.position;
                root.transform.localRotation = entry.Value.rotation;
                break; 
            }
        }
        currentPose.Clear(); 
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            currentPose.Add(entry.Value.name, new BoneData(root.transform.InverseTransformPoint(entry.Value.position), entry.Value.transform.position, entry.Value.transform.rotation));
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
