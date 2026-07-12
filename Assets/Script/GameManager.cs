using System.Collections; // CỰC KỲ QUAN TRỌNG: Thư viện bắt buộc phải có để chạy Coroutine (IEnumerator)
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(AudioSource))] // Tự động yêu cầu AudioSource làm loa phát bộ đàm
public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Giữ nguyên instance (viết thường) của bạn để không lỗi code gọi từ nơi khác

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
    // 🎬 PHẦN THÊM MỚI: CẤU HÌNH CHO CHUỖI ĐỐI THOẠI MỞ ĐẦU KỊCH TÍNH
    // =========================================================================
    [Header("🎬 CHUỖI ĐỐI THOẠI MỞ ĐẦU MÀN 1")]
    [Tooltip("Kéo file âm thanh tiếng Tiến sĩ gọi bộ đàm (ví dụ: DR_cứu tôi với) vào đây")]
    [SerializeField] private AudioClip drIntroRadioClip;
    [Tooltip("Kéo đối tượng Rex (đã gắn script RexDialogue) từ Hierarchy vào đây")]
    [SerializeField] private RexDialogue rexDialogue;

    private AudioSource audioSource; // Chiếc "loa" 2D tái tạo tiếng bộ đàm
    // =========================================================================

    private int zombiesKilledCount = 0;
    private bool chestSpawned = false;
    private float timer = 0f;
    private bool isVictory = false;

    void Awake()
    {
        instance = this;

        // Tự động tìm hoặc thêm AudioSource lên GameManager làm bộ đàm
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Đặt Spatial Blend = 0f (2D Sound) để tiếng bộ đàm Tiến sĩ nghe rõ đều cả hai tai 
        audioSource.spatialBlend = 0f;
    }

    void Start()
    {
        if (nutQuaManButton != null)
        {
            nutQuaManButton.SetActive(false);
        }

        // Kích hoạt chuỗi hội thoại mở màn ngay khi game vừa khởi chạy
        StartCoroutine(PlayOpeningDialogueSequence());
    }

    void Update()
    {
        if (!isVictory)
        {
            timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Chuỗi thời gian: Tiến sĩ kêu cứu -> Chờ dứt câu -> Rex nạp đạn đáp lại quyết tâm
    /// </summary>
    private IEnumerator PlayOpeningDialogueSequence()
    {
        // Chờ 0.5 giây đầu game để màn hình chuyển động ổn định
        yield return new WaitForSeconds(0.5f);

        // 1. Tiến sĩ Elias kêu cứu qua bộ đàm
        if (drIntroRadioClip != null && audioSource != null)
        {
            Debug.Log("📻 Bộ đàm phát: " + drIntroRadioClip.name);
            audioSource.PlayOneShot(drIntroRadioClip);

            // Chờ đúng bằng độ dài của file âm thanh cộng thêm 0.3 giây nghỉ thở
            yield return new WaitForSeconds(drIntroRadioClip.length + 0.3f);
        }

        // 2. Rex đáp lời (gọi hàm phát tiếng từ script RexDialogue gắn trên Rex)
        if (rexDialogue != null)
        {
            rexDialogue.PlayStartVoice();
        }
        else
        {
            Debug.LogWarning("⚠️ Bạn chưa kéo đối tượng Rex vào ô 'Rex Dialogue' trên GameManager!");
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

        // ĐÃ SỬA: Thay số 3 bằng biến linh hoạt tự điền tay trên Inspector
        if (zombiesKilledCount >= quaiChetDeRaRuong && !chestSpawned)
        {
            chestSpawned = true;
            SpawnRewardChest();
        }

        // KHI DIỆT HẾT SẠCH ZOMBIE
        if (totalZombies <= 0 && !isVictory)
        {
            isVictory = true;
            HienNutQuaManNho();
        }
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