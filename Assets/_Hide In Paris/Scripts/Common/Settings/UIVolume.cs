using UnityEngine;
using UnityEngine.UI;

namespace inkolorgames
{
    public class UIVolume : MonoSingleton<UIVolume>
    {
        [SerializeField]
        private Slider mainVolumeSlider, musicSlider, sfxSlider;

        private void Start()
        {
            mainVolumeSlider.value = VolumeManager.Instance.CurrentMainVolume;
            musicSlider.value = VolumeManager.Instance.CurrentMusicVolume;
            sfxSlider.value = VolumeManager.Instance.CurrentSFXVolume;
            SetVolumeMaster(VolumeManager.Instance.CurrentMainVolume);
            SetVolumeMusic(VolumeManager.Instance.CurrentMusicVolume);
            SetVolumeSFX(VolumeManager.Instance.CurrentSFXVolume);
        }

        public void SetVolumeMaster(float sliderValue) => VolumeManager.Instance.SetMainVolume(sliderValue);
        public void SetVolumeSFX(float sliderValue) => VolumeManager.Instance.SetSFXVolume(sliderValue);
        public void SetVolumeMusic(float sliderValue) => VolumeManager.Instance.SetMusicVolume(sliderValue);

    }
}