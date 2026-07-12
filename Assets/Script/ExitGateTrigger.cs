using UnityEngine;

public class ExitGateTrigger : MonoBehaviour
{
    // Biến trạng thái để đảm bảo sự kiện Boss chỉ được gọi duy nhất 1 lần
    private bool isTriggered = false;

    /// <summary>
    /// Hàm tự động chạy khi có bất kỳ vật thể nào đi xuyên qua vùng Collider ẩn
    /// </summary>
    /// <param name="other">Vật thể va chạm vào vùng kích hoạt này</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kiểm tra xem vật thể chạm vào cổng có phải là người chơi (Rex) hay không
        // Hãy chắc chắn bạn đã gán Tag "Player" cho Rex trên Inspector!
        if (other.CompareTag("Player") && !isTriggered)
        {
            // 2. Kiểm tra xem GameManager đã sẵn sàng cho sự kiện Boss chưa (đã diệt hết quái và cứu Tiến sĩ chưa)
            if (GameManager.instance != null)
            {
                if (GameManager.instance.IsReadyForBossAmbush())
                {
                    // Đánh dấu đã kích hoạt cổng thành công để khóa Trigger lại
                    isTriggered = true;

                    Debug.Log("🚪 [CỔNG RA]: Rex đã chạy tới cổng thoát hiểm! Đủ điều kiện kích hoạt Boss!");

                    // 3. Ra lệnh cho GameManager bắt đầu sự kiện Boss gầm rú xuất hiện
                    GameManager.instance.TriggerBossAmbush();
                }
                else
                {
                    // Nếu người chơi chạy đến cổng nhưng chưa làm xong nhiệm vụ
                    Debug.Log("🚧 [CỔNG KHÓA]: Bạn chưa diệt sạch Zombie hoặc chưa giải cứu Tiến sĩ Elias!");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ [CẢNH BÁO]: Không tìm thấy GameManager trong Scene này!");
            }
        }
    }
}