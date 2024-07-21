using UnityEngine;
using UnityEngine.UI;

public class TabButtonSound : MonoBehaviour
{
    public AudioClip tapSound; // タップ音のクリップ
    private AudioSource audioSource; // タップ音を再生するAudioSource

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSourceコンポーネントを追加
        audioSource.clip = tapSound;
    }

    public void PlayTapSound()
    {
        if (tapSound != null)
        {
            audioSource.PlayOneShot(tapSound);
        }
    }
}
