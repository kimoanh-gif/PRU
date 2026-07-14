using System.Collections;
using UnityEngine;

public class TimedZombieSpawner : MonoBehaviour
{
    [Header("Cau Hinh Nhom Zombie")]
    [Tooltip("Keo Object Cha chua cac con Zombie vao day")]
    [SerializeField] private GameObject zombieGroup;

    [Header("Cau Hinh Thoi Gian")]
    [Tooltip("Khoang thoi gian cho tu khi vao game den khi Zombie xuat hien (giay)")]
    [SerializeField] private float spawnDelay = 1.0f;

    void Start()
    {
        // An nhom Zombie ngay khi khoi chay de cho den gio phuc kich
        if (zombieGroup != null)
        {
            zombieGroup.SetActive(false);
        }

        // Bat dau dem nguoc de goi Zombie xuat hien
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        // Cho dung so giay ban cau hinh tren Inspector
        yield return new WaitForSeconds(spawnDelay);

        Debug.Log("[XUAT HIEN]: Thoi gian cho da het! Bay Zombie xuat hien!");

        // Kich hoat hien thi bay Zombie
        if (zombieGroup != null)
        {
            zombieGroup.SetActive(true);
        }

        // Tu huy doi tuong spawner nay de tiet kiem bo nho vi nhiem vu da hoan thanh
        Destroy(gameObject);
    }
}