using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    [SerializeField] private float moveSpeed = 3f; // Tốc độ di chuyển của Zombie

    private Transform player; // Rex
    private Rigidbody2D rb;
    private Animator anim;    // Bộ não chuyển động
    private bool isFacingRight = false; // Mặc định ban đầu dựa trên hình ảnh Sprite

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (anim == null)
        {
            Debug.LogError("Zombie chưa có component Animator!");
        }

        // Tự động tìm Rex qua Tag Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Tự động kiểm tra hướng nhìn ban đầu của Zombie qua Scale.x
        isFacingRight = transform.localScale.x > 0;
    }

    void Update()
    {
        if (player == null) return;

        // Luôn luôn đuổi theo Rex
        ChasePlayer();
    }

    void ChasePlayer()
    {
        // 1. Tính toán hướng di chuyển dựa trên vị trí của Rex
        float directionX = player.position.x - transform.position.x;

        // Nếu ở xa Rex, Zombie sẽ chạy đuổi theo
        if (Mathf.Abs(directionX) > 0.2f)
        {
            float speedX = directionX > 0 ? moveSpeed : -moveSpeed;
            rb.linearVelocity = new Vector2(speedX, rb.linearVelocity.y);

            // 2. KÍCH HOẠT ANIMATION: Bật biến 'isWalking' thành true đúng như bảng Animator
            if (anim != null)
            {
                anim.SetBool("isWalking", true);
            }

            // 3. Logic lật mặt tự động (Flip)
            if ((directionX > 0 && !isFacingRight) || (directionX < 0 && isFacingRight))
            {
                Flip();
            }
        }
        else
        {
            // Nếu đã chạm sát sạt vào người Rex thì đứng im
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            // TẮT ANIMATION: Trả biến 'isWalking' về false để Zombie đứng im thở
            if (anim != null)
            {
                anim.SetBool("isWalking", false);
            }
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        // Lật mặt Sprite 2D bằng cách đảo dấu trục X của Scale
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}