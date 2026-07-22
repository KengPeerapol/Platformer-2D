using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance;

    [Header("UI Settings")]
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private GameObject gameOverPanel; // ช่องสำหรับใส่ UI หน้าจอ Game Over

    // ตัวแปรเก็บชีวิต ใช้ static เพื่อให้จำค่าข้ามด่านได้ เริ่มต้นที่ 3
    private static int lives = 3;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // ซ่อนหน้า Game Over ไว้ก่อนตอนเริ่มเกม และปรับเวลาให้เดินปกติ
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        Time.timeScale = 1f;

        UpdateUI();
    }

    public void LoseLife(PlayerController player)
    {
        lives--;
        UpdateUI();

        if (lives > 0)
        {
            // ถ้าชีวิตยังเหลือ สั่งให้ Player วาร์ปกลับ Checkpoint
            if (player != null) player.Respawn();
        }
        else
        {
            // ถ้าชีวิตเหลือ 0 (หรือน้อยกว่า) เรียกฟังก์ชันจบเกม
            GameOver();
        }
    }

    private void UpdateUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
    }

    private void GameOver()
    {
        // เปิดหน้าจอ Game Over
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // หยุดเวลาในเกมเพื่อไม่ให้ศัตรูขยับ หรือผู้เล่นเดินต่อได้
        Time.timeScale = 0f;
    }

    // ฟังก์ชันนี้เอาไว้ผูกกับปุ่ม "Restart" บนหน้า Game Over
    public void RestartGame()
    {
        lives = 3; // รีเซ็ตชีวิตกลับเป็น 3

        // ถ้าคุณมีฟังก์ชัน ResetPoints() ใน PointsManager ให้เรียกใช้ด้วย (ถ้าไม่มีให้ลบบรรทัดนี้ทิ้ง)
        if (PointsManager.Instance != null) PointsManager.Instance.ResetPoints();

        Time.timeScale = 1f; // คืนค่าเวลา

        // โหลดด่านแรกสุดใหม่ (หรือจะใช้ SceneManager.GetActiveScene().buildIndex เพื่อโหลดด่านปัจจุบันใหม่ก็ได้)
        SceneManager.LoadScene(0);
    }
}