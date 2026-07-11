using System.Collections;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Cài đặt hồi máu")]
    [SerializeField] private float healAmount = 50f;

    [Header("Cài đặt súng rơi ra")]
    [SerializeField] private GameObject gunPrefab; // Kéo Prefab khẩu súng nằm dưới đất vào đây

    [Header("Thời gian chờ mở nắp rương")]
    [SerializeField] private float delayBeforeSpawn = 0.5f; // Chờ 0.5 giây cho hoạt ảnh mở chạy xong

    private Animator anim;
    private bool hasOpened = false;

    void Start()
    {
        // Tự động lấy linh kiện Animator gắn trên chiếc rương
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi người chơi Rex chạm vào hòm báu
        if (collision.CompareTag("Player") && !hasOpened)
        {
            hasOpened = true; // Khóa lại ngay lập tức

            // 1. Kích hoạt Animation mở rương trên Animator
            if (anim != null)
            {
                anim.SetTrigger("Open");
            }

            // 2. Chạy hàm phụ để chờ mở nắp xong mới nhả đồ
            StartCoroutine(OpenChestRoutine(collision.gameObject));
        }
    }

    // Hàm xử lý thời gian chờ hoạt ảnh
    private IEnumerator OpenChestRoutine(GameObject playerObj)
    {
        // Chờ một khoảng thời gian bằng delayBeforeSpawn (Ví dụ: 0.5 giây)
        yield return new WaitForSeconds(delayBeforeSpawn);

        // 3. Hồi máu cho Rex
        PlayerHealth health = playerObj.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Heal(healAmount);
            Debug.Log($"Đã hồi {healAmount} máu cho Rex!");
        }

        // 4. Sinh ra súng dưới đất
        if (gunPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0.2f, 0.5f, 0f);
            Instantiate(gunPrefab, spawnPos, Quaternion.identity);
            Debug.Log("Rương đã mở toang và nhả vũ khí!");
        }

        // 5. Xóa chiếc rương đi sau khi đã hoàn tất
        Destroy(gameObject);
    }
}