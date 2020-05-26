using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectEnabler : MonoBehaviour
{

    private void Awake()
    {
        int count = transform.childCount;

        int index = Random.Range(0, count);
        for(int i = 0; i < count; i ++)
        {
            var t = transform.GetChild(i);
            t.gameObject.SetActive(i == index);
        }
    }
}
