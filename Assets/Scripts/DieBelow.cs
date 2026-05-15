using UnityEngine;

public class DieBelow : MonoBehaviour 
{
    [SerializeField] float minY;


    void FixedUpdate()
    {
        if (transform.position.y < minY) transform.gameObject.SetActive(false);
    }
}
