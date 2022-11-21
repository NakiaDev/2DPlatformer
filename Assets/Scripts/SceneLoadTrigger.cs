using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] string loadSceneString;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == NewPlayer.Instance.gameObject)
        {
            SceneManager.LoadScene(loadSceneString);
            NewPlayer.Instance.SetSpawnLocation();
        }
    }
}
