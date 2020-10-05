using UnityEngine;

public class BrokenProp : MonoBehaviour
{
    public Kid brokenBy;

    [SerializeField] private float timeToClean = 5f;
    private LevelManager lm;
    private PlayerClass pc;
    public int Value { get; set; }

    private void Start()
    {
        pc = FindObjectOfType<PlayerClass>();
        lm = FindObjectOfType<LevelManager>();
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
            lm.AddDamageCashValue((int) (-Value * 0.5f));
            Destroy(gameObject);
        }
    }
}