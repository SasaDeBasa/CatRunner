using UnityEngine;
using TMPro;

public class LeaderboardEntry : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI scoreText;

    public void Setup(int rank, string username, int score)
    {
        rankText.text = rank.ToString();
        usernameText.text = username;
        scoreText.text = score.ToString();
    }
}
