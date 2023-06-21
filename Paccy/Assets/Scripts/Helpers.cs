using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static void TryPlay(this AudioSource audioSource)
    {
        if (audioSource.isPlaying)
        {
            return;
        }
        audioSource.Play();
    }    
}
