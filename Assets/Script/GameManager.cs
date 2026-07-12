using System.Collections; // Bắt buộc phải có để chạy Coroutine
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("CÀI ĐẶT KIỂU MÀN CHƠI (QUAN TRỌNG)")]
    [Tooltip("Tích chọn nếu màn này có sự kiện Boss Ambush (Màn 2). Bỏ tích nếu là màn bắn quái thường qua màn luôn (Màn 1).")]
    [SerializeField] private bool coSuKienBoss = false;

    [Header("Cài đặt Nút Qua Màn Nhỏ")]
    [SerializeField] private GameObject nutQuaManButton;

    [Header("Cài đặt giao diện chiến thắng cũ")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private string nextSceneName;

    [Header("Cài đặt Rương Thưởng Màn")]
    [SerializeField] private GameObject ruongPrefab;
    [SerializeField] private Transform viTriRoiRuong;

    [Header("CẤU HÌNH SỐ QUÁI CHẾT ĐỂ RA GƯƠNG (MỚI)")]
    [Tooltip("Màn 1 muốn giết 1 con ra rương thì điền số 1 vào đây. Màn 2 điền số 3.")]
    [SerializeField] private int quaiChetDeRaRuong = 3;

    [Header("Cấu hình tổng số lượng quái trong màn")]
    [SerializeField] private int totalZombies = 6;

    // =========================================================================
    // 🎬 PHẦN ĐỐI THOẠI MỞ ĐẦU KỊCH TÍNH
    // =========================================================================
    [Header("🎬 CHUỖI ĐỐI THOẠI MỞ ĐẦU MÀN")]
    [Tooltip("Kéo file âm thanh tiếng Tiến sĩ gọi bộ đàm vào đây (Chỉ dùng nếu có hội thoại)")]
    [SerializeField] private AudioClip drIntroRadioClip;
    [Tooltip("Kéo đối tượng Rex (đã gắn script RexDialogue) vào đây (Chỉ dùng nếu có hội thoại)")]
    [SerializeField] private RexDialogue rexDialogue;

    // =========================================================================
    // 🌋 SỰ KIỆN BOSS TRÙM CUỐI AMBUSH (CHỈ DÙNG CHO MÀN CÓ BOSS)
    // =========================================================================
    [Header("🌋 CẤU HÌNH SỰ KIỆN BOSS TRÙM CUỐI")]
    [Tooltip("Kéo Prefab con Boss quái vật bay vào đây")]
    [SerializeField] private GameObject bossPrefab;
    [Tooltip("Kéo một Object rỗng làm vị trí xuất hiện của Boss trên cao")]
    [SerializeField] private Transform bossSpawnPoint;
    [Tooltip("Tiếng gầm rú cực lớn của quái vật khi xuất hiện")]
    [SerializeField] private AudioClip bossRoarSound;
    [Tooltip("Giọng thoại đe dọa của Boss")]
    [SerializeField] private AudioClip bossDialogueVoice;
    [Tooltip("Giọng thoại phản hồi của Rex")]
    [SerializeField] private AudioClip rexBossReplyVoice;

    private AudioSource audioSource; // "Loa" phát tiếng bộ đàm, tiếng gầm và giọng Boss
    private AudioSource rexAudioSource; // "Loa" của Rex để phát giọng Rex đáp trả Boss

    private int zombiesKilledCount = 0;
    private bool chestSpawned = false;
    private float timer = 0f;
    private bool isVictory = false;

    // Các biến trạng thái kiểm tra điều kiện xuất hiện Boss
    private bool drRescued = false;
    private bool normalZombiesCleared = false;
    private bool bossEventTriggered = false;
    private bool bossDefeated = false;

    void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f; // Âm thanh 2D nghe rõ đều 2 tai
    }

    void Start()
    {
        if (nutQuaManButton != null)
        {
            nutQuaManButton.SetActive(false);
        }

        // Tìm AudioSource trên người Rex để chuẩn bị thoại
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Rex");
        if (playerObj != null)
        {
            rexAudioSource = playerObj.GetComponent<AudioSource>();
            if (rexAudioSource == null)
            {
                rexAudioSource = playerObj.AddComponent<AudioSource>();
            }
        }

        // Kích hoạt chuỗi hội thoại mở màn (Nếu được gán đầy đủ)
        if (drIntroRadioClip != null || rexDialogue != null)
        {
            StartCoroutine(PlayOpeningDialogueSequence());
        }
    }

    void Update()
    {
        if (!isVictory)
        {
            timer += Time.deltaTime;
        }
    }

    private IEnumerator PlayOpeningDialogueSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (drIntroRadioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(drIntroRadioClip);
            yield return new WaitForSeconds(drIntroRadioClip.length + 0.3f);
        }

        if (rexDialogue != null)
        {
            rexDialogue.PlayStartVoice();
        }
    }

    public void ZombieKilled()
    {
        zombiesKilledCount++;

        if (totalZombies > 0)
        {
            totalZombies--;
        }

        Debug.Log($"[BÁO CÁO]: Quái chết! Đã giết: {zombiesKilledCount} con. Còn lại: {totalZombies} con.");

        if (zombiesKilledCount >= quaiChetDeRaRuong && !chestSpawned)
        {
            chestSpawned = true;
            SpawnRewardChest();
        }

        // Khi diệt sạch quái thường trong màn
        if (totalZombies <= 0 && !normalZombiesCleared)
        {
            normalZombiesCleared = true;

            // NẾU LÀ MÀN THƯỜNG (MÀN 1) -> KHÔNG CÓ BOSS: Cho qua màn luôn!
            if (!coSuKienBoss)
            {
                isVictory = true;
                HienNutQuaManNho();
                Debug.Log("🏆 Màn chơi hoàn thành! Đã xuất hiện nút qua màn.");
            }
            else
            {
                Debug.Log("🛡️ Đã diệt sạch quái thường! Đang đợi cứu Tiến sĩ và đi ra cổng để kích hoạt Boss...");
            }
        }
    }

    /// <summary>
    /// Gọi từ script DrRescue khi lồng kính vỡ thành công
    /// </summary>
    public void SetDrRescued()
    {
        drRescued = true;
        Debug.Log("👨‍🔬 Tiến sĩ Elias đã được giải cứu tự do!");
    }

    /// <summary>
    /// Kiểm tra xem Rex và Tiến sĩ đã đủ điều kiện để kích hoạt sự kiện Boss chưa
    /// </summary>
    public bool IsReadyForBossAmbush()
    {
        // Phải là màn có Boss, đã diệt hết quái thường và đã cứu được Tiến sĩ
        return coSuKienBoss && normalZombiesCleared && drRescued && !bossEventTriggered;
    }

    /// <summary>
    /// Kích hoạt chuỗi sự kiện Trùm Cuối Xuất Hiện khi người chơi chạm vùng Cổng Thoát
    /// </summary>
    public void TriggerBossAmbush()
    {
        if (bossEventTriggered) return;
        bossEventTriggered = true;

        StartCoroutine(PlayBossAmbushSequence());
    }

    /// <summary>
    /// Chuỗi điện ảnh: Đất rung chuyển -> Boss gầm rú xuất hiện -> Đối thoại -> Đánh nhau
    /// </summary>
    private IEnumerator PlayBossAmbushSequence()
    {
        Debug.Log("🎬 BẮT ĐẦU SỰ KIỆN TRÙM CUỐI XUẤT HIỆN!");

        // 1. Phát tiếng gầm rú cực lớn của quái vật bay
        if (bossRoarSound != null)
        {
            audioSource.PlayOneShot(bossRoarSound, 1f);
        }

        yield return new WaitForSeconds(1.5f);

        // 2. Triệu hồi (Spawn) con Boss quái vật bay tại vị trí trên cao
        GameObject bossInstance = null;
        BossFlyingAI bossAI = null;

        if (bossPrefab != null && bossSpawnPoint != null)
        {
            bossInstance = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
            bossAI = bossInstance.GetComponent<BossFlyingAI>();
            if (bossAI != null)
            {
                bossAI.InitializeBoss(false); // Vô hiệu hóa AI tấn công tạm thời trong lúc thoại
            }
        }

        yield return new WaitForSeconds(1.0f);

        // 3. Boss cất giọng thoại đe dọa
        if (bossDialogueVoice != null && audioSource != null)
        {
            Debug.Log("🗣️ BOSS: Rex... ngươi không thoát được đâu!");
            audioSource.PlayOneShot(bossDialogueVoice, 0.9f);
            yield return new WaitForSeconds(bossDialogueVoice.length + 0.5f);
        }

        // 4. Rex cất giọng thoại đáp trả dũng cảm
        if (rexBossReplyVoice != null && rexAudioSource != null)
        {
            Debug.Log("🗣️ Rex: Lại thêm một thực thể đột biến nữa sao? Tiến sĩ, hãy lùi lại sau!");
            rexAudioSource.PlayOneShot(rexBossReplyVoice, 0.9f);
            yield return new WaitForSeconds(rexBossReplyVoice.length + 0.3f);
        }

        // 5. Chính thức cho phép Boss bắt đầu lao vào tấn công Rex
        if (bossAI != null)
        {
            bossAI.InitializeBoss(true); // Kích hoạt AI chiến đấu!
            Debug.Log("⚔️ CHIẾN ĐẤU BẮT ĐẦU! DIỆT BOSS ĐỂ THOÁT THÂN!");
        }
    }

    /// <summary>
    /// Gọi từ script Boss khi Boss bị tiêu diệt
    /// </summary>
    public void BossKilled()
    {
        if (bossDefeated) return;
        bossDefeated = true;
        isVictory = true;

        Debug.Log("🏆 Boss Trùm Cuối đã bị tiêu diệt hoàn toàn!");
        HienNutQuaManNho();
    }

    void SpawnRewardChest()
    {
        if (ruongPrefab != null && viTriRoiRuong != null)
        {
            Instantiate(ruongPrefab, viTriRoiRuong.position, Quaternion.identity);
            Debug.Log("🎉 Rương báu đã xuất hiện thành công!");
        }
    }

    void HienNutQuaManNho()
    {
        if (nutQuaManButton == null)
        {
            nutQuaManButton = GameObject.Find("NutQuaMan");
        }

        if (nutQuaManButton != null)
        {
            nutQuaManButton.SetActive(true);
            Debug.Log("📢 ĐÃ HIỆN NÚT NEXT LEVEL THÀNH CÔNG!");
        }
    }

    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}