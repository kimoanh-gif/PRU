using UnityEngine;

public class EnemyHealth_Tung : MonoBehaviour
{
    [Header("Cài đặt máu")]
    public float maxHealth = 50f; // Bạn có thể chỉnh 200 cho Boss, 50 cho Nhện
    private float currentHealth;

    private Animator anim;
    private bool daChet = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    // Khi đạn Rex bắn trúng, nó sẽ gọi hàm này để trừ máu
    public void TakeDamage(float damageAmount)
    {
        if (daChet) return;

        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " bị dính đạn! Máu còn: " + currentHealth);

        // Kích hoạt hiệu ứng chớp đỏ (nếu có script RongFlashEffect gắn kèm)
        // Code này vô hại kể cả khi không có script chớp đỏ
        StartCoroutine(HieuUngChopDo());

        // Nếu hết máu thì chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        daChet = true;
        Debug.Log(gameObject.name + " đã tử trận!");

        // 1. Chạy animation chết "chet"
        if (anim != null)
        {
            anim.SetTrigger("chet");
        }

        // 2. Tắt Collider để Rex không bị vấp vào xác quái
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 3. Cho quái biến mất sau 1.5 giây để kịp diễn xong animation chết
        Destroy(gameObject, 1.5f);
    }

    // Hàm chớp đỏ nhanh khi dính đạn
    private System.Collections.IEnumerator HieuUngChopDo()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            sr.color = originalColor;
        }
    }
}