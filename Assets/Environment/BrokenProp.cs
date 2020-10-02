using UnityEngine;

public class BrokenProp : MonoBehaviour
{
    public Kid brokenBy;

    [SerializeField] private float timeToClean = 5f;

    private PlayerClass pc;

    private void Start()
    {
        pc = FindObjectOfType<PlayerClass>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) Clean();
    }

    public void Clean()
    {
        timeToClean -= Time.deltaTime;
        if (timeToClean <= 0)
        {
            if (pc.SelectedClass == PlayerClass.CaretakerClass.Detective) brokenBy.Track();
            Destroy(gameObject);
        }
    }
}