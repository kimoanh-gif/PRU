using UnityEngine;

public class DanRongVaCham : MonoBehaviour
{
    [Header("Sát thương gây ra cho Rex")]
    public float satThuong = 10f;

    [Header("Thời gian tự hủy nếu bay trượt (giây)")]
    public float thoiGianTuHuy = 4f;

    void Start()
    {
        // Tự hủy sau 4 giây để tránh rác bộ nhớ
        Destroy(gameObject, thoiGianTuHuy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem vật bị đâm trúng có phải là Rex (có Tag là Player) hay không
        if (collision.CompareTag("Player"))
        {
            // Tìm đúng script "PlayerHealth" (viết liền) trên người Rex của nhóm bạn
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(satThuong); // Gọi hàm nhận sát thương của Rex
                Debug.Log("Rex bị dính đạn rồng! Trừ " + satThuong + " máu.");
            }

            // Hủy viên đạn ngay lập tức
            Destroy(gameObject);
        }
        // Nếu chạm vào đất (Ground) cũng tự biến mất cho đẹp
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}