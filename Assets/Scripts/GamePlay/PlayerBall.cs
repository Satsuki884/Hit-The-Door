using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{
    public static PlayerBall Instance { get; private set; }
    [SerializeField] private GameObject shotPrefab;
    [SerializeField] private Transform shotSpawn;
    [SerializeField] private float shotChargeRate = 0.5f;
    [SerializeField] private float shotSpeed = 10f;
    [SerializeField] private float minPlayerSize = 0.2f;
    [SerializeField] private float _minPlayerSizeForLose = 0.3f;
    [SerializeField] private float infectionRadiusFactor = 2f;
    private float currentCharge = 0f;
    private Vector3 initialScale;
    private Vector3 _initialPosition;
    [SerializeField] private GameObject goal;
    [SerializeField] private float moveSpeed = 5f;
    private GameObject shotPreview;
    private List<Collider> highlightedObstacles = new List<Collider>();
    private bool _isGameOver = false;
    private bool isStopped = false;
    private GameObject obstacle;

    [SerializeField] private BoxCollider roadCollider;
    [SerializeField] private Vector3 roadInitialScale;
    [SerializeField] private float roadScaleFactor = 1.0f;
    public bool isGameOver
    {
        get => _isGameOver;
        set => _isGameOver = value;
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        initialScale = transform.localScale;
        _initialPosition = transform.position;

        if (roadCollider != null)
        {
            roadInitialScale = roadCollider.transform.localScale;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ChargeShot();


        }
        if (Input.GetMouseButtonUp(0))
        {
            RemoveHighlight();
            FireShot();
        }

        ConstrainMovementToRoad();

        if (roadCollider != null)
        {
            float newScaleXandZ = transform.localScale.x * roadScaleFactor;
            roadCollider.transform.localScale = new Vector3(newScaleXandZ, roadCollider.transform.localScale.y, newScaleXandZ);
        }

        if (isStopped && obstacle == null)
        {
            isStopped = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }

    void ConstrainMovementToRoad()
    {
        if (roadCollider != null)
        {
            Bounds roadBounds = roadCollider.bounds;
            Vector3 position = transform.position;

            position.x = Mathf.Clamp(position.x, roadBounds.min.x, roadBounds.max.x);
            position.z = Mathf.Clamp(position.z, roadBounds.min.z, roadBounds.max.z);
            transform.position = position;
        }
    }

    void ChargeShot()
    {
        if (transform.localScale.x > minPlayerSize)
        {
            currentCharge += shotChargeRate * Time.deltaTime;
            transform.localScale -= Vector3.one * shotChargeRate * Time.deltaTime;

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            if (shotPreview == null)
            {
                shotPreview = Instantiate(shotPrefab, mouseWorldPosition, Quaternion.identity);
                shotPreview.GetComponent<Rigidbody>().isKinematic = true;
                shotPreview.GetComponent<Collider>().enabled = false;
            }

            shotPreview.transform.position = mouseWorldPosition;
            shotPreview.transform.localScale = Vector3.one * currentCharge;

            RemoveHighlight();
            
            HighlightObjectsInRadius(shotPreview.transform.position, currentCharge * infectionRadiusFactor);

            if (transform.localScale.x <= _minPlayerSizeForLose)
            {
                GameOver("You Lose.\nYou are very small");
            }
        }
    }

    void FireShot()
    {
        if (currentCharge > 0)
        {
            Vector3 targetPosition = shotPreview.transform.position;

            GameObject shot = Instantiate(shotPrefab, shotSpawn.position, Quaternion.identity);
            shot.transform.localScale = Vector3.one * currentCharge;

            Vector3 direction = (targetPosition - shotSpawn.position).normalized;
            shot.GetComponent<Rigidbody>().velocity = direction * shotSpeed;
            shot.GetComponent<Shot>().SetInfectionRadius(currentCharge * infectionRadiusFactor);
            shot.GetComponent<Collider>().enabled = true;

            Destroy(shotPreview);
            currentCharge = 0;
        }
    }

    void HighlightObjectsInRadius(Vector3 position, float radius)
    {
        Collider[] obstacles = Physics.OverlapSphere(position, radius);

        foreach (var obstacle in obstacles)
        {
            if (obstacle.CompareTag("Obstacle"))
            {
                highlightedObstacles.Add(obstacle);
                Renderer renderer = obstacle.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                }
            }
        }
    }

    void RemoveHighlight()
    {
        foreach (var obstacle in highlightedObstacles)
        {
            if (obstacle != null)
            {
                Renderer renderer = obstacle.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.green;
                }
            }
        }
        highlightedObstacles.Clear();
    }

    public void Restart()
    {
        _isGameOver = false;
        transform.localScale = initialScale;
        transform.position = _initialPosition;
        currentCharge = 0f;

        if (shotPreview != null)
        {
            Destroy(shotPreview);
        }

        Time.timeScale = 1f;
    }

    void GameOver(string message)
    {
        if (!_isGameOver)
        {
            _isGameOver = true;
            GameManager.Instance.GameOver(message);
            StopAllCoroutines();
            Time.timeScale = 0f;
        }
    }

    public void TryMoveToGoal()
    {
        StartCoroutine(MoveToGoal(goal.transform.position));
    }

    private IEnumerator MoveToGoal(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f && !isStopped)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isStopped = true;
            obstacle = collision.gameObject;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Goal")||collision.gameObject.CompareTag("Door"))
        {
            // Debug.Log("Trigger with goal");
            GameOver("You Win!");
        }
    }
}
