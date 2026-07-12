using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class BossFlyingAI : MonoBehaviour
{
    [Header("❤ Chỉ Số Máu Trùm")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("🏃 Chỉ Số Di Chuyển Bay")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float attackRange = 1.0f;

    [Header("🔊 Âm Thanh")]
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;

    private bool isCombatActive = false; // Chỉ tấn công khi kết thúc hội thoại điện ảnh
    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Cấu hình vật lý chuẩn cho quái bay
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        currentHealth = maxHealth;
    }

    void Start()
    {
        // Tự động định vị Rex để đuổi theo
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Rex");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    /// <summary>
    /// GameManager gọi hàm này để kích hoạt trạng thái tấn công sau khi đối thoại xong
    /// </summary>
    public void InitializeBoss(bool startCombat)
    {
        isCombatActive = startCombat;
    }

    void Update()
    {
        if (isDead || !isCombatActive || player == null) return;

        // Tính toán khoảng cách và hướng bay tới Rex
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            // Di chuyển bay đuổi theo Rex
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Xoay mặt quái vật hướng về phía Rex
            if (direction.x > 0.1f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x < -0.1f)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            if (anim != null) anim.SetBool("isMoving", true);
        }
        else
        {
            // Khi sát Rex: Thực hiện đòn tấn công
            if (anim != null) anim.SetBool("isMoving", false);
        }
    }

    /// <summary>
    /// Nhận sát thương khi bị Rex bắn trúng
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"💥 Boss nhận {damage} sát thương! Máu còn lại: {currentHealth}/{maxHealth}");

        if (hurtSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hurtSound, 0.8f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        isCombatActive = false;
        rb.linearVelocity = Vector2.zero;

        Debug.Log("💀 Boss đang chết...");

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound, 1.0f);
        }

        // Tải hoạt ảnh chết nếu có
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // Báo cáo về GameManager để hiện nút Next Level màu tím qua màn!
        if (GameManager.instance != null)
        {
            GameManager.instance.BossKilled();
        }

        // Biến mất sau khi hoạt ảnh và âm thanh chết chạy xong (ví dụ sau 1.5 giây)
        Destroy(gameObject, 1.5f);
    }

    // Nhận diện va chạm vật lý nếu Rex bắn đạn
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ví dụ viên đạn của Rex có tag là "Bullet"
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(15); // Nhận 15 sát thương mỗi viên đạn
            Destroy(other.gameObject); // Phá hủy viên đạn
        }
    }
}