using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class breakCells : MonoBehaviour
{
    // https://www.youtube.com/watch?v=NGixJd79mcE
    public GameObject fractured; 
    public GameObject root; 
    public float breakForce; 
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("f") || Input.GetKey("f")){
            // gameObject.SetActive(true);
            BreakThing();
        }
    }

    public void BreakThing(){
        System.Random rnd = new System.Random();
        
        Debug.Log("Called Break Thing");
        GameObject frac = Instantiate(fractured, root.transform.position, root.transform.rotation);
        // frac.transform.localPosition = transform.localPosition;
        // frac.transform.localRotation = transform.localRotation;
        foreach(Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>()){
            // int r = rnd.Next(2);
            // if(r == 1){
            Vector3 force = (rb.transform.position - root.transform.position).normalized * breakForce; 
            rb.AddForce(force); 
            // }
        }
        // root.GetComponent<Renderer>().enabled = false;
        // Destroy(gameObject);
        // gameObject.SetActive(false);
    }
}
