using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // เช็กว่าคนที่เดินมาชนมี Tag เป็น "Player" ไหม
        if (collision.CompareTag("Player"))
        {
            // ดึงสคริปต์ PlayerController จากตัว Player
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                // ส่งพิกัดของเสา Checkpoint นี้ ไปบันทึกเป็นจุดเกิดใหม่ในสคริปต์ Player
                player.SetSpawnPosition(transform.position);

                // แสดงข้อความใน Console เพื่อให้เรารู้ว่าระบบเซฟทำงานแล้ว
                Debug.Log("Checkpoint Saved!");
            }
        }
    }
}