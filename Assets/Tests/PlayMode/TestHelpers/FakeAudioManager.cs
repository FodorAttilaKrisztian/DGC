using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class FakeAudioManager : AudioManager
{
    public override void PlaySFX(AudioClip clip, float volume = 1f)
    {
        Debug.Log($"FakeAudioManager.PlaySFX called with clip {clip?.name}");
    }
}