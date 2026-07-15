using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Thư viện để chuyển cảnh

public class CongDichChuyen : MonoBehaviour
{
    [Header("Tên màn chơi tiếp theo")]
    [SerializeField] private string tenManChoiMoi = "Map5"; // Thay bằng tên chính xác Scene Màn 5 của bạn

    [Header("Thời gian đứng chờ (giây)")]
    [SerializeField] private float thoiGianCho = 1f;

    private float timer = 0f;
    private bool playerDangDungTrongCong = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi Rex (Tag Player) bước vào cổng
        if (collision.CompareTag("Player"))
        {
            playerDangDungTrongCong = true;
            timer = 0f; // Reset lại bộ đếm giây
            Debug.Log("Rex đã bước vào cổng dịch chuyển. Bắt đầu đếm ngược...");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Nếu Rex đi ra khỏi cổng trước khi đủ 1 giây
        if (collision.CompareTag("Player"))
        {
            playerDangDungTrongCong = false;
            timer = 0f; // Reset bộ đếm
            Debug.Log("Rex đã rời cổng. Hủy đếm ngược.");
        }
    }

    private void Update()
    {
        // Nếu Rex đang đứng yên trong cổng, tiến hành cộng dồn thời gian thực tế
        if (playerDangDungTrongCong)
        {
            timer += Time.deltaTime;

            if (timer >= thoiGianCho)
            {
                playerDangDungTrongCong = false; // Khóa lại tránh kích hoạt chuyển màn nhiều lần
                ChuyenMan();
            }
        }
    }

    private void ChuyenMan()
    {
        Debug.Log("Đã đứng đủ 1 giây! Đang chuyển qua màn chơi mới...");

        // Chuyển sang màn chơi mới
        SceneManager.LoadScene(tenManChoiMoi);
    }
}