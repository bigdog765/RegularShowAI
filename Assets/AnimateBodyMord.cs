using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateBodyMord : MonoBehaviour
{
    private Animator animator;
    private APIListener listener;
    private bool isCoroutineRunning = false;
    private int prevTalker = -1;

    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();
        listener = FindObjectOfType<APIListener>();
    }

    void Update()
    {
        if (listener.talker == 1)
        {
            //animator.SetTrigger("MordT");
            if (listener.audio_complete)
            {
                animator.ResetTrigger("MordT");
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
