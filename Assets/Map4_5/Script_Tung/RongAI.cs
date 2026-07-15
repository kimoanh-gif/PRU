using System.Collections;
using UnityEngine;

public class RongAI : MonoBehaviour
{
    [Header("=== Cấu Hình Bay Tuần Tra ===")]
    public float tocDoBayTuan = 2f;
    public Transform pointA; // Điểm bay qua
    public Transform pointB; // Điểm bay lại
    private Vector3 targetDestination;

    [Header("=== Phát Hiện & Đuổi Theo ===")]
    public Transform player; // Kéo Rex vào đây
    public float tamNhin = 10f; // Khoảng cách phát hiện Rex
    public float tamTanCong = 6f; // Khoảng cách dừng lại để khạc đạn
    public float tocDoDuoi = 4f;

    [Header("=== Tấn Công (Khạc Đạn) ===")]
    public GameObject danRongPrefab; // Kéo Prefab đạn của rồng vào đây
    public Transform firePoint;     // Vị trí mồm rồng để bắn đạn ra
    public float tocDoDan = 10f;
    public float thoiGianHoiChieu = 1.5f;
    private float cooldownTimer;

    [Tooltip("Tích chọn nếu viên đạn bay ngược với hướng nhìn của rồng")]
    public bool daoNguocHuongDan = false;

    private Animator anim;
    private Rigidbody2D rb;
    private bool daChet = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Tắt trọng lực để rồng không bị rơi tuột xuống đất
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }

        if (pointA != null) targetDestination = pointA.position;
    }

    void Update()
    {
        if (daChet) return;

        cooldownTimer += Time.deltaTime;

        float khoangCachDenPlayer = player != null ? Vector2.Distance(transform.position, player.position) : float.MaxValue;

        // 1. Nếu Rex trong tầm bắn -> Dừng lại khạc đạn
        if (khoangCachDenPlayer <= tamTanCong)
        {
            DungLai();
            QuayMatVePhia(player.position);

            if (cooldownTimer >= thoiGianHoiChieu)
            {
                TanCong();
            }
        }
        // 2. Nếu Rex trong tầm nhìn nhưng còn xa -> Bay đuổi theo Rex (theo cả X và Y)
        else if (khoangCachDenPlayer <= tamNhin)
        {
            DuoiTheoPlayer();
        }
        // 3. Không thấy Rex -> Bay tuần tra giữa Point A và Point B
        else
        {
            TuanTra();
        }
    }

    void TuanTra()
    {
        if (pointA == null || pointB == null) return;

        // Kích hoạt animation bay (Hãy đổi tên "bay" thành trigger bay của bạn nếu khác)
        if (anim != null) anim.SetTrigger("bay");

        // Di chuyển mượt mà tới điểm tuần tra
        Vector2 direction = (targetDestination - transform.position).normalized;
        rb.linearVelocity = direction * tocDoBayTuan;

        QuayMatVePhia(targetDestination);

        if (Vector2.Distance(transform.position, targetDestination) < 0.5f)
        {
            targetDestination = (targetDestination == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void DuoiTheoPlayer()
    {
        if (anim != null) anim.SetTrigger("bay");

        // Đuổi theo Rex theo hướng vector 2D (bay chéo lên/xuống nếu Rex nhảy)
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * tocDoDuoi;

        QuayMatVePhia(player.position);
    }

    void TanCong()
    {
        cooldownTimer = 0f;
        // Kích hoạt animation tấn công (Đổi thành tên trigger khạc đạn của bạn nếu khác)
        if (anim != null) anim.SetTrigger("tancong");
    }

    // Hàm này sẽ được gọi thông qua Animation Event ở khoảnh khắc rồng há mồm khạc đạn
    public void ThucHienBanDan()
    {
        if (daChet || player == null || danRongPrefab == null || firePoint == null) return;

        // 1. Ép tọa độ Z về 0 để tránh đạn bị tàng hình
        Vector3 viTriBan = new Vector3(firePoint.position.x, firePoint.position.y, 0f);
        GameObject vienDan = Instantiate(danRongPrefab, viTriBan, Quaternion.identity);

        // 2. Tính toán hướng bay thực tế dựa trên vị trí của Rex
        Vector2 huongBay = (player.position - viTriBan).normalized;

        // Nếu đạn bị ngược hướng, chúng ta nhân đảo ngược Vector lại
        if (daoNguocHuongDan)
        {
            huongBay = -huongBay;
        }

        // 3. Truyền lực bay cho đạn
        Rigidbody2D rbDan = vienDan.GetComponent<Rigidbody2D>();
        if (rbDan != null)
        {
            rbDan.gravityScale = 0f;
            rbDan.linearVelocity = huongBay * tocDoDan;
        }

        // 4. Xoay đầu viên đạn nhìn về phía Rex cho đẹp
        float angle = Mathf.Atan2(huongBay.y, huongBay.x) * Mathf.Rad2Deg;
        vienDan.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void DungLai()
    {
        rb.linearVelocity = Vector2.zero;
    }

    void QuayMatVePhia(Vector3 target)
    {
        // Nếu Sprite gốc của Rồng quay sang TRÁI:
        if (target.x > transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}