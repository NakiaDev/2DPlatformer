using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] string loadSceneString;
    [SerializeField] bool destroyPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (NewPlayer.Instance != null && collision.gameObject == NewPlayer.Instance.gameObject)
        {
            SceneManager.LoadScene(loadSceneString);

            if (destroyPlayer)
            {
                Destroy(NewPlayer.Instance);
                Destroy(GameManager.Instance);
            }
            else
                NewPlayer.Instance.SetSpawnLocation();
        }
    }
}
