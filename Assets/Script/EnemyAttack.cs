using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Cài đặt sát thương")]
    [SerializeField] private float damage = 20f;         // Cắn một phát mất 20 máu
    [SerializeField] private float attackCooldown = 1f;  // Phải chờ 1 giây mới được cắn phát tiếp theo

    private float cooldownTimer = 0f;

    void Update()
    {
        // Bộ đếm thời gian hồi chiêu
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    // 1. Kiểm tra va chạm vật lý cứng với Rex (Code cũ của bạn giữ nguyên)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (cooldownTimer <= 0)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    cooldownTimer = attackCooldown;
                }
            }
        }
    }

    // 2. THÊM VÀO: Kiểm tra va chạm dạng Trigger (Xuyên thấu) với Phòng Kính
    private void OnTriggerStay2D(Collider2D collision)
    {
        DrRoomManager drRoom = collision.GetComponent<DrRoomManager>();

        if (drRoom != null)
        {
            if (cooldownTimer <= 0)
            {
                // SỬA DÒNG NÀY: Thêm (int) vào trước biến damage để ép kiểu dữ liệu
                drRoom.TakeDamage((int)damage);

                cooldownTimer = attackCooldown;
                Debug.Log("Zombie đang cào Phòng Kính!");
            }
        }
    }
}