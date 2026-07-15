using UnityEngine;

public class CameraFollow_Tung : MonoBehaviour
{
    [Header("Mục tiêu theo dõi")]
    public Transform target;          // Kéo nhân vật chính Rex vào đây

    [Header("Cấu hình di chuyển")]
    public float smoothSpeed = 5f;    // Độ mượt khi camera đuổi theo nhân vật
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Khoảng cách lệch (Giữ Z luôn là -10)

    void LateUpdate()
    {
        // Nếu chưa kéo nhân vật vào hoặc nhân vật bị hủy, tự động tìm theo Tag của team
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        if (target != null)
        {
            // Tính toán vị trí camera cần tới
            Vector3 desiredPosition = target.position + offset;

            // Di chuyển mượt mà từ vị trí hiện tại tới vị trí đích
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Cập nhật vị trí cho Camera
            transform.position = smoothedPosition;
        }
    }
}