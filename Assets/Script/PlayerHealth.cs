using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // BẮT BUỘC phải có thư viện này để điều khiển Slider

public class PlayerHealth : MonoBehaviour
{
    [Header("Cài đặt máu của Rex")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Gắn thanh máu UI")]
    [SerializeField] private Slider healthSlider; // Kéo thanh Slider vào đây

    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        // Thêm dòng này để ép thanh máu đầy 100% ngay lập tức khi load màn
        UpdateHealthUI();
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Rex bị cắn! Máu còn: " + currentHealth);

        UpdateHealthUI(); // Cập nhật lại thanh máu trên màn hình

        if (sr != null) StartCoroutine(FlashRed());

        if (currentHealth <= 0) Die();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Rex được hồi máu! Hiện tại: " + currentHealth);

        UpdateHealthUI(); // Cập nhật lại thanh máu trên màn hình
    }

    // Hàm cập nhật thanh Slider UI
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    private System.Collections.IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = originalColor;
    }

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}