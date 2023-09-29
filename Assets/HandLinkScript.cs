using UnityEngine;

public class BoneRotationCopier : MonoBehaviour
{
    public GameObject sourceModel; // Reference to the model you want to copy bone rotations from.
    public GameObject targetModel; // Reference to the model you want to copy bone rotations to.


    void Update()
    {
        targetModel.transform.position = sourceModel.transform.position;
        targetModel.transform.rotation = sourceModel.transform.rotation;
        
        // Iterate through each bone in the source model.
        foreach (Transform sourceBone in sourceModel.GetComponentsInChildren<Transform>())
        {
            foreach (Transform targetBone in targetModel.GetComponentsInChildren<Transform>())
            {
                if(sourceBone.name == targetBone.name){
                    targetBone.transform.localRotation = sourceBone.transform.localRotation;
                }
            }
            
            // Find the corresponding bone in the target model based on the name.
            // Transform targetBone = sourceBone.transform.Find(sourceBone.name);
            // Debug.Log(sourceBone.name);
            // if (targetBone != null)
            // {
            //     Debug.Log("FOUND: " + sourceBone.name);
            //     // Copy the rotation from the source bone to the target bone.
            //     
            // }
        }
    }
}