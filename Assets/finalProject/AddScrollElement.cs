using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oculus;
using TMPro;

// https://forum.unity.com/threads/add-list-string-to-scrollview-content.613186/
public class AddScrollElement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError(gameObject);
        // Debug.Log("__found__: " + PrefabStage.IsPartOfPrefabContents(gameObject));
        // foreach (GameObject trans in gameObject.GetComponentsInChildren<GameObject>())
        foreach (Transform child in transform)
        {
            if(child){
                if(child.childCount == 3){
                    Debug.Log("__found__: " + child); 
                    TextMeshPro text = child.GetComponentInChildren<TextMeshPro>();
                    text.SetText("BLAHBLAH");
                    Debug.Log("__found__3__: " + text); 
                    // Debug.Log("__found__text__: " + child.Find("Text (TMP)")); 
                    // Debug.Log("__found__0__: " + child.childCount); 
                    // Debug.Log("__found__1__: " + child.GetChild(1));
                    // Debug.Log("__found__1__: " + child.GetChild(1));
                    // foreach (Transform visual in child.GetChild(1).transform){
                    // Debug.Log("__found__1__: " + child.GetChild(1).GetChild(0).GetChild(1));
                    // Button myButton = child.GetChild(1).GetChild(0).GetChild(1);
                    
                    // Transform newButton = GameObject.Instantiate(child);
                    // newButton.position = child.position + new Vector3(0.1f,0.1f,0.1f);
                    // newButton.rotation = child.rotation;
                    // }
                }
            }
            // gameObject.Add(b)
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
