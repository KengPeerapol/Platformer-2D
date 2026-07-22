using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                // ส่งตัว player ไปให้ LivesManager จัดการลดชีวิตและวาร์ป
                if (LivesManager.Instance != null)
                {
                    LivesManager.Instance.LoseLife(player);
                }
            }
        }
    }
}