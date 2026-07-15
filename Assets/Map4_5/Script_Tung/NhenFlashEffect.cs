using UnityEngine;
using System.Collections;

public class NhenFlashEffect : MonoBehaviour
{
    [Header("Hiệu ứng chớp nháy")]
    [SerializeField] private float flashDuration = 0.15f;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Animator anim;

    // Lưu lại máu ở khung hình trước để phát hiện khi bị mất máu
    private float lastHealth;
    private EnemyHealth groupEnemyHealth;
    private bool playedDeathAnim = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        groupEnemyHealth = GetComponent<EnemyHealth>();

        // Quét toàn bộ SpriteRenderer trên thân quái
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                originalColors[i] = spriteRenderers[i].color;
        }

        // Đọc máu ban đầu từ file gốc của nhóm
        if (groupEnemyHealth != null)
        {
            // Dùng Reflection trong C# để đọc biến private currentHealth từ file gốc của nhóm mà không cần sửa file gốc!
            lastHealth = GetCurrentHealthFromGroupScript();
        }
    }

    void Update()
    {
        if (groupEnemyHealth == null) return;

        float currentHealth = GetCurrentHealthFromGroupScript();

        // 1. Nếu máu bị giảm so với khung hình trước -> Chớp đỏ!
        if (currentHealth < lastHealth && currentHealth > 0)
        {
            StopAllCoroutines();
            StartCoroutine(HieuUngChopDo());
            lastHealth = currentHealth;
        }

        // 2. Nếu máu về 0 và chưa chạy Animation chết -> Kích hoạt hoạt ảnh "chet" viết thường
        if (currentHealth <= 0 && !playedDeathAnim)
        {
            playedDeathAnim = true;
            if (anim != null)
            {
                anim.SetTrigger("chet");
            }

            // Tắt Collider để Rex đi xuyên qua được cái xác lúc đang diễn hoạt ảnh chết
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

    // Mẹo đọc biến private từ script khác cực kỳ an toàn
    private float GetCurrentHealthFromGroupScript()
    {
        try
        {
            var field = typeof(EnemyHealth).GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                return (float)field.GetValue(groupEnemyHealth);
            }
        }
        catch { }
        return 50f;
    }
}