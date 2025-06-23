using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class ControladorPartida : MonoBehaviour
{
    Player player;
    public Enemigo enemy;
    public GameObject canvasFinal;
    public GameObject canvasPausa;
    public TextMeshProUGUI textWinLose;
    bool finalizar = false;
    float tiempo = 99;
    public TextMeshProUGUI timeText;
    public AudioSource audioMenu;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.vida<=0 ||enemy.vida<=0)
        {
            if (!finalizar)
            {
                finalizar = true;
                StartCoroutine(CanvasFinal());
            }
        }
        else
        {
            Pausar();
            tiempo -= Time.deltaTime/2;
            timeText.text = "" + (int)tiempo;

            if (tiempo<=0)
            {
                player.vida = 0;
                tiempo = 0;
            }
        }

       
    }

    public void PausaTactil()
    {
        if (Time.timeScale == 1)
        {
            canvasPausa.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            canvasPausa.SetActive(false);
            Time.timeScale = 1;
        }
        audioMenu.Play();
    }
    public void Pausar()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !finalizar)
        {
            if (Time.timeScale == 1)
            {
                canvasPausa.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                canvasPausa.SetActive(false);
                Time.timeScale = 1;
            }
            audioMenu.Play();
        }
    }

    public IEnumerator CanvasFinal()
    {
        yield return new WaitForSeconds(4f);
        if (player.vida>0)
        {
            textWinLose.color = Color.green;
            textWinLose.text = "YOU WIN";
        }
        else
        {
            textWinLose.color = Color.red;
            textWinLose.text = "YOU LOSE";
        }
        canvasFinal.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

  
}
