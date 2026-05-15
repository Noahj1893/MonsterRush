using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] Boss bossScript;

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            bossScript.enabled = true;
        }
    }
}
