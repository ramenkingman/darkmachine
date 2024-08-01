using UnityEngine;
using UnityEngine.UI;

public class PlaySoundOnButtonClick : MonoBehaviour
{
    public AudioClip audioClip; // 再生するオーディオクリップをインスペクターで指定
    private static AudioSource audioSource; // オーディオソース

    void Start()
    {
        if (audioSource == null)
        {
            // AudioSourceがまだ存在しない場合、このオブジェクトに追加
            GameObject audioSourceObject = new GameObject("GlobalAudioSource");
            audioSource = audioSourceObject.AddComponent<AudioSource>();
            DontDestroyOnLoad(audioSourceObject);
        }

        // オーディオクリップを設定
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
