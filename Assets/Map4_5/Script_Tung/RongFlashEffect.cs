using UnityEngine;
using System.Collections;

public class RongFlashEffect : MonoBehaviour
{
    [Header("Hiệu ứng chớp nháy")]
    [SerializeField] private float flashDuration = 0.15f;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Animator anim;

    private float lastHealth;
    private EnemyHealth groupEnemyHealth;
    private bool playedDeathAnim = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        groupEnemyHealth = GetComponent<EnemyHealth>();

        // Lấy tất cả SpriteRenderer trên người rồng
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                originalColors[i] = spriteRenderers[i].color;
        }

        if (groupEnemyHealth != null)
        {
            lastHealth = GetCurrentHealthFromGroupScript();
        }
    }

    void Update()
    {
        if (groupEnemyHealth == null) return;

        float currentHealth = GetCurrentHealthFromGroupScript();

        // 1. Trúng đạn -> Chớp đỏ
        if (currentHealth < lastHealth && currentHealth > 0)
        {
            StopAllCoroutines();
            StartCoroutine(HieuUngChopDo());
            lastHealth = currentHealth;
        }

        // 2. Hết máu -> Chạy animation chết viết thường "chet"
        if (currentHealth <= 0 && !playedDeathAnim)
        {
            playedDeathAnim = true;
            if (anim != null)
            {
                anim.SetTrigger("chet");
            }

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    }

    private IEnumerator HieuUngChopDo()
    {
        foreach (var sr in spriteRenderers)
        {
            if (sr != null) sr.color = Color.red;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].color = originalColors[i];
        }
    }

    private float GetCurrentHealthFromGroupScript()
    {
        try
        {
            var field = typeof(EnemyHealth).GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) return (float)field.GetValue(groupEnemyHealth);
        }
        catch { }
        return 200f; // Mặc định nếu lỗi là 200 máu
    }
}