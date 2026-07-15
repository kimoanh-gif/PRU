using System.Collections;
using UnityEngine;

public class NhenAI : MonoBehaviour
{
    [Header("=== Cấu Hình Tuần Tra ===")]
    public float tocDoTuanTra = 2f;
    public Transform pointA;
    public Transform pointB;
    private Vector3 targetDestination;

    [Header("=== Phát Hiện & Tấn Công ===")]
    public Transform player;
    public float tamNhin = 8f; // Tầm nhìn phát hiện Rex
    public float tamTanCong = 5f; // Khoảng cách đứng khạc đạn
    public float tocDoDuoiTheo = 3f;

    [Header("=== Bắn Đạn ===")]
    public GameObject danPrefab;    // Kéo Prefab viên đạn vào đây
    public Transform firePoint;     // Kéo Empty Object ở mồm nhện vào đây
    public float tocDoBayCuaDan = 8f;
    public float thoiGianHoiChieu = 2f;
    private float cooldownTimer;

    [Header("=== Trạng Thái ===")]
    public float maxMau = 50f;
    private float mauHienTai;
    private bool daChet = false;

    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        mauHienTai = maxMau;

        if (pointA != null) targetDestination = pointA.position;
    }

    void Update()
    {
        if (daChet) return;

        cooldownTimer += Time.deltaTime;

        float khoangCachDenPlayer = player != null ? Vector2.Distance(transform.position, player.position) : float.MaxValue;
        bool matDoiMat = KiemTraMatDoiMat();

        // 1. Nếu Player nằm trong tầm bắn VÀ mặt đối mặt (cùng độ cao trục Y)
        if (khoangCachDenPlayer <= tamTanCong && matDoiMat)
        {
            DungLai();
            QuayMatVePhia(player.position);

            if (cooldownTimer >= thoiGianHoiChieu)
            {
                TanCong();
            }
        }
        // 2. Nếu thấy Player trước mặt nhưng còn ở quá xa -> Đi lại gần hơn
        else if (khoangCachDenPlayer <= tamNhin && matDoiMat)
        {
            DuoiTheoPlayer();
        }
        // 3. Không thấy Player hoặc lệch độ cao hoặc ở sau lưng -> Đi tuần tra trái phải
        else
        {
            TuanTra();
        }
    }

    bool KiemTraMatDoiMat()
    {
        if (player == null) return false;

        // Nếu Rex đứng lệch quá 1.5 mét theo trục Y thì coi như không thấy
        if (Mathf.Abs(player.position.y - transform.position.y) > 1.5f)
        {
            return false;
        }

        // Logic hướng nhìn (Scale X dương là nhìn TRÁI, âm là nhìn PHẢI)
        float huongNhinNhen = transform.localScale.x > 0 ? -1f : 1f;
        float huongToPlayer = player.position.x - transform.position.x;

        if ((huongNhinNhen > 0 && huongToPlayer > 0) || (huongNhinNhen < 0 && huongToPlayer < 0))
        {
            return true;
        }
        return false;
    }

    void TuanTra()
    {
        if (pointA == null || pointB == null) return;

        // ĐÃ SỬA: Dùng "dichuyen" viết thường theo đúng Parameter của bạn
        if (anim != null) anim.SetTrigger("dichuyen");

        Vector2 direction = (targetDestination - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * tocDoTuanTra, rb.linearVelocity.y);

        QuayMatVePhia(targetDestination);

        if (Vector2.Distance(transform.position, targetDestination) < 0.5f)
        {
            targetDestination = (targetDestination == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void DuoiTheoPlayer()
    {
        // ĐÃ SỬA: Dùng "dichuyen" viết thường
        if (anim != null) anim.SetTrigger("dichuyen");

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * tocDoDuoiTheo, rb.linearVelocity.y);

        QuayMatVePhia(player.position);
    }

    void TanCong()
    {
        cooldownTimer = 0f;
        // ĐÃ SỬA: Dùng "tancong" viết thường
        if (anim != null) anim.SetTrigger("tancong");
    }

    public void ThucHienBanDan()
    {
        if (daChet || player == null || danPrefab == null || firePoint == null) return;

        GameObject vienDan = Instantiate(danPrefab, firePoint.position, Quaternion.identity);

        float huongBan = transform.localScale.x > 0 ? -1f : 1f;
        Vector2 vectorHuongBay = new Vector2(huongBan, 0f);

        Rigidbody2D rbDan = vienDan.GetComponent<Rigidbody2D>();
        if (rbDan != null)
        {
            rbDan.gravityScale = 0;
            rbDan.linearVelocity = vectorHuongBay * tocDoBayCuaDan;
        }

        Vector3 scaleDan = vienDan.transform.localScale;
        scaleDan.x = Mathf.Abs(scaleDan.x) * huongBan;
        vienDan.transform.localScale = scaleDan;
    }

    public void TakeDamage(float damage)
    {
        if (daChet) return;

        mauHienTai -= damage;
        if (mauHienTai <= 0)
        {
            Chet();
        }
    }

    void Chet()
    {
        daChet = true;
        DungLai();
        GetComponent<Collider2D>().enabled = false;

        // ĐÃ SỬA: Dùng "chet" viết thường
        if (anim != null) anim.SetTrigger("chet");

        StartCoroutine(BienMat());
    }

    IEnumerator BienMat()
    {
        yield return new WaitForSeconds(1.5f);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float duration = 1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    void DungLai()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void QuayMatVePhia(Vector3 target)
    {
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