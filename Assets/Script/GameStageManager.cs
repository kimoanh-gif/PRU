using UnityEngine;

public class GameStageManager : MonoBehaviour
{
    public static GameStageManager Instance;

    [Header("Cấu hình Rương")]
    [SerializeField] private GameObject chestPrefab;     // Kéo file HomBau_Prefab vào đây
    [SerializeField] private Transform chestSpawnPoint; // Tạo 1 Object trống làm điểm rơi rương

    private int currentKills = 0;
    private bool chestSpawned = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RegisterKill()
    {
        if (chestSpawned) return;
        currentKills++;

        if (currentKills >= 3) // Đủ 3 quái chết
        {
            chestSpawned = true;
            if (chestPrefab != null && chestSpawnPoint != null)
            {
                Instantiate(chestPrefab, chestSpawnPoint.position, Quaternion.identity);
                Debug.Log("🎉 Đủ 3 quái! Hòm báu đã xuất hiện!");
            }
        }
    }
}