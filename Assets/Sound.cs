using UnityEngine;
using UnityEngine.UI;

public class PlaySoundOnButtonClick : MonoBehaviour
{
    public AudioClip audioClip; // 再生するオーディオクリップをインスペクターで指定
    private AudioSource audioSource; // オーディオソース

    void Start()
    {
        // AudioSourceをこのオブジェクトに追加
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;

        // ボタンのクリックイベントにリスナーを追加
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        if (audioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}
