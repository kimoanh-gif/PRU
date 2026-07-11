using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    private float horizontalInput;
    private bool isFacingRight = true;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheckPoint; // Tạo một Empty Object đặt dưới chân Rex
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f); // Kích thước vùng kiểm tra chân chạm đất
    [SerializeField] private LayerMask groundLayer; // Chọn Layer của Ground và Platforms
    private bool isGrounded;
    private bool jumpRequested;

    // --- ĐÃ THÊM: CẤU HÌNH ĐỔI HOẠT ẢNH SÚNG MỚI ---
    [Header("Weapon Upgrade Settings")]
    [Tooltip("Kéo file Rex_AK_Override ở ô Project vào đây")]
    [SerializeField] private AnimatorOverrideController boAnimSungAK; 

    // Các thành phần vật lý và hoạt họa của nhân vật
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        // Tự động tìm và gán các thành phần từ GameObject Player
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Nhận nút di chuyển (A/D hoặc mũi tên Trái/Phải)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Nhận nút Nhảy (Nút Space/Phím cách) - Kiểm tra xem có đang đứng trên đất không
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }

        // Gọi hàm xử lý lật hướng mặt nhân vật
        FlipController();

        // Cập nhật trạng thái chạm đất liên tục
        CheckGround();

        // ĐIỀU KHIỂN CÁC TRẠNG THÁI ANIMATION
        if (anim != null)
        {
            anim.speed = 1f;

            // Chuyển đổi trạng thái Chạy (Run)
            anim.SetBool("isRunning", horizontalInput != 0);

            // Chuyển đổi trạng thái Nhảy (Jump)
            anim.SetBool("isGrounded", isGrounded);

            // Cập nhật biến float "yVelocity" trong Animator để làm anim rơi xuống
            anim.SetFloat("yVelocity", rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Áp dụng lực vật lý di chuyển nhân vật sang trái hoặc phải
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

            // Xử lý lực nhảy trong FixedUpdate để đồng bộ vật lý mượt mà hơn
            if (jumpRequested)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpRequested = false; // Reset yêu cầu nhảy
            }
        }
    }

    void CheckGround()
    {
        if (groundCheckPoint != null)
        {
            // Tạo một hộp ảo ngay dưới chân để quét xem có chạm trần của Layer Ground/Platforms không
            isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0f, groundLayer);
        }
        else
        {
            // Nếu chưa kéo vị trí Check, tạm thời xem như luôn chạm đất để không bị lỗi không nhảy được
            isGrounded = true;
        }
    }

    void FlipController()
    {
        // Logic lật mặt nhân vật dựa vào phím bấm hướng đi và hướng mặt hiện tại
        if ((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;

            // Đảo ngược giá trị X của Scale để lật ngược hình ảnh (Sprite)
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    // --- ĐÃ THÊM: HÀM XỬ LÝ CHẠM VÀO SÚNG ĐỂ ĐỔI HOẠT ẢNH ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi Rex chạm vào khẩu súng rơi ra từ hòm báu (Nhớ đặt Tag của file súng dưới đất là GunItem nhé)
        if (collision.CompareTag("GunItem"))
        {
            // 1. Nhặt súng: Xóa cây súng nằm dưới đất đi
            Destroy(collision.gameObject); 

            // 2. Thay bộ não hoạt ảnh: Ép Animator đổi sang bộ ảnh AK của file Override
            if (anim != null && boAnimSungAK != null)
            {
                anim.runtimeAnimatorController = boAnimSungAK;
                Debug.Log("🔫 Thành công! Đã tráo toàn bộ ảnh Rex súng lục sang tư thế cầm AK!");
            }
            else
            {
                Debug.LogError("Lỗi: Chưa kéo file Rex_AK_Override vào bảng Inspector của Rex!");
            }

           
        }
    }

    // Vẽ hộp kiểm tra mặt đất trong cửa sổ Scene để bạn dễ căn chỉnh độ rộng của chân Rex
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        }
    }
}