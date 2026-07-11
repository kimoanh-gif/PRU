using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

    private int zombiesKilledCount = 0;
    private bool chestSpawned = false;
    private float timer = 0f;
    private bool isVictory = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (nutQuaManButton != null)
        {
            nutQuaManButton.SetActive(false);
        }
    }

    void Update()
    {
        if (!isVictory)
        {
            timer += Time.deltaTime;
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