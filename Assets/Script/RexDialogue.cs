using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RexDialogue : MonoBehaviour
{
    [Header("🎬 Thoại Bắt Đầu Trận Đấu (Màn 1)")]
    [Tooltip("Kéo file thoại 'rex_start.mp3' của Rex vào đây")]
    [SerializeField] private AudioClip startCombatVoice;

    private AudioSource audioSource;

    void Awake()
    {
        // Tự động lấy AudioSource gắn trên người Rex
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Đặt Spatial Blend = 0.0f để nghe rõ đều 2 tai (như tiếng bộ đàm/nội tâm của nhân vật)
        audioSource.spatialBlend = 0.0f;
    }

    /// <summary>
    /// Phát giọng nói mở đầu trận chiến của Rex
    /// </summary>
    public void PlayStartVoice()
    {
        if (startCombatVoice != null && audioSource != null)
        {
            Debug.Log("🗣️ Rex: Nạp đạn xong... Lũ quái vật chết tiệt này đông quá...");
            audioSource.PlayOneShot(startCombatVoice);
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán file thoại startCombatVoice cho Rex hoặc thiếu AudioSource!");
        }
    }
}