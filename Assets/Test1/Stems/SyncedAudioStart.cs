using UnityEngine;

public class SyncedAudioStart : MonoBehaviour
{
    [SerializeField] private AudioSource[] syncedAudioSources;
    [SerializeField] private float synchronizedLoopStartDelay = .1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartSynchronizedLoops();
    }
    private void StartSynchronizedLoops()
    {
        foreach (AudioSource source in syncedAudioSources)
        {
            double startDspTime = AudioSettings.dspTime + synchronizedLoopStartDelay;
            source.Stop();
            source.PlayScheduled(startDspTime);
        }
    }
}
