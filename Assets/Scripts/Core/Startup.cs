using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Simple script telling Unity to Destroy this if it finds an identical one in the scene!*/

public class Startup : MonoBehaviour
{
    public bool DontDestroyThisObjectOnLoad = false;

    // Use this for initialization
    void Awake()
    {
        if (DontDestroyThisObjectOnLoad)
            DontDestroyOnLoad(gameObject);

        if (GameObject.Find("Startup") != null && GameObject.Find("Startup").CompareTag("Startup"))
            Destroy(gameObject);
    }
}
