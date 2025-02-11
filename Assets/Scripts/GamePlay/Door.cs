using UnityEngine;

public class Door : MonoBehaviour
{

    public static Door Instance{ get; private set; }
    [SerializeField]private GameObject door;
    [SerializeField] private Transform player;
    [SerializeField] private float openDistance = 1.5f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        door.SetActive(true);
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) < openDistance)
        {
            door.SetActive(false);
        }
    }

    public void Restart()
    {
        door.SetActive(true);
    }
}
