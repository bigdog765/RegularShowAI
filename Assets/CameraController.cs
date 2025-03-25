using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetMord; // The target position to move the camera to
    public Transform targetRigby; 
    public float smoothSpeed; // Adjust this value to control the smoothness of the movement
    private APIListener listener;
    // Start is called before the first frame update
    void Start()
    {
        listener = FindObjectOfType<APIListener>();
    }
    
    // Update is called once per frame
    void Update()
    {
        
            if (targetMord != null && targetRigby != null)
            {
                
                if (listener.talker == 1) {
                    // Use Vector3.Lerp to smoothly interpolate between the current position and the target position
                    transform.position = Vector3.Lerp(transform.position, targetMord.position, smoothSpeed * Time.deltaTime);
                    // Use Quaternion.Lerp to smoothly interpolate between the current rotation and a rotated one
                    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(-1, 0, 1), new Vector3(0, 1, 0));
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);

            }
                if (listener.talker == 2)
                {
                    // Use Vector3.Lerp to smoothly interpolate between the current position and the target position
                    transform.position = Vector3.Lerp(transform.position, targetRigby.position, smoothSpeed * Time.deltaTime);
                    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(-1, 0, -1), new Vector3(0, 1, 0));
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
                }
            }
        
        
    }
}
