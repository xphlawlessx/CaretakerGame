using System;
using Environment;
using UnityEngine;

public class DestructableProp : MonoBehaviour
{
    [SerializeField] private GameObject _brokenVersion;
    private int hp = 1;
    public RoomObjective room;
    [SerializeField] private int Value = 500;

    private void Start()
    {
        room = GetComponentInParent<RoomObjective>();
        var lm = FindObjectOfType<LevelManager>();
        OnDestroyed += lm.AddDamageCashValue;
    }

    private event Action<int> OnDestroyed;

    public int GetHit(Kid by)
    {
        hp -= 1;
        Debug.Log(hp);
        if (hp <= 0)
        {
            var prop = Instantiate(_brokenVersion, transform.position, transform.rotation);
            var broken = prop.GetComponent<BrokenProp>();
            if (broken != null)
            {
                broken.brokenBy = by;
                broken.Value = Value;
            }

            OnDestroyed?.Invoke(Value);
            Destroy(gameObject);
            if (room == null) return 0;
            var num = room.DestroyProp(this);
            return num;
        }

        return -1;
    }
}