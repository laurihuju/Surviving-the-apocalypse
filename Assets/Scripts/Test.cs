using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Terrain terrain;

    void Start()
    {
        Debug.Log(gameObject.name);
        StartCoroutine(Testi());
    }

    private IEnumerator Testi()
    {
        yield return new WaitForSeconds(10);
        Debug.Log(gameObject.name);
    }
}
