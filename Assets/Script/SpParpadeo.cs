using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpParpadeo : MonoBehaviour
{

    SpriteRenderer sp;
    bool parpadeando;
    public void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
    }
    public void IniciarParpadeo()
    {
        StartCoroutine(Parpadeo());

    }

    IEnumerator Parpadeo()
    {
        parpadeando = true;
        sp.enabled = true;
        yield return new WaitForSeconds(0.02f);  // era 0.07
        sp.enabled = false;
        yield return new WaitForSeconds(0.02f);
        sp.enabled = true;
        yield return new WaitForSeconds(0.02f);
        sp.enabled = false;
        yield return new WaitForSeconds(0.02f);
        sp.enabled = true;
        yield return new WaitForSeconds(0.02f); //new
        sp.enabled = false;
        yield return new WaitForSeconds(0.02f); // new 
        sp.enabled = true;

        yield return new WaitForSeconds(0.14f); // era 0.14
        parpadeando = false;
        StartCoroutine(Parpadeo());

    }

    public void QuitarImagen()
    {
        sp.enabled = false;
      //  sombra.SetActive(false);
    }
}
