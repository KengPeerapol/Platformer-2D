using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int pointsToAdd;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            PointsManager.Instance.IncreasePoints(pointsToAdd);
            Destroy(gameObject); 
        } 
    }
}
