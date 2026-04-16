using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGameOnPlayerTouch : MonoBehaviour
{
    static bool loadInProgress;

    void Awake()
    {
        loadInProgress = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (loadInProgress)
            return;

        Transform t = collision.collider.transform;
        while (t != null)
        {
            if (t.CompareTag("Player"))
            {
                loadInProgress = true;
                var scene = SceneManager.GetActiveScene();
                if (scene.buildIndex >= 0)
                    SceneManager.LoadScene(scene.buildIndex);
                else
                    SceneManager.LoadScene(scene.name);
                return;
            }

            t = t.parent;
        }
    }
}
