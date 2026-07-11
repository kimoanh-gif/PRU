using UnityEngine;

public class ZombieAttackController : MonoBehaviour
{
    public enum EnemyClass { ZombieNhanh, ZombieGiap, BossChua }
    [Header("Phân loại Zombie Màn 2")]
    public EnemyClass loaiQuai;

    [Header("Cấu hình Chưởng Tím")]
    [SerializeField] private GameObject chuongTimPrefab;
    [SerializeField] private Transform attackPoint; // Điểm họng súng gắn trên tay Zombie

    // Hàm này được gọi từ Animation Event khi tay Zombie vung ra
    public void TriggerAnimationSpawn()
    {
        // 1. Luôn luôn lấy vị trí BÀN TAY hiện tại của Zombie làm điểm xuất phát
        // Dù Zombie có bị đẩy lùi hay đi tới, attackPoint.position vẫn sẽ đi theo tay nó
        Vector3 spawnPosition = attackPoint != null ? attackPoint.position : transform.position;

        if (chuongTimPrefab != null)
        {
            // 2. Sinh ra chưởng tím ngay tại vị trí tay Zombie vào ĐÚNG khoảnh khắc đó
            GameObject chuong = Instantiate(chuongTimPrefab, spawnPosition, Quaternion.identity);

            // 3. Xử lý kích thước nếu là Boss
            if (loaiQuai == EnemyClass.BossChua)
            {
                chuong.transform.localScale = new Vector3(2.5f, 2.5f, 1f);
            }

            Debug.Log($"🔥 Chưởng tím xuất phát từ đúng vị trí tay của {gameObject.name}!");
        }
    }
}