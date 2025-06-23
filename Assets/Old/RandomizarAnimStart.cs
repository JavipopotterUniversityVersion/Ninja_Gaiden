using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizarAnimStart : MonoBehaviour
{
    public GameObject padreInstanciador;
    private void Start()
    {
        GetComponent<Animator>().SetInteger("random", Random.Range(1, 5));


        if (padreInstanciador.transform.localScale.x > 0)
        {

        }
        else
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
