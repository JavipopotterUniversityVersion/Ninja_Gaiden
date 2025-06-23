using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Enemigo : MonoBehaviour  //0.6f es distancia vertical
{
    public float speed = 3;
    public float vida = 20;
   public float vidaMax;
    int congelar = 1;

    float speedOriginal;
    float vidaInicial;
    float escalaOriginal;
    bool ataqueIniciado;
    bool puedoSerGolpeado = true;
    bool puedoAtacar = true;
    bool puedoGirarme = true;

    //Instancias
    public GameObject ataqueMano;
    public GameObject respawnAtaque;
    public AudioSource audioAtacado1;
    public AudioSource audioAtacado2;
    public AudioSource audioLevantarse;
    public AudioSource audioMuerte;

    //Componentes
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sp;
    Player player;
    public Image barraVida;
    public GameObject barraVidaBlancaAsociada;
    public GameObject respawnFx;
    public GameObject [] fxSangreDiente;

    //Sombra
    public GameObject sombra;
    Vector3 posicionSombra;
    Vector3 posicionLocalOriginalSombra;
    bool excepcionPorAtacadoSaltando;
    bool atacadoEnAire;

    //Trucos
    public bool invencible;
    public float vidaPreInvencible;
    Vector3 posicionOriginal;
    private void Awake()
    {
        speedOriginal = speed;
        vidaMax = vida;
        posicionLocalOriginalSombra = sombra.transform.localPosition;
        posicionOriginal = transform.position;

    }
    void Start()
    {
        barraVidaBlancaAsociada.SetActive(true);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        vidaInicial = vida;
        player = GameObject.FindObjectOfType<Player>();
        escalaOriginal = transform.localScale.x;

    }

    void Update()
    {
        if (invencible)
        {
            vida = vidaPreInvencible+2;
        }

        if(player.vida<=0)
        {
            sombra.transform.localPosition = posicionLocalOriginalSombra;

            return;
        }

        if (vivo)
        {
            MirarPlayer();
            BuscarPlayer();
            ActualizarInterfaz();
            RecibirCombo();
        }

        //sombra

        if (posicionSombra != Vector3.zero) //Existe el vector, o estoy saltando
        {
            if (excepcionPorAtacadoSaltando == false)
            {
                sombra.transform.position = new Vector3(transform.position.x, posicionSombra.y, 0);

            }
            else
            {
                sombra.transform.localPosition = Vector3.MoveTowards(sombra.transform.localPosition, posicionLocalOriginalSombra, Time.deltaTime * 2);
            }
        }
        else
        {
            sombra.transform.localPosition = posicionLocalOriginalSombra;
        }
        //fin sombra

        ComprobarMuerte();


        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.6f, 0.4f), transform.position.z);


    }
    void BuscarPlayer()
    {
        if (vida>0)
        {
            float distanciaPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (ataqueIniciado == false)
            {
                if (distanciaPlayer > 1 && congelar!=0)
                {
                 //   anim.SetFloat("Move", 1);


                    AnimatorStateInfo clip = anim.GetCurrentAnimatorStateInfo(0);
                    if (clip.IsName("AtacadoFuerteDer"))
                    {

                    }
                    else if (player.parpadeando==false)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, congelar * speed * Time.deltaTime);
                        anim.SetFloat("Move", 1);

                    }
                    else
                    {
                        anim.SetFloat("Move", 0);

                    }

                    float verticalDistance = Mathf.Abs(transform.position.y - player.transform.position.y);

                    if (verticalDistance>.85f)
                    {
                        if (player.transform.position.y < transform.position.y)
                        {
                            anim.SetInteger("verticalDir", -1);
                        }
                        else if (player.transform.position.y > transform.position.y)
                        {
                            anim.SetInteger("verticalDir", 1);
                        }
                    }
                    else
                    {
                        anim.SetInteger("verticalDir", 0);
                    }

                }
                else if (congelar!=0)
                {
                    anim.SetFloat("Move", 0);
                    ataqueIniciado = true;

                    if (puedoAtacar)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, player.transform.position.y, transform.position.z), Time.deltaTime);
                        StartCoroutine(AtaqueMano());
                    }
                }
            }
        }
    }
    void ActualizarInterfaz()
    {
        vida = Mathf.Clamp(vida, 0, 500);
        barraVida.fillAmount = vida / vidaInicial;
    }
    bool vivo = true;
    void ComprobarMuerte()
    {
        if (vida <= 0 && vivo)
        {
            vivo = false;
            anim.Play("Muerte");
            speed = 0;
            Destroy(barraVidaBlancaAsociada);
            gameObject.tag = "Untagged";
            Destroy(gameObject, 25f);
        }
    }
    int direction;
    void MirarPlayer()
    {
        if (player.parpadeando == false)
        {
            if (player.transform.position.x > transform.position.x && puedoGirarme)
            {
                direction = 1;
            }
            else if (puedoGirarme)
            {
                direction = -1;
            }


            transform.localScale = new Vector3(escalaOriginal * direction, escalaOriginal, 1);
        }
    }
   
    IEnumerator AtaqueMano()
    {
        GameObject ataque = Instantiate(ataqueMano, respawnAtaque.transform.position, Quaternion.identity);
        ataque.GetComponent<ModificarTag>().padre = transform;

        
        if (player.transform.localScale.x == transform.localScale.x)
            {
                ataque.gameObject.tag = "GolpeFuerte";
                anim.SetBool("Combo", true);
            }
        else
        {

                if (player.golpesRecibidos >= 2)
                {
                    
                    
                    ataque.gameObject.tag = "GolpeFuerte";
                    anim.SetBool("Combo", true);
                }
                else
                {

                    anim.SetBool("Combo", false);

                }
        }
        
        anim.Play("PunchEnemy");
     
     
     
        yield return new WaitForSeconds(0.4f);
        if (puedoSerGolpeado)
        {
            ataqueIniciado = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (vivo)
        {
            if (collision.tag == "GolpePlayer" && puedoSerGolpeado)
            {
                Atacado(1);
                Destroy(collision.gameObject);
            }else if (collision.tag =="GolpePlayer" && puedoSerGolpeado==false)
            {
               /* int randomValorAudio = Random.Range(0, 10);
                    if (randomValorAudio < 7)
                    {
                     //   audioAtacado1.Play();
                    }*/
                
            }
            if (collision.tag == "GolpePlayerFuerte") 
            {
                Atacado(2);
                Destroy(collision.gameObject);
            }

        }


    }

    public void SangreDientes()
    {
        float probabilidad = 0;

        probabilidad = (barraVida.fillAmount * 100);  //  0   y   100.  La probabilidad es "inversa"

        if (Random.Range(0, 100) >= probabilidad - 15)
        {
           GameObject instancia=  Instantiate(fxSangreDiente[Random.Range(0, fxSangreDiente.Length)], respawnFx.transform.position, Quaternion.identity);
            instancia.GetComponent<RandomizarAnimStart>().padreInstanciador = gameObject;
        }
    }
    public void InstanciaSangreExterna()
    {
      //  GameObject instancia = Instantiate(fxSangreDiente[Random.Range(0, fxSangreDiente.Length)], respawnFx.transform.position, Quaternion.identity);
      //  instancia.GetComponent<RandomizarAnimStart>().padreInstanciador = gameObject;
    }
    public void Atacado(int vidaPerdida)
    {
        vida -= vidaPerdida;
        speed = 0;
        puedoAtacar = false;
        puedoSerGolpeado = false;
        puedoGirarme = false;

        golpesRecibidos += 1;
        tiempoUltimoAtaque = Time.time;

        if (vivo)
        {
            if (vidaPerdida == 1)
            {
                SangreDientes();
                audioAtacado1.Play();
                anim.Play("AtacadoEnemy");
                if (corrutinaPrevia != null)
                {
                    StopCoroutine(corrutinaPrevia);
                }
                corrutinaPrevia =  StartCoroutine(RestablecerAtacado(0.2f)); //era solo start esta
            }
            else
            {
                golpesRecibidos = 0;
                audioAtacado2.Play();
                anim.Play("AtacadoFuerteDer");
                if (corrutinaPrevia != null)
                {
                    StopCoroutine(corrutinaPrevia);
                }
                corrutinaPrevia = StartCoroutine(RestablecerAtacado(0.65f));
            }
        }

        /*  if (vidaPerdida == 1)
          {
              audioAtacado1.Play();
              anim.Play("AtacadoEnemy");
              StartCoroutine(RestablecerAtacado(0.2f));
          }
          else
          {
              audioAtacado2.Play();
              anim.Play("AtacadoFuerteDer");
              if (corrutinaPrevia!=null)
              {
                  StopCoroutine(corrutinaPrevia);
              }
             corrutinaPrevia = StartCoroutine(RestablecerAtacado(0.65f));
          }*/
    }
    bool puedoMoverme = true;
    Coroutine corrutinaPrevia;

    IEnumerator RestablecerAtacado(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        speed = speedOriginal;
        puedoGirarme = true;
        posicionSombra = Vector3.zero;

        yield return new WaitForSeconds(0.1f); // era 0.3
        puedoAtacar = true;
        ataqueIniciado = false;

        yield return new WaitForSeconds(0.1f); // era 0.3
      //  speed = speedOriginal;//new
    //    puedoGirarme = true; //new

        puedoSerGolpeado = true;
       // puedoAtacar = true; // new



    }

    public void AudioLevantarse()
    {
        audioLevantarse.Play();
    }
    public void AudioMuerte()
    {
        audioMuerte.Play();
    }

    float tiempoUltimoAtaque;
    public int golpesRecibidos = 0;

    void RecibirCombo()
    {
        if (Time.timeScale >= 1)
        {
            if (Time.time - tiempoUltimoAtaque > 1.25f)
            {
                golpesRecibidos = 0; // Reinicia el contador si han pasado más de 1 segundo
                //Debug.Log("Restablecido");

            }


        }

    }

    public void Congelar()
    {
        congelar = 0;
        StartCoroutine(ForzarDescongelado());
    }
    public void Descongelar()
    {
        congelar = 1;
    }

    IEnumerator ForzarDescongelado()
    {
        yield return new WaitForSeconds(1.35f);
        if (congelar==0)
        {
            congelar = 1;
            Debug.LogError("Descongelado forzado");
        }
    }

    public void IniciarParpadeo()
    {
        StartCoroutine(Parpadeo());

    }

    IEnumerator Parpadeo()
    {
        sp.enabled = true;
        yield return new WaitForSeconds(0.03f);  // era 0.07
        sp.enabled = false;
        yield return new WaitForSeconds(0.03f);
        sp.enabled = true;
        yield return new WaitForSeconds(0.03f);
        sp.enabled = false;
        yield return new WaitForSeconds(0.03f);
        sp.enabled = true;
        yield return new WaitForSeconds(0.03f); //new
        sp.enabled = false;
        yield return new WaitForSeconds(0.03f); // new 
        sp.enabled = true;
        yield return new WaitForSeconds(0.03f); //new
        sp.enabled = false;
        yield return new WaitForSeconds(0.03f); // new 
        sp.enabled = true;
        yield return new WaitForSeconds(0.03f); //new
        sp.enabled = false;
        yield return new WaitForSeconds(0.03f); // new 
        sp.enabled = true;
        yield return new WaitForSeconds(0.14f); // era 0.14
        Destroy(gameObject);
    }


    public void PosicionarSombra()
    {
        if (atacadoEnAire)
        {
            atacadoEnAire = false;
            sombra.transform.position += new Vector3(0, 0.2f, 0);
            excepcionPorAtacadoSaltando = true;
        }
        // sombra.transform.localPosition = posicionLocalOriginalSombra;
    }

    public void Reiniciar()
    {
        this.enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        sombra.SetActive(true);
        anim.Play("Idleenemy");
        vida = vidaMax;
        transform.position = posicionOriginal;
        GetComponent<EneLanzado>().enabled = false;
        transform.localScale = new Vector3(5, 5, 1);
    }

}
