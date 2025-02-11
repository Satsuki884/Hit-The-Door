using UnityEngine;

public class Shot : MonoBehaviour
{
    private float infectionRadius;
    private float power;

    public void SetInfectionRadius(float radius)
    {
        infectionRadius = radius;
    }

    public void SetPower(float powerLevel)
    {
        power = powerLevel;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Goal") || collision.gameObject.CompareTag("Door"))
        {
            Destroy(gameObject);
            PlayerBall.Instance.TryMoveToGoal();
        }
        if (collision.CompareTag("Obstacle"))
        {
            InfectObstacles();

            Destroy(gameObject);
            PlayerBall.Instance.TryMoveToGoal();
        }
    }
    void InfectObstacles()
    {
        Collider[] obstacles = Physics.OverlapSphere(transform.position, infectionRadius);
        foreach (Collider obstacle in obstacles)
        {
            if (obstacle.CompareTag("Obstacle"))
            {
                Destroy(obstacle.gameObject);
            }
        }
    }

}