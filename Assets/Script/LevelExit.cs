using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [Header("End of Game Settings")]
    [Tooltip("ถ้าเล่นจบด่านสุดท้ายแล้ว ให้กลับไปที่ Scene Index ที่เท่าไหร่ (ค่าเริ่มต้น 0 มักจะเป็นหน้า Main Menu)")]
    [SerializeField] private int mainMenuIndex = 0;

    private bool isLoading = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่าสิ่งที่มาชนคือผู้เล่น และยังไม่ได้ทำการโหลดฉากอยู่
        if (collision.CompareTag("Player") && !isLoading)
        {
            isLoading = true; // ล็อคการทำงานไว้ ป้องกันผู้เล่นเดินชนซ้ำรัวๆ
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        // ดึงค่า Index ของด่านปัจจุบัน แล้วบวก 1 เพื่อหาด่านถัดไป
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // ตรวจสอบว่าด่านถัดไป (nextSceneIndex) มีอยู่จริงใน Build Settings หรือไม่
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // ถ้ามีด่านถัดไป ให้ทำการโหลดฉากนั้นทันที
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // ถ้าไม่มีด่านถัดไปแล้ว (จบเกม) ให้โหลดกลับไปที่หน้าเมนูหลัก
            Debug.Log("ยินดีด้วย! จบเกมแล้ว โหลดหน้าเมนูหลัก...");
            SceneManager.LoadScene(mainMenuIndex);
        }
    }
}