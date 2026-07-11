using UnityEngine;
using TMPro;

public class DrRoomManager : MonoBehaviour
{
    [Header("Cấu hình Máu Phòng Kính")]
    public int maxHP = 150;
    private int currentHP;

    [Header("Tiến sĩ (DR) ")]
    public DrRescue drScript;

    [Header("Giao diện UI")]
    public TextMeshProUGUI hpText;

    private bool isDestroyed = false;
    private bool allZombiesDead = false; // Biến kiểm tra xem đã diệt sạch Zombie chưa

    void Start()
    {
        currentHP = maxHP;
        UpdateUI();
    }

    void Update()
    {
        // Nếu phòng kính đã vỡ hoặc đã xác nhận diệt sạch quái thì không cần quét lại nữa
        if (isDestroyed || allZombiesDead) return;

        // Quét bản đồ để tìm tất cả các Object có Tag là "Enemy" (Lũ Zombie)
        GameObject[] remainingZombies = GameObject.FindGameObjectsWithTag("Enemy");

        // Khi số lượng Zombie trên bàn đồ bằng 0
        if (remainingZombies.Length == 0)
        {
            allZombiesDead = true;
            if (hpText != null)
            {
                hpText.text = "HỆ THỐNG MỞ! HÃY CHẠY ĐẾN CỨU DR!";
            }
        }
    }

    // 1. Zombie cào lồng kính gây mất máu
    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        currentHP -= damage;
        UpdateUI();

        // THUA CUỘC: Nếu Rex bảo vệ thất bại, để Zombie cào vỡ kính trước khi kịp giết hết quái
        if (currentHP <= 0)
        {
            isDestroyed = true;
            if (hpText != null)
            {
                hpText.text = "BẢO VỆ THẤT BẠI! GAME OVER!";
            }
            Debug.Log("💀 Phòng kính đã bị phá hủy hoàn toàn!");

            // Lệnh chơi lại màn này nếu bạn muốn (bỏ dấu // ở dưới để dùng):
            // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    void UpdateUI()
    {
        if (hpText != null && !isDestroyed && !allZombiesDead)
        {
            hpText.text = $"PHÒNG DR. ELIAS\nHP: {currentHP}/{maxHP}\n(Tiêu diệt hết Zombie!)";
        }
    }

    // 2. Rex chạy lại gần chạm vào phòng kính để giải cứu
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ĐIỀU KIỆN QUYẾT ĐỊNH: Kính chưa vỡ VÀ người chạm là Rex VÀ PHẢI DIỆT HẾT ZOMBIE
        if (!isDestroyed && collision.CompareTag("Player") && allZombiesDead)
        {
            isDestroyed = true;

            // Ra lệnh cho ông Tiến sĩ tự động chạy ra ngoài
            if (drScript != null)
            {
                drScript.SetFree();
            }

            // Ẩn chữ giao diện UI đi vì đã hoàn thành màn chơi
            if (hpText != null)
            {
                hpText.gameObject.SetActive(false);
            }

            // Xóa phòng kính đi để lấy lối đi cho ông Dr chạy ra ngoài
            Destroy(gameObject);
        }
    }
}