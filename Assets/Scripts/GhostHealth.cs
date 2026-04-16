using UnityEngine;

public class GhostHealth : MonoBehaviour
{
    [SerializeField] int hitsToDefeat = 1;

    int hitsTaken;

    public void TakeHit(int damage)
    {
        if (damage <= 0 || !gameObject.activeInHierarchy)
            return;

        hitsTaken += damage;
        if (hitsTaken >= hitsToDefeat)
            gameObject.SetActive(false);
    }
}
