using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Loads a new scene, while also clearing level-specific inventory!*/

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] string _loadSceneName;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == NewPlayer.Instance.gameObject)
        {
            GameManager.Instance.Hud.LoadSceneName = _loadSceneName;
            GameManager.Instance.Inventory.Clear();
            GameManager.Instance.Hud.Animator.SetTrigger("coverScreen");
            enabled = false;
        }
    }
}
