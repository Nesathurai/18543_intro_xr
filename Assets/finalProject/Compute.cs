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
    IDictionary<string, BoneData> currentPose = new Dictionary<string, BoneData>();
    private GameObject targetModel; // Reference to the model you want to copy bone rotations to.
    int saveCount = 0; 
    public TextMeshPro text;
    // Start is called before the first frame update
    void Start()
    {
        boneMap = handLinkScript.boneMap;
        // text = GetComponentInChildren<TextMeshPro>();
        // text.text = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA!";
        // text.gameObject.SetActive(false);
        targetModel = handLinkScript.targetModel;
        // poseDisplay.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // int i = 0;
        // foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        // {
        //     if(i == 0){
        //         Debug.Log("__COMPUTE__ " + entry.Value.transform.rotation);
        //         // currentPose.Add(entry.Value.name, new BoneData(entry.Value.transform.position + new Vector3(0.1f,0.1f,0.1f), entry.Value.transform.rotation));
        //     }
        //     ++i;
        // }
        // save pose
        if(Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.Keypad1)){
            Debug.Log("STARTING SAVE");
            save();
            Debug.Log("ENDING SAVE");
        }
        // if(Input.GetKeyDown("9")){
        //     text.gameObject.SetActive(true);
        // }
        // load pose
        if(Input.GetKeyDown("l") || Input.GetKeyDown(KeyCode.Keypad2)){
            Debug.Log("STARTING LOAD");
            load();
            Debug.Log("ENDING LOAD");
        }
        
        // compare pose
        if(Input.GetKeyDown("c") || Input.GetKeyDown(KeyCode.Keypad3)){
            // get the closest pose 
            Debug.Log("STARTING LOAD");
            Debug.Log(loadAll());
            Debug.Log("ENDING LOAD");
            Debug.Log("STARTING COMPAREALL");
            // _poseActiveVisuals[i].GetComponentInChildren<TextMeshPro>().text = _poses[i].name;
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
        string jsonData = JsonConvert.SerializeObject(currentPose, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        File.WriteAllText(path, jsonData);
        Debug.Log("SAVED JSON\n"); 
        saveCount++; 
        return true; 
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
                        targetBone.position = onePose[targetBone.name].position;
                        targetBone.rotation = onePose[targetBone.name].rotation;
                    }
                }
            }
            count++; 
        }
    }

    float compare(IDictionary<string, BoneData> onePose0, IDictionary<string, BoneData> onePose1){
        // returns true if bone differences less than some delta 
        float del = 0;
        foreach(KeyValuePair<string, BoneData> entry in onePose0)
        {
            del += (entry.Value.position - onePose1[entry.Key].position).magnitude;
            // to subtract quaternions must use inverse 
            del += (Quaternion.Inverse(entry.Value.rotation) * onePose1[entry.Key].rotation).eulerAngles.magnitude;
        }
        Debug.Log("DEL: " + del); 
        return del;
    }
    
    bool loadAll(){
        string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\";
        string[] files = Directory.GetFiles(path);
        Debug.Log("FILES: " + files);
        int count = 0; 
        foreach (string file in files) {
            Debug.Log("C: " + count);
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
        // generate text in create visuals 
        createVisuals.Start();
        
        return true;
    }

    string compareAll(){
        float minDel = 999999999999.0f;
        string poseName = "NULL";
        // grab current pose to compare against
        currentPose.Clear(); 
        foreach(KeyValuePair<Transform, Transform> entry in boneMap)
        {
            // currentPose.Add(entry.Value.name, new BoneData(entry.Value.transform.position + new Vector3(0.1f,0.1f,0.1f), entry.Value.transform.rotation));
            currentPose.Add(entry.Value.name, new BoneData(entry.Value.transform.position, entry.Value.transform.rotation));
        }
        foreach(KeyValuePair<string, IDictionary<string, BoneData>> pose in allPoses){
            float del = compare(pose.Value, currentPose);
            if(del < minDel){
                minDel = del;
                poseName = pose.Key;
            }
        }
        createVisuals.ShowVisuals(poseName);
        return poseName;
    }
}
