using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMenu : MonoBehaviour
{
    public float tiempoPrevioInput = 3;
    public GameObject fadeIn;
    bool presionada = true;
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke("PuedoPresionar", tiempoPrevioInput);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !presionada)
        {
            presionada = true;
            fadeIn.SetActive(true);
            Invoke("CargarNivel", 1.5f);
        }
    }
    public void PresionadoTactil()
    {
        if (presionada==false)
        {
            presionada = true;
            fadeIn.SetActive(true);
            Invoke("CargarNivel", 1.5f);
        }
    }

    public void PuedoPresionar()
    {
        presionada = false;
    }

    public void CargarNivel()
    {
        SceneManager.LoadScene(1);
    }
    
}
