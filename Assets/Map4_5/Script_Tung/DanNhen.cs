using UnityEngine;
using System.Collections;

public class DanNhen : MonoBehaviour
{
    [Header("Cài đặt sát thương lên Rex")]
    public float satThuong = 10f; // Rex sẽ mất 10 máu mỗi phát trúng

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(TuDongXoayTheoHuongBay());
    }

    IEnumerator TuDongXoayTheoHuongBay()
    {
        yield return new WaitForEndOfFrame();

        if (rb != null && rb.linearVelocity.x != 0)
        {
            float huongX = rb.linearVelocity.x;
            Vector3 scale = transform.localScale;

            // Sprite gốc quay sang trái:
            // Bay sang phải (huongX > 0) -> Lật âm Scale X để quay sang phải
            // Bay sang trái (huongX < 0) -> Giữ nguyên dương để quay sang trái
            if (huongX > 0)
            {
                scale.x = -Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = Mathf.Abs(scale.x);
            }
            transform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi đạn chạm vào Rex (Tag là Player)
        if (collision.CompareTag("Player"))
        {
            // Gọi trực tiếp script PlayerHealth có sẵn của Rex để trừ máu và chớp đỏ
            PlayerHealth rexHealth = collision.GetComponent<PlayerHealth>();
            if (rexHealth != null)
            {
                rexHealth.TakeDamage(satThuong);
            }

            // Hủy viên đạn ngay lập tức để tránh bay xuyên qua Rex
            Destroy(gameObject);
        }
    }
}