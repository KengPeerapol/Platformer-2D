using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public GameObject wideCamera;

    void Start()
    {
        if (wideCamera != null)
        {
            wideCamera.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (wideCamera != null)
            {
                wideCamera.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (wideCamera != null)
            {
                wideCamera.SetActive(false);
            }
        }
    }
}