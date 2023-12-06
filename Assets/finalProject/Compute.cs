using UnityEngine;
using UnityEngine.SceneManagement;
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
    public GameObject textOutput;
    public GameObject textInput; 
    // public GameObject textInput;
    public GameObject textMode;
    public HandDummy handDummy; 
    public OVRCameraRig ovrCameraRig;
    public Oculus.Interaction.Input.FromOVRHandDataSource fromovrhanddatasource; 
    public OVRVirtualKeyboard keyboard; 
    private GameObject targetModel; // Reference to the model you want to copy bone rotations to.
    int saveCount = 0; 
    string mode = ""; 
    float timer = 0.0f;
    bool loadedWord = true;
    string wordToTranslate = "SALSAS";
    int wordToTranslatei = 0;
    public GameObject saveButton;
    public GameObject compareButton;
    public GameObject translateButton;
    public GameObject trainButton;
    public GameObject resetButton;
    // references
    // https://www.youtube.com/watch?v=lBzwUKQ3tbw
    
    // Start is called before the first frame update
    void Start()
    {
        boneMap = handLinkScript.boneMap;
        targetModel = handLinkScript.targetModel;
        // generate text in create visuals 
        createVisuals.Start();
        keyboard.OnKeyboardHidden();
    }

    public void saveMode(){
        mode = "save";
    }
    public void compareMode(){
        mode = "compare";
    }
    public void translateMode(){
        mode = "translate";
    }
    public void trainMode(){
        mode = "train";
    }
    public void resetMode(){
        mode = "reset";
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

        // create two modes: one mode to save, one mode to recognize (also a clear button) 
        // when mode is equal to "", then the mode can change / at default menu 
        if(mode == ""){
            // TODO: add names / dialog showing modes 
            // enter save mode
            if(Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown("1")){
                mode = "save";
                // textInput.SetActive(true); 
                keyboard.OnKeyboardShown();
                // textInput.GetComponentInChildren<TMP_Text>().text = keyboard.TextCommitField.text;
            }
            // enter compare mode
            else if(Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown("2")){
                mode = "compare";
                keyboard.OnKeyboardHidden();
            }
            else if(Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown("3")){
                mode = "translate";
                keyboard.OnKeyboardHidden();
                timer = 0;
            }
            else if(Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown("4")){
                mode = "train";
                keyboard.OnKeyboardHidden();
                timer = 0;
                loadedWord = false;
                wordToTranslatei = 0;
            }
            else if(Input.GetKeyDown(KeyCode.Return)){
                mode = ""; 
                // reset scene without having to exit 
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else{
                textMode.GetComponentInChildren<TMP_Text>().text = "Press '1' to enter 'save' mode\nPress '2' to enter 'compare' mode\nPress '3' to enter 'translate' mode\nPress 'return' to reset the environment!";
                keyboard.OnKeyboardHidden();
            }
        }
        else if(mode == "save"){
            // save current pose 
            if(Input.GetKeyDown("space")){
                if(keyboard.TextCommitField.text.Length == 0){
                    save("");
                }
                else{
                    save(keyboard.TextCommitField.text); 
                }
            }
            else if(Input.GetKeyDown(KeyCode.Return)){
                mode = ""; 
            }
            else{
                textMode.GetComponentInChildren<TMP_Text>().text = "In mode 'save'\nPress 'space' to save pose!\nPress 'return' to return to the previous menu!";
            }
        }
        else if(mode == "compare"){
            // load all poses and compare 
            if(Input.GetKeyDown("space")){
                // get the closest pose 
                // Debug.Log(loadAll());
                loadAll();
                string poseFound = compareAll();
                // Debug.Log("FOUND POSE: " + poseFound);
                if(poseFound != "NULL"){
                    // display hand with pose
                    handDummy.displayHand(poseFound);
                    // update output text 
                    updateTextOutput(poseFound); 
                }
            }
            else if(Input.GetKeyDown(KeyCode.Return)){
                mode = ""; 
            }
            else{
                textMode.GetComponentInChildren<TMP_Text>().text = "In mode 'compare'\nPress 'space' to compare pose!\nPress 'return' to return to the previous menu!";
            }
        }
        else if(mode == "translate"){
            timer += Time.deltaTime;
            // for translate mode - only check every n milliseconds
            if(timer > 1.0) {
                timer = 0; 
                loadAll();
                string poseFound = compareAll();
                if(poseFound != "NULL"){
                    // display hand with pose
                    handDummy.displayHand(poseFound);
                    // update output text 
                    updateTextOutput(poseFound); 
                }
            }
            else if(Input.GetKeyDown(KeyCode.Return)){
                mode = ""; 
            }
            else{
                textMode.GetComponentInChildren<TMP_Text>().text = "In mode 'translate'\nPress 'return' to return to the previous menu!";
            }
        }
        else if(mode == "train"){
            timer += Time.deltaTime;
            if(loadedWord == false){
                loadAll();
                handDummy.displayHand(wordToTranslate[0].ToString());
                handDummy.displayHand(wordToTranslate[1].ToString());
                handDummy.displayHand(wordToTranslate[2].ToString());
                handDummy.displayHand(wordToTranslate[3].ToString());
                handDummy.displayHand(wordToTranslate[4].ToString());
                handDummy.displayHand(wordToTranslate[5].ToString());
                loadedWord = true;
            }
            // for translate mode - only check every n milliseconds
            if(timer > 2.0) {
                timer = 0; 
                // loadAll();
                string poseFound = compareAll();
                if(wordToTranslatei >= wordToTranslate.Length){
                    wordToTranslatei = 0;
                }
                if(poseFound == wordToTranslate[wordToTranslatei].ToString()){
                    // update output text 
                    updateTextOutput(poseFound); 
                    wordToTranslatei++;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Return)){
                mode = ""; 
            }
            else{
                textMode.GetComponentInChildren<TMP_Text>().text = "In mode 'train'\nPress 'return' to return to the previous menu!";
            }
        }
    }
    bool save(string fname){
        
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
        if(fname == ""){
            fname = "boneData"+saveCount.ToString();
        }
        if(allPoses.ContainsKey(fname)){
            allPoses[fname] = currentPose;
        }
        else{
            allPoses.Add(fname, currentPose);
        }
        string jsonData = JsonConvert.SerializeObject(currentPose, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        string path = @"C:\Users\ahnes\OneDrive\Documents\GitHub\18543_intro_xr\data\" + fname + ".json";
        // Debug.Log("PATH: " + path); 
        File.WriteAllText(path, jsonData);
        // Debug.Log("SAVED JSON\n"); 
        load(fname);
        createVisuals.ManualUpdate();
        createVisuals.ShowVisuals(fname);
        handDummy.displayHand(fname);
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
        float del = 0;
        foreach(KeyValuePair<string, BoneData> entry in onePose0)
        {
            float d;
            if(entry.Key == "handDummy"){
                // the root has a different name for the current pose (AllanHandScanRigged) than the dummyHand
                d = (float) Math.Pow(Vector3.Distance(entry.Value.localPosition, onePose1["AllanHandScanRigged"].localPosition), 2);
            }
            else{
                d = (float) Math.Pow(Vector3.Distance(entry.Value.localPosition, onePose1[entry.Key].localPosition), 2);
            }
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
        // calibrate the sensitivity of pose matching
        float minDel = 0.04f;
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
        Debug.Log("min del: " + minDel); 
        createVisuals.ManualUpdate();
        createVisuals.ShowVisuals(poseName);
        return poseName;
    }

    string updateTextOutput(string str){
        textOutput.GetComponentInChildren<TMP_Text>().text += str;
        textOutput.GetComponentInChildren<TMP_Text>().text += " " ; 
        return textOutput.GetComponentInChildren<TMP_Text>().text;
    }
}
