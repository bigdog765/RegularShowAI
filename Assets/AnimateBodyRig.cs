using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateBodyRig : MonoBehaviour
{
    private Animator animator;
    private APIListener listener;
    private bool isCoroutineRunning = false;
    private int prevTalker = -1;
    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();
        listener = FindObjectOfType<APIListener>();
    }

    // Update is called once per frame
    void Update()
    {
        if(listener.talker == 2)
        {
            //animator.SetTrigger("RigbyT");
            if (listener.audio_complete)
            {
                animator.ResetTrigger("RigbyT");
                listener.talker = -1;
            }
        }
        
    }
    IEnumerator WaitForSpeechToEnd(int time, Action callback)
    {
        yield return new WaitForSeconds(time);
        // Call the callback function
        callback.Invoke();
    }
}
