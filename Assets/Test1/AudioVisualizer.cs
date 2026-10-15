using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public Transform[] bars;
    public ParticleSystem particleSystem;

    [Header("Settings")]
    public FrequencyFocusWindow frequencyFocusWindow;
    public float amplification = 1f;
    public float baseHeight = 0f;
    public FFTWindow fftWindow; // Determines how audio is smoothened
    public bool useDecibels;

    [Header("State")]
    public float[] spectrumData;

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

            // Update particle system emission rate based on the average amplitude of the bars
            if (particleSystem != null)
            {
                var emission = particleSystem.emission;
                emission.rateOverTime = Mathf.Clamp(sum * amplification * 100, 0, 1000); // Adjust the multiplier as needed for desired effect
            }
        }
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