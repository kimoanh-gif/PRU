using System.Collections;
using UnityEngine;

public class DrRescue : MonoBehaviour
{
    private bool isFree = false;
    private float originalScaleX;

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Transform player;

    [Header("Cấu hình di chuyển")]
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private float stopDistance = 0.8f;

    [Header("Cấu hình Âm Thanh (Kêu Cứu)")]
    [SerializeField] private AudioClip helpCrySound;
    [SerializeField] private float cryInterval = 4.0f;
    private AudioSource audioSource;
    private float nextCryTime = 0f;

    [Header("🎬 Chuỗi Đối Thoại Giải Cứu")]
    [Tooltip("Kéo file 'rex_rescue.mp3' vào đây")]
    [SerializeField] private AudioClip rexRescueVoice;
    [Tooltip("Kéo file 'dr_thanks.mp3' vào đây")]
    [SerializeField] private AudioClip drThanksVoice;

    private AudioSource rexAudioSource; // Loa của Rex để phát giọng Rex

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalScaleX = transform.localScale.x;

        // Tự động lấy AudioSource trên người Tiến sĩ
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // =========================================================================
        // 🔊 ĐÃ SỬA LỖI ÂM THANH BÉ: Chuyển hẳn sang 0f (2D) để nghe rõ ở mọi khoảng cách
        // =========================================================================
        audioSource.spatialBlend = 0f;

        // Tự động tìm Rex để lấy AudioSource của Rex phát giọng Rex
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Rex");

        if (playerObj != null)
        {
            player = playerObj.transform;
            rexAudioSource = playerObj.GetComponent<AudioSource>();
            if (rexAudioSource == null)
            {
                rexAudioSource = playerObj.AddComponent<AudioSource>();
            }
        }
    }

    void Update()
    {
        if (!isFree)
        {
            if (helpCrySound != null && Time.time >= nextCryTime)
            {
                audioSource.PlayOneShot(helpCrySound);
                nextCryTime = Time.time + cryInterval;
            }
        }
    }

    void LateUpdate()
    {
        if (isFree && player != null)
        {
            float distanceX = Mathf.Abs(transform.position.x - player.position.x);

            if (distanceX > stopDistance)
            {
                Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                float directionX = player.position.x - transform.position.x;
                if (directionX > 0.05f)
                {
                    transform.localScale = new Vector3(Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
                }
                else if (directionX < -0.05f)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
                }

                if (anim != null) anim.SetBool("IsRunning", true);
            }
            else
            {
                if (anim != null) anim.SetBool("IsRunning", false);
            }
        }
    }

    /// <summary>
    /// Gọi khi lồng kính vỡ
    /// </summary>
    public void SetFree()
    {
        if (isFree) return; // Tránh kích hoạt nhiều lần
        isFree = true;

        transform.SetParent(null);

        // 1. Tắt tiếng kêu cứu ngay lập tức
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 2. Chạy chuỗi hội thoại kịch tính
        StartCoroutine(PlayDialogueSequence());

        // 3. Tắt vật lý để chạy theo Rex mượt mà
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) Destroy(rb);

        Collider2D cl = GetComponent<Collider2D>();
        if (cl != null) Destroy(cl);

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 5;
        }
    }

    /// <summary>
    /// Chuỗi đối thoại: Rex nói trước -> Tiến sĩ trả lời -> Tiến sĩ bắt đầu chạy
    /// </summary>
    private IEnumerator PlayDialogueSequence()
    {
        // Phân đoạn 1: Rex lên tiếng (Phát âm thanh từ loa của Rex)
        if (rexRescueVoice != null && rexAudioSource != null)
        {
            Debug.Log("🗣️ Rex: Ổn rồi thưa Tiến sĩ! Kính đã vỡ...");
            rexAudioSource.PlayOneShot(rexRescueVoice);

            // Chờ cho đến khi Rex nói xong câu thoại của mình
            yield return new WaitForSeconds(rexRescueVoice.length + 0.5f);
        }

        // Phân đoạn 2: Tiến sĩ Elias đáp lại cảm ơn
        if (drThanksVoice != null && audioSource != null)
        {
            Debug.Log("🗣️ Tiến sĩ Elias: Cảm ơn cậu... Rex!");
            audioSource.PlayOneShot(drThanksVoice);

            // Chờ Tiến sĩ nói xong câu cảm ơn
            yield return new WaitForSeconds(drThanksVoice.length);
        }

        Debug.Log("🏃 Tiến sĩ bắt đầu bám đuôi Rex!");
    }
}