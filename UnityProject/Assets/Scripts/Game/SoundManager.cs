using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicAudioSource;
    public AudioSource soundAudioSource;

    [Header("Music Files")] 
    public AudioClip mainThemeAudioClip;
    public AudioClip winningThemeAudioClip;

    [Header("Audio Files")] 
    public AudioClip deathAudioClip;
    public AudioClip pickupAudioClip;
    public AudioClip radarAudioClip;
    public AudioClip gemPickupAudioClip;
    public AudioClip unplugAudioClip;
    public AudioClip plugAudioClip;
    public AudioClip socketActivationAudioClip;

    private Transform _robotTransform;

    private float _originalVolume;

    private float _pluginVolume;

    private void RegisterActions()
    {
        Toolbox.GetTool<RobotBatteryManager>().BatteryRunningLow += PlayDeathSound;
        Toolbox.GetTool<RobotBatteryManager>().BatteryIncreased += CancleDeathSound;
        Toolbox.GetTool<RobotPickupManager>().PickupPickedUp += PlayPickupSound;
        Toolbox.GetTool<RobotCableManager>().RobotUnplugged += PlayUnplugSound;
        Toolbox.GetTool<RobotCableManager>().RobotPlugged += CancleDeathSound;
        Toolbox.GetTool<RobotCableManager>().RobotPlugged += PlayPlugSound;
        Toolbox.GetTool<SocketFeedbackController>().SocketActivated += PlaySocketActivationSound;
        Toolbox.GetTool<WinCanvas>().WinningStarted += SwitchToWinningMusic;
        RobotRadar.RadarFireOut += PlayRadarSound;
    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<RobotBatteryManager>() != null)
        {
            Toolbox.GetTool<RobotBatteryManager>().BatteryRunningLow -= PlayDeathSound;
            Toolbox.GetTool<RobotBatteryManager>().BatteryIncreased -= CancleDeathSound;
        }
        if (Toolbox.GetTool<RobotPickupManager>() != null)
        {
            Toolbox.GetTool<RobotPickupManager>().PickupPickedUp -= PlayPickupSound;
        }
        if (Toolbox.GetTool<RobotCableManager>() != null)
        {
            Toolbox.GetTool<RobotCableManager>().RobotUnplugged -= PlayUnplugSound;
            Toolbox.GetTool<RobotCableManager>().RobotPlugged -= CancleDeathSound;
            Toolbox.GetTool<RobotCableManager>().RobotPlugged -= PlayPlugSound;
        }
        if (Toolbox.GetTool<SocketFeedbackController>() != null)
        {
            Toolbox.GetTool<SocketFeedbackController>().SocketActivated -= PlaySocketActivationSound;
        }
        if (Toolbox.GetTool<WinCanvas>() != null)
        {
            Toolbox.GetTool<WinCanvas>().WinningStarted -= SwitchToWinningMusic;
        }


        RobotRadar.RadarFireOut -= PlayRadarSound;
    }

    private void Start()
    {
        _originalVolume = soundAudioSource.volume;

        RegisterActions();

        _pluginVolume = 0;

        _robotTransform = Toolbox.GetTool<RobotController>().transform;

        musicAudioSource.clip = mainThemeAudioClip;
        musicAudioSource.Play();
    }

    private void SwitchToWinningMusic()
    {
        LeanTween.value(gameObject, 1, 0, 0.5f).setOnUpdate(OnMusicAudioSourceChange).setOnComplete(() =>
        {
            musicAudioSource.Stop();
            musicAudioSource.clip = winningThemeAudioClip;
            musicAudioSource.Play();
            LeanTween.value(gameObject, 0, 1, 0.5f).setOnUpdate(OnMusicAudioSourceChange);
        });
    }

    private void OnMusicAudioSourceChange(float volumeChange)
    {
        musicAudioSource.volume = volumeChange;
    }

    private void PlayDeathSound()
    {
        soundAudioSource.volume = 0.7f;
        soundAudioSource.clip = deathAudioClip;
        soundAudioSource.Play();
        //AudioSource.PlayClipAtPoint(deathAudioClip, _robotTransform.position);
    }

    private void PlayPickupSound(Pickup pickup)
    {
        if (pickup is RadarStation)
        {
            return;
        }

        if (pickup is GemPickup)
        {
            soundAudioSource.PlayOneShot(gemPickupAudioClip, 0.5f);
        }
        else
        {
            soundAudioSource.PlayOneShot(pickupAudioClip, _originalVolume);
        }
    }

    private void PlayRadarSound()
    {
        AudioSource.PlayClipAtPoint(radarAudioClip, _robotTransform.position, 0.7f);
    }

    private void PlayUnplugSound()
    {
        soundAudioSource.PlayOneShot(unplugAudioClip, _originalVolume);
    }

    private void PlayPlugSound()
    {
        soundAudioSource.PlayOneShot(plugAudioClip, _pluginVolume);

        _pluginVolume = _originalVolume;
    }

    private void PlaySocketActivationSound()
    {
        soundAudioSource.PlayOneShot(socketActivationAudioClip, 0.3f);
    }

    private void CancleDeathSound()
    {
        soundAudioSource.Stop();
        soundAudioSource.volume = _originalVolume;
    }
}
