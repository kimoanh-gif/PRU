using UnityEngine;

public class IdleBobbing : MonoBehaviour
{
    [Header("🛸 Cấu Hình Bập Bềnh")]
    [Tooltip("Tốc độ chuyển động lên xuống (Càng cao càng nhanh)")]
    [SerializeField] private float bobSpeed = 3.0f;

    [Tooltip("Độ cao di chuyển lên xuống (Càng cao bập bềnh càng mạnh)")]
    [SerializeField] private float bobHeight = 0.08f;

    [Header("🔄 Hiệu Ứng Nghiêng Nhẹ (Tùy Chọn)")]
    [Tooltip("Có muốn nhân vật hơi nghiêng qua lại khi bập bềnh không?")]
    [SerializeField] private bool enableTilt = true;
    [Tooltip("Góc nghiêng tối đa (độ)")]
    [SerializeField] private float maxTiltAngle = 3.0f;

    private Vector3 startLocalPosition;
    private float randomOffset; // Giúp các con quái không bị nhún nhảy đều tăm tắp cùng một nhịp

    void Start()
    {
        // 1. Lưu lại vị trí ban đầu của Sprite để làm điểm tựa nhấp nhô
        startLocalPosition = transform.localPosition;

        // 2. Tạo một số ngẫu nhiên để các con quái nhấp nhô so le nhau nhìn cho tự nhiên
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // 3. Sử dụng hàm Sin để tạo chuyển động hình sóng uốn lượn tuần hoàn mượt mà
        float timeValue = (Time.time * bobSpeed) + randomOffset;
        float newY = startLocalPosition.y + (Mathf.Sin(timeValue) * bobHeight);

        // Áp dụng vị trí mới vào trục Y (giữ nguyên trục X và Z của đối tượng)
        transform.localPosition = new Vector3(startLocalPosition.x, newY, startLocalPosition.z);

        // 4. Nếu bật hiệu ứng nghiêng, xoay nhẹ Sprite qua lại để tạo cảm giác "mềm mại"
        if (enableTilt)
        {
            float tiltZ = Mathf.Cos(timeValue) * maxTiltAngle;
            transform.localRotation = Quaternion.Euler(0, 0, tiltZ);
        }
    }
}