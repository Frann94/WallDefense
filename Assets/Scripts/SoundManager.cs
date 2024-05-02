using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Button muteButton;
    private bool _isMuted = false;
    public Color mutedColor;
    private Color normalColor;

    void Start()
    {
        normalColor = muteButton.image.color;
        AudioListener.volume = 0.25f;
        volumeSlider.value = AudioListener.volume;

        volumeSlider.onValueChanged.AddListener(SetVolume);
        muteButton.onClick.AddListener(ToggleMute);
    }

    void ToggleMute()
    {
        if (!_isMuted)
        {
            AudioListener.volume = 0f;
            muteButton.image.color = mutedColor;
        }
        else
        {
            AudioListener.volume = volumeSlider.value;
            muteButton.image.color = normalColor;
        }
        _isMuted = !_isMuted;
    }

    void SetVolume(float volume)
    {
        if (!_isMuted)
        {
            AudioListener.volume = volume;
        }
    }
}
