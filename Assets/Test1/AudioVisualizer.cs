using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public Transform[] bars;
    public ParticleSystem[] particleSystems;

    [Header("Settings")]
    public FrequencyFocusWindow frequencyFocusWindow;
    public float amplification = 1f;
    public float baseHeight = 0f;
    public FFTWindow fftWindow; // Determines how audio is smoothened
    public bool useDecibels;

    [Header("Particle Burst")]
    // These values control particle bursts independently from the bar amplification.
    public float particleBurstThreshold = 0.01f;
    public float particleBurstSensitivity = 1.5f;
    public float particleBurstCooldown = 0.1f;
    public int particleBurstCount = 20;

    [Header("State")]
    public float[] spectrumData;
    private float previousAudioLevel;
    private float nextParticleBurstTime;

    void Awake()
    {
        // Must be a power of 2 number, between 64 and 8192. 4096 is a good default.
        spectrumData = new float[4096];
    }

    void Update()
    {
        audioSource.GetSpectrumData(spectrumData, 0, fftWindow); //grab frequency spectrum of current audio and store in spectrumData array. 0 is channel, fftWindow is the smoothing method
        var blockSize = spectrumData.Length / bars.Length / (int)frequencyFocusWindow; //how many frequency values from array does each bar represent
        for(int i = 0; i < bars.Length; i++)
        {
            float sum = 0f;
            for(int j = 0; j < blockSize; j++)
            {
                sum += spectrumData[i * blockSize + j];
            }
            sum /= blockSize;
            float amplitude = Mathf.Clamp(sum, 1e-7f, 1f); // Clamp to avoid log10(0) when using decibels
            var scale = bars[i].localScale;
            if (useDecibels)
            {
                // Convert amplitude to decibels
                scale.y = -Mathf.Log10(amplitude) * amplification / 200; // Divide by 200 to scale down the decibel value for visualization. Makes smaller changes more noticeable.
            }
            else
            {
                scale.y = sum * amplification + baseHeight;
            }
            bars[i].localScale = scale;
        }

        // Check the raw spectrum data for a new audio peak. This does not use amplification.
        UpdateParticleBurst(blockSize);
    }

    private void UpdateParticleBurst(int blockSize)
    {
        if (particleSystems == null || particleSystems.Length == 0)
        {
            return;
        }

        // Average the focused spectrum bins to get one audio level for the burst detector.
        float audioLevel = 0f;
        int focusedBinCount = blockSize * bars.Length;
        for (int i = 0; i < focusedBinCount; i++)
        {
            audioLevel += spectrumData[i];
        }
        audioLevel /= focusedBinCount;

        // A burst requires both a minimum level and a rise from the previous frame.
        bool isAudioPeak = audioLevel >= particleBurstThreshold &&
                           audioLevel >= previousAudioLevel * particleBurstSensitivity;
        if (isAudioPeak && Time.time >= nextParticleBurstTime)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Emit(particleBurstCount);
            }
            nextParticleBurstTime = Time.time + particleBurstCooldown;
        }

        previousAudioLevel = audioLevel;
    }
}

public enum FrequencyFocusWindow
{
    Entire = 1,
    FirstHalf = 2,
    FirstQuarter = 4,
    FirstEight = 8,
    FirstSixteenth = 16
}