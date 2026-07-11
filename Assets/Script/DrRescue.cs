using UnityEngine;
using UnityEngine.UI;

public class DrRescue : MonoBehaviour
{
    private bool isFree = false;
    private bool reachedRex = false;
    private float originalScaleX;

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Transform player;
    private Collider2D drCollider;
    private Rigidbody2D rb;

    [Header("Cấu hình di chuyển")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private float delayBeforeNextScene = 2.0f;

    [Header("Cấu hình Tự Động Bấm Button Chuyển Màn")]
    [SerializeField] private Button nextLevelButton;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        drCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        originalScaleX = transform.localScale.x;

        if (anim != null)
        {
            anim.applyRootMotion = false;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            playerObj = GameObject.Find("Rex");
        }

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // TỰ ĐỘNG CẤU HÌNH VẬT LÝ AN TOÀN
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (drCollider != null)
        {
            drCollider.isTrigger = true;
        }
    }

    void LateUpdate()
    {
        // Sử dụng LateUpdate để ép tọa độ di chuyển chạy SAU KHI Animator cập nhật clip hoạt họa.
        // Điều này sẽ ghi đè và sửa hoàn toàn lỗi giật lùi/nhúc nhích tại chỗ do dính keyframe Transform trong Animation.
        if (isFree && player != null && !reachedRex)
        {
            float distanceX = Mathf.Abs(transform.position.x - player.position.x);

            if (distanceX > stopDistance)
            {
                // Tính toán vị trí mới
                Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);

                // Di chuyển tịnh tiến bắt buộc bằng Code
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                // Tự động xoay mặt theo hướng Rex
                float directionX = player.position.x - transform.position.x;
                if (directionX > 0.05f)
                {
                    transform.localScale = new Vector3(Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
                }
                else if (directionX < -0.05f)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
                }

                if (anim != null)
                {
                    anim.SetBool("IsRunning", true);
                }
            }
            else
            {
                reachedRex = true;

                if (anim != null)
                {
                    anim.SetBool("IsRunning", false);
                }

                Debug.Log("🎉 Tiến sĩ Elias đã tiếp cận Rex an toàn! Chuẩn bị tự động chuyển màn...");
                Invoke("TransitionToNextStage", delayBeforeNextScene);
            }
        }
    }

    public void SetFree()
    {
        isFree = true;

        // Tách cha hoàn toàn để không chịu ảnh hưởng từ Phòng Kính
        transform.SetParent(null);

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 5;
        }

        // Tắt mô phỏng vật lý hoàn toàn để di chuyển thuần túy bằng tọa độ mượt mà nhất
        if (rb != null)
        {
            rb.simulated = false;
        }

        if (drCollider != null)
        {
            drCollider.isTrigger = true;
        }

        Debug.Log("👨‍⚕️ Dr. Elias đã được tự do và bắt đầu chạy!");
    }

    void TransitionToNextStage()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.Invoke();
        }
        else
        {
            Debug.LogError("⚠️ LỖI: Bạn chưa kéo Button 'Next Level' vào ô 'Next Level Button' của Script DrRescue trên Inspector!");
        }
    }
}