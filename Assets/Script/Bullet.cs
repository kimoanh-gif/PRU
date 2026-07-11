using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Thời gian tự hủy")]
    [SerializeField] private float lifeTime = 2f;

    void Start()
    {
        // Tự hủy sau 2 giây để tránh rác bộ nhớ game
        Destroy(gameObject, lifeTime);
    }

    // Vì đạn đã chỉnh thành Is Trigger nên dùng hàm OnTriggerEnter2D
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu đạn chạm vào Zombie
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(10f); // Trừ 10 máu của Zombie
            }

            Destroy(gameObject); // Xóa viên đạn ngay sau khi trúng đích
        }
    }
}