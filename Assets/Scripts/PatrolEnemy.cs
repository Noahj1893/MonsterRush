using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [SerializeField] float leftX;
    [SerializeField] float rightX;
    [SerializeField] float speed;
    int dir = 1;


    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.left * dir);
    }
    void LateUpdate()
    {
        if(transform.position.x < leftX)
        {
            transform.position = new Vector2(leftX, transform.position.y);
            dir *= -1;
        }
        else if (transform.position.x > rightX)
        {
            transform.position = new Vector2(rightX, transform.position.y);
            dir *= -1;
        }
    }
    public void setPatrol(float left, float right)
    {
        leftX = left;
        rightX = right;
    }
}
