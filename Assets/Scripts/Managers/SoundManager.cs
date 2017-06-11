using UnityEngine;

public class SoundManager : JMBehaviour
{
    public static SoundManager Instance;
    public AudioSource MainAudioSource;
    public AudioClip[] BackgroundMusics;
    public AudioClip FailureSound;

    public override void DoStart()
    {
        Instance = this;
        base.DoStart();
    }

    public override void DoUpdate()
    {
        if (ConfigurationManager.BackgroundMusicOn && !MainAudioSource.isPlaying && GameManager.GameState != GameState.Over)
        {
            PlayBackgroundMusic();
        }
        base.DoUpdate();
    }

    protected void PlayBackgroundMusic()
    {
        int randomIndex = Random.Range(0, BackgroundMusics.Length);
        MainAudioSource.PlayOneShot(BackgroundMusics[randomIndex]);
    }

    public static void PlayFailureSound()
    {
        if (Instance.MainAudioSource.isPlaying)
        {
            Instance.MainAudioSource.Stop();
        }

        Instance.MainAudioSource.PlayOneShot(Instance.FailureSound);
    }
}