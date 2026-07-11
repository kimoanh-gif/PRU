using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform groundCheckPoint; // Điểm kiểm tra trước mặt xem còn đất không
    [SerializeField] private LayerMask groundLayer;      // Chọn Layer Ground

    private bool movingRight = true;
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Đảm bảo Layer của mặt đất được gán đúng
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Ground");
        }
    }

    void Update()
    {
        // 1. Cho Zombie di chuyển về phía trước
        float direction = movingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // 2. Kích hoạt Animation đi bộ (Nếu vận tốc trục X khác 0)
        if (anim != null)
        {
            anim.SetBool("isWalking", true);
        }

        // 3. Kiểm tra xem phía trước mặt còn đất không để quay đầu (Tránh rớt vực)
        if (groundCheckPoint != null)
        {
            // Bắn một tia Raycast nhỏ từ điểm check hướng xuống dưới
            bool hasGroundAhead = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, 0.5f, groundLayer);

            // Nếu phía trước trống rỗng (không chạm vào Ground Layer), tiến hành quay đầu
            if (!hasGroundAhead)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        movingRight = !movingRight;

        // Xoay ngược hướng mặt của Zombie
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // Vẽ một vòng tròn nhỏ trong chế độ Scene để dễ nhìn vị trí điểm Check
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, 0.1f);
        }
    }
}