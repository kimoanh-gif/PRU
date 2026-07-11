using UnityEngine;

public class ZombieMeleeProjectiles : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damageToPlayer = 15;
    [SerializeField] private int damageToRoom = 20;

    private Vector2 moveDirection = Vector2.left;

    void Update()
    {
        // Chưởng tự tịnh tiến sang trái sau khi rời tay Zombie
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // LỆNH QUAN TRỌNG: Nếu chưởng tím va chạm với một viên chưởng tím khác,
        // lập tức bỏ qua, cho chúng xuyên qua nhau mượt mà, TUYỆT ĐỐI KHÔNG ỦN MÔNG.
        if (collision.CompareTag("ChuongTim") || collision.GetComponent<ZombieMeleeProjectiles>() != null)
        {
            return;
        }

        // 1. Nếu trúng người chơi Rex
        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                // player.TakeDamage(damageToPlayer); // Bỏ comment để trừ máu Rex
                Debug.Log($"Rex bị trúng chưởng tím! Mất {damageToPlayer} HP.");
            }
            Destroy(gameObject); // Nổ và biến mất
        }

        // 2. Nếu trúng phòng kính Dr. Elias
        if (collision.CompareTag("DrRoom"))
        {
            DrRoomManager room = collision.GetComponent<DrRoomManager>();
            if (room != null)
            {
                room.TakeDamage(damageToRoom); // Trừ máu phòng kính
                Debug.Log($"Phòng Dr bị trúng chưởng tím! Mất {damageToRoom} HP.");
            }
            Destroy(gameObject); // Nổ và biến mất
        }
    }
}