using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Instantiates (creates) one or more objects*/

public class Instantiator : MonoBehaviour
{
    [SerializeField] GameObject[] _objects;
    [SerializeField] float _amount; //Amount can only be used if objects is one item

    public void InstantiateObjects()
    {
        //Instantiate the entire array of objects
        if (_amount == 0)
        {
            for (int i = 0; i < _objects.Length; i++)
            {
                GameObject iObject = Instantiate(_objects[i], transform.position, Quaternion.identity, null);
                if (iObject.TryGetComponent(out Ejector ejector))
                    ejector.LaunchOnStart = true;
            }
        }
        //Instantiate a specific amount of the first object in the array
        else if (_objects.Length != 0)
        {
            for (int i = 0; i < _amount; i++)
            {
                GameObject iObject = Instantiate(_objects[0], transform.position, Quaternion.identity, null);
                if (iObject.TryGetComponent(out Ejector ejector))
                    ejector.LaunchOnStart = true;
            }
        }
    }
}
