using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Cài đặt máu")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [Header("Quà rơi ra khi chết (Thanh Gươm)")]
    [SerializeField] private GameObject swordPrefab;

    // CHIẾC KHÓA BẢO VỆ: Đảm bảo con quái này chỉ báo tử ĐÚNG 1 LẦN Duy Nhất!
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return; // Nếu đã chết rồi thì bỏ qua, không nhận sát thương nữa

        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " bị trúng đạn! Máu còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Khóa lại ngay lập tức!
        isDead = true;

        Debug.Log(gameObject.name + " đã thực sự chết và chỉ báo cáo 1 lần!");

        // Báo cho trọng tài biết có 1 con quái vừa chết
        if (GameManager.instance != null)
        {
            GameManager.instance.ZombieKilled();
        }

        if (swordPrefab != null)
        {
            Instantiate(swordPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}