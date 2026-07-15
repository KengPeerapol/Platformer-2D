using UnityEngine;
using TMPro;

public class PointsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text pointsText;

    public static PointsManager Instance;

    private int points = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        pointsText.text = "Points: " + points;
    }

    public void IncreasePoints(int amount)
    {
        points += amount;
        pointsText.text = "Points: " + points;
    }
}
