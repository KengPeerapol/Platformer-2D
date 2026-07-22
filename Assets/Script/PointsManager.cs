using UnityEngine;
using TMPro;

public class PointsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text pointsText;

    public static PointsManager Instance;

    // 🌟 พระเอกของเราอยู่ตรงนี้: เติมคำว่า static เข้าไป 
    // มันจะทำให้ค่า points ไม่หายไปไหนแม้เราจะเปลี่ยนซีน
    private static int points = 0;

    private void Awake()
    {
        // ทุกครั้งที่โหลดด่านใหม่ ให้สคริปต์ตัวนี้รับหน้าที่เป็น Instance ทันที
        Instance = this;
    }

    private void Start()
    {
        // อัปเดตตัวเลขบนหน้าจอตั้งแต่เริ่มด่าน (มันจะดึงแต้มเดิมที่จำไว้มาแสดง)
        UpdateUI();
    }

    public void IncreasePoints(int amount)
    {
        points += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (pointsText != null)
        {
            pointsText.text = "Points: " + points;
        }
    }

    // [แถม] เอาไว้ใช้ตอนผู้เล่นตาย หรือ กลับหน้าเมนูหลัก เพื่อเคลียร์แต้มเป็น 0
    public void ResetPoints()
    {
        points = 0;
        UpdateUI();
    }
}