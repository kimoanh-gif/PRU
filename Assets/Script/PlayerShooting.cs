using UnityEngine;

[RequireComponent(typeof(AudioSource))] // Tự động yêu cầu có AudioSource trên người Rex
public class PlayerShooting : MonoBehaviour
{
    [Header("Cài đặt bắn súng")]
    [SerializeField] private GameObject bulletPrefab; // Kéo Prefab viên đạn vào đây
    [SerializeField] private Transform firePoint;     // Vị trí nòng súng (FirePoint)
    [SerializeField] private float bulletSpeed = 15f;

    [Header("Cấu hình Cúi Bắn")]
    [SerializeField] private float crouchFirePointYOffset = -0.4f; // Khoảng cách hạ thấp nòng súng (Y) khi cúi

    [Header("Gắn súng cũ để ẩn đi")]
    [SerializeField] private GameObject oldGunObject; // Kéo khẩu súng cũ trên người Rex vào đây

    // ==========================================
    // 🔊 THÊM PHẦN KHAI BÁO ÂM THANH SÚNG Ở ĐÂY
    // ==========================================
    [Header("🔊 Cấu Hình Âm Thanh Súng")]
    [Tooltip("Tiếng súng lục bình thường (Lúc đầu game - ID 0)")]
    [SerializeField] private AudioClip pistolShotSound;
    [Tooltip("Tiếng súng nâng cấp cực mạnh (Sau khi nhặt súng mới - ID 1)")]
    [SerializeField] private AudioClip upgradedShotSound;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.8f;   // Độ to của tiếng súng

    private AudioSource audioSource; // "Loa" để phát âm thanh
    // ==========================================

    // Biến lưu trữ ID súng hiện tại (0: Súng thường, 1: Súng mới cấp cao)
    private int currentGunID = 0;
    private Animator anim;
    private Rigidbody2D rb; // Để đóng băng tốc độ khi đang cúi bắn

    private Vector3 originalFirePointPos; // Vị trí nòng súng lúc đứng
    private Vector3 crouchFirePointPos;   // Vị trí nòng súng lúc cúi

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Lưu lại vị trí nòng súng mặc định và tính toán vị trí nòng súng khi ngồi
        if (firePoint != null)
        {
            originalFirePointPos = firePoint.localPosition;
            crouchFirePointPos = originalFirePointPos + new Vector3(0f, crouchFirePointYOffset, 0f);
        }

        // ==========================================
        // 🔊 TỰ ĐỘNG KHỞI TẠO LOA PHÁT TRÊN REX
        // ==========================================
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0.5f; // Hiệu ứng âm thanh không gian 2D/3D nhẹ
        audioSource.playOnAwake = false;
        // ==========================================
    }

    void Update()
    {
        // 1. LOGIC XỬ LÝ CÚI BẤN
        // Kiểm tra nếu người chơi GIỮ phím S hoặc phím Mũi tên xuống
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (anim != null) anim.SetBool("isCrouching", true); // Kích hoạt animation Rex_Crouch_Shoot

            if (firePoint != null) firePoint.localPosition = crouchFirePointPos; // Hạ thấp nòng súng xuống

            // Đóng băng vận tốc trục X để Rex không bị trượt đi nếu lỡ tay bấm nút chạy khi đang ngồi
            if (rb != null) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            if (anim != null) anim.SetBool("isCrouching", false); // Quay về Animation đứng bình thường

            if (firePoint != null) firePoint.localPosition = originalFirePointPos; // Trả nòng súng về vị trí cũ
        }

        // 2. LOGIC BẮN ĐẠN
        if (Input.GetMouseButtonDown(0)) // Click chuột trái để bắn (hoạt động ở cả tư thế đứng/ngồi)
        {
            Shoot();
        }
    }

    // Hàm nhận diện khi nhặt được súng mới
    public void ChangeWeapon(int newGunID)
    {
        currentGunID = newGunID;

        if (anim != null)
        {
            anim.SetInteger("gunID", newGunID); // Đổi Animation cầm súng mới
        }

        // --- ĐOẠN CODE ĐỂ ẨN SÚNG CŨ KHI NHẶT SÚNG MỚI ---
        if (newGunID == 1 && oldGunObject != null)
        {
            oldGunObject.SetActive(false); // Tắt hẳn khẩu súng cũ đi, biến mất hoàn toàn!
        }
        // -------------------------------------------------

        Debug.Log("Rex đã nâng cấp lên súng mới! Đã ẩn súng cũ.");
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // ==========================================
        // 🔊 TỰ ĐỘNG PHÁT TIẾNG SÚNG NỔ PHÙ HỢP VỚI LOẠI SÚNG ĐANG CẦM
        // ==========================================
        // Nếu đang cầm súng nâng cấp (ID = 1) và đã gán âm thanh upgradedShotSound thì phát nó, ngược lại dùng tiếng súng lục mặc định
        AudioClip clipToPlay = (currentGunID == 1 && upgradedShotSound != null) ? upgradedShotSound : pistolShotSound;

        if (clipToPlay != null && audioSource != null)
        {
            // Thay đổi cao độ (pitch) nhẹ ngẫu nhiên từ 0.95 đến 1.05 giúp tiếng súng dồn dập tự nhiên
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clipToPlay, soundVolume);
        }
        // ==========================================

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        float directionX = transform.localScale.x < 0 ? -1f : 1f;

        // Nếu đang cầm súng mới (ID = 1) thì đạn bay siêu nhanh (gấp đôi tốc độ)
        float finalSpeed = (currentGunID == 1) ? bulletSpeed * 2f : bulletSpeed;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = new Vector2(finalSpeed * directionX, 0f);
        }

        Vector3 bulletScale = bullet.transform.localScale;
        bulletScale.x = Mathf.Abs(bulletScale.x) * directionX;
        bullet.transform.localScale = bulletScale;
    }
}