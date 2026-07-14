using System.Collections; // Bat buoc phai co de chay Coroutine
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("CAI DAT KIEU MAN CHOI (QUAN TRONG)")]
    [Tooltip("Tich chon neu man nay co su kien Boss Ambush (Man 2). Bo tich neu la man ban quai thuong qua man luon (Man 1).")]
    [SerializeField] private bool coSuKienBoss = false;

    [Header("Cai dat Nut Qua Man Nho")]
    [SerializeField] private GameObject nutQuaManButton;

    [Header("Cai dat giao dien chien thang cu")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private string nextSceneName;

    [Header("Cai dat Ruong Thuong Man")]
    [SerializeField] private GameObject ruongPrefab;
    [SerializeField] private Transform viTriRoiRuong;

    [Header("CAU HINH SO QUAI CHET DE RA GUONG (MOI)")]
    [Tooltip("Man 1 muon giet 1 con ra ruong thi dien so 1 vao day. Man 2 dien so 3.")]
    [SerializeField] private int quaiChetDeRaRuong = 3;

    [Header("Cau hinh tong so luong quai trong man")]
    [SerializeField] private int totalZombies = 6;

    [Header("CHUOI DOI THOAI MO DAU MAN")]
    [Tooltip("Keo file am thanh tieng Tien si goi bo dam vao day (Chi dung neu co hoi thoai)")]
    [SerializeField] private AudioClip drIntroRadioClip;
    [Tooltip("Keo doi tuong Rex (da gan script RexDialogue) vao day (Chi dung neu co hoi thoai)")]
    [SerializeField] private RexDialogue rexDialogue;

 
    [Header("CAU HINH SU KIEN BOSS TRUM CUOI")]
    [Tooltip("Keo Prefab con Boss quai vat bay vao day")]
    [SerializeField] private GameObject bossPrefab;
    [Tooltip("Keo mot Object rong lam vi tri xuat hien cua Boss tren cao")]
    [SerializeField] private Transform bossSpawnPoint;
    [Tooltip("Tieng gam ru cuc lon cua quai vat khi xuat hien")]
    [SerializeField] private AudioClip bossRoarSound;
    [Tooltip("Giong thoai de doa cua Boss")]
    [SerializeField] private AudioClip bossDialogueVoice;
    [Tooltip("Giong thoai phan hoi cua Rex")]
    [SerializeField] private AudioClip rexBossReplyVoice;

    private AudioSource audioSource; // "Loa" phat tieng bo dam, tieng gam va giong Boss
    private AudioSource rexAudioSource; // "Loa" cua Rex de phat giong Rex dap tra Boss

    private int zombiesKilledCount = 0;
    private bool chestSpawned = false;
    private float timer = 0f;
    private bool isVictory = false;

    // Cac bien trang thai kiem tra dieu kien xuat hien Boss
    private bool drRescued = false;
    private bool normalZombiesCleared = false;
    private bool bossEventTriggered = false;
    private bool bossDefeated = false;

    void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f; // Am thanh 2D nghe ro deu 2 tai
    }

    void Start()
    {
        if (nutQuaManButton != null)
        {
            nutQuaManButton.SetActive(false);
        }

        // Tim AudioSource tren nguoi Rex de chuan bi thoai
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Rex");
        if (playerObj != null)
        {
            rexAudioSource = playerObj.GetComponent<AudioSource>();
            if (rexAudioSource == null)
            {
                rexAudioSource = playerObj.AddComponent<AudioSource>();
            }
        }

        // Kich hoat chuoi hoi thoai mo man (Neu duoc gan day du)
        if (drIntroRadioClip != null || rexDialogue != null)
        {
            StartCoroutine(PlayOpeningDialogueSequence());
        }
    }

    void Update()
    {
        if (!isVictory)
        {
            timer += Time.deltaTime;
        }
    }

    private IEnumerator PlayOpeningDialogueSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (drIntroRadioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(drIntroRadioClip);
            yield return new WaitForSeconds(drIntroRadioClip.length + 0.3f);
        }

        if (rexDialogue != null)
        {
            rexDialogue.PlayStartVoice();
        }
    }

    public void ZombieKilled()
    {
        zombiesKilledCount++;

        if (totalZombies > 0)
        {
            totalZombies--;
        }

        Debug.Log($"BAO CAO: Quai chet! Da giet: {zombiesKilledCount} con. Con lai: {totalZombies} con.");

        if (zombiesKilledCount >= quaiChetDeRaRuong && !chestSpawned)
        {
            chestSpawned = true;
            SpawnRewardChest();
        }

        // Khi diet sach quai thuong trong man
        if (totalZombies <= 0 && !normalZombiesCleared)
        {
            normalZombiesCleared = true;

            // NEU LA MAN THUONG (MAN 1) -> KHONG CO BOSS: Cho qua man luon!
            if (!coSuKienBoss)
            {
                isVictory = true;
                HienNutQuaManNho();
                Debug.Log("Man choi hoan thanh! Da xuat hien nut qua man.");
            }
            else
            {
                Debug.Log("Da diet sach quai thuong! Dang doi cuu Tien si va di ra cong de kich hoat Boss...");
            }
        }
    }

    /// <summary>
    /// Goi tu script DrRescue khi long kinh vo thanh cong
    /// </summary>
    public void SetDrRescued()
    {
        drRescued = true;
        Debug.Log("Tien si Elias da duoc giai cuu tu do!");
    }

    /// <summary>
    /// Kiem tra xem Rex va Tien si da du dieu kien de kich hoat su kien Boss chua
    /// </summary>
    public bool IsReadyForBossAmbush()
    {
        // Phai la man co Boss, da diet het quai thuong va da cuu duoc Tien si
        return coSuKienBoss && normalZombiesCleared && drRescued && !bossEventTriggered;
    }

    /// <summary>
    /// Kich hoat chuoi su kien Trum Cuoi Xuat Hien khi nguoi choi cham vung Cong Thoat
    /// </summary>
    public void TriggerBossAmbush()
    {
        if (bossEventTriggered) return;
        bossEventTriggered = true;

        StartCoroutine(PlayBossAmbushSequence());
    }

    /// <summary>
    /// Chuoi dien anh: Dat rung chuyen -> Boss gam ru xuat hien -> Doi thoai -> Danh nhau
    /// </summary>
    private IEnumerator PlayBossAmbushSequence()
    {
        Debug.Log("BAT DAU SU KIEN TRUM CUOI XUAT HIEN!");

        // 1. Phat tieng gam ru cuc lon cua quai vat bay
        if (bossRoarSound != null)
        {
            audioSource.PlayOneShot(bossRoarSound, 1f);
        }

        yield return new WaitForSeconds(1.5f);

        // 2. Trieu hoi (Spawn) con Boss quai vat bay tai vi tri tren cao
        GameObject bossInstance = null;
        BossFlyingAI bossAI = null;

        if (bossPrefab != null && bossSpawnPoint != null)
        {
            bossInstance = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
            bossAI = bossInstance.GetComponent<BossFlyingAI>();
            if (bossAI != null)
            {
                bossAI.InitializeBoss(false); // Vo hieu hoa AI tan cong tam thoi trong luc thoai
            }
        }

        yield return new WaitForSeconds(1.0f);

        // 3. Boss cat giong thoai de doa
        if (bossDialogueVoice != null && audioSource != null)
        {
            Debug.Log("BOSS: Rex... nguoi khong thoat duoc dau!");
            audioSource.PlayOneShot(bossDialogueVoice, 0.9f);
            yield return new WaitForSeconds(bossDialogueVoice.length + 0.5f);
        }

        // 4. Rex cat giong thoai dap tra dung cam
        if (rexBossReplyVoice != null && rexAudioSource != null)
        {
            Debug.Log("Rex: Lai them mot thuc the dot bien nua sao? Tien si, hay lui lai sau!");
            rexAudioSource.PlayOneShot(rexBossReplyVoice, 0.9f);
            yield return new WaitForSeconds(rexBossReplyVoice.length + 0.3f);
        }

        // 5. Chinh thuc cho phep Boss bat dau lao vao tan cong Rex
        if (bossAI != null)
        {
            bossAI.InitializeBoss(true); // Kich hoat AI chien dau!
            Debug.Log("CHIEN DAU BAT DAU! DIET BOSS DE THOAT THAN!");
        }
    }

    /// <summary>
    /// Goi tu script Boss khi Boss bi tieu diet
    /// </summary>
    public void BossKilled()
    {
        if (bossDefeated) return;
        bossDefeated = true;
        isVictory = true;

        Debug.Log("Boss Trum Cuoi da bi tieu diet hoan toan!");
        HienNutQuaManNho();
    }

    void SpawnRewardChest()
    {
        if (ruongPrefab != null && viTriRoiRuong != null)
        {
            Instantiate(ruongPrefab, viTriRoiRuong.position, Quaternion.identity);
            Debug.Log("Ruong bau da xuat hien thanh cong!");
        }
    }

    void HienNutQuaManNho()
    {
        if (nutQuaManButton == null)
        {
            nutQuaManButton = GameObject.Find("NutQuaMan");
        }

        if (nutQuaManButton != null)
        {
            nutQuaManButton.SetActive(true);
            Debug.Log("DA HIEN NUT NEXT LEVEL THANH CONG!");
        }
    }

    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // =========================================================================
    // CODE THEM MOI: XU LY CLICK BUTTON
    // =========================================================================

    // Thuc thi khi click vao vung nut START
    public void StartGame(string targetSceneName)
    {
        Debug.Log("Bat dau vao game!");
        SceneManager.LoadScene(targetSceneName);
    }

    // Thuc thi khi click vao vung nut CHOI LAI
    public void ChoiLaiManHienTai()
    {
        Debug.Log("Dang tai lai man choi hien tai...");
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}