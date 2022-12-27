using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private bool _isTaken = false;
    public bool IsTaken { get { return _isTaken; } }

    public bool TrySpawn(GameObject gameObject)
    {
        if (!_isTaken)
        {
            Instantiate(gameObject, transform.localPosition, transform.rotation);
            return _isTaken = true;
        }
        else
        {
            return false;
        }
    }
}
