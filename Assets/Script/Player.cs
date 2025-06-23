using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float Speed { get { return speed; } set { speed = value; } }
    public float AirSpeed { get { return _airSpeed; } set { _airSpeed = value; } }
    //Variables player
    public bool android = true;
    public float modificadorTiempoSaltoGeneral = 1;

    public float _airSpeed = 1.65f;
    public float speed = 2;
    public float vida = 40;
    public float porcentajeMinimo = 0.75f;
    public float fuerzaSalto = 2;
    float fuerzaSaltoOriginal;
    public int bombasHumo = 20;
    public bool parpadeando = false;
    public float fuerzaKnock;
    public float tiempoSalto = 0;
    //Variables ocultas player
    float speedOriginal;
    float modificadorSpeed = 1;
    public float posicionYPreviaSalto;
    bool puedoGirarme = true;
    bool puedoSaltar = true;
    bool ejecutandoSalto;
    bool excepcionPorAtacadoSaltando = false;
    bool atacadoEnAire;
    public float vidaInicial;
    bool puedoAtacar = true;
    public bool puedoSerAtacado = true;
    bool knockBack = false;
    float tiempoKnock;
    int knockBackdirection;
    bool vivo = true;
    bool enCartel;
    public bool tomandoCartel;
    float tiempoUltimoAtaque;
    public int golpesRecibidos = 0;
    public Vector2 inputSalto = new Vector2(0, 0); // era 0, 1     04052024 Flauros
    GameObject cartelColisionado; //Para reubicar con cartel
    public Joystick joy;

    //Instancias
    public GameObject ataqueMano;
    public GameObject ataqueSenal;
    public GameObject respawnAtaque;
    public GameObject[] fxSangreDiente;

    //Armas
    bool tengoCartel = false;

    //Componentes
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sp;
    public Image barraVida;
    Coroutine corrutinaAParar;
    public TextMeshProUGUI textBombas;
    public AudioSource audioSalto;
    public AudioSource audioLevantarse;
    public AudioSource audioAtacado;
    public AudioSource audioMuerte;
    public AudioSource audioBombaHumo;

    public RuntimeAnimatorController controllerBasico;
    public RuntimeAnimatorController controllerCartel;

    //Objetos
    public GameObject sombra;
    Vector3 posicionSombra;
    Vector3 posicionLocalOriginalSombra;

    public GameObject respawnFx;

    //Trucos
    public bool invencible;
    public float vidaPreInvencible;

    // Start is called before the first frame update
    void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        vidaInicial = vida;
        speedOriginal = speed;
        fuerzaSaltoOriginal = fuerzaSalto;
        posicionLocalOriginalSombra = sombra.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (invencible)
        {
            vida = vidaPreInvencible + 2;
        }
        if (vivo && Time.timeScale == 1)
        {
            InputPlayer();
            RecibirCombo();
        }
        if (vivo == false)
        {
            sombra.transform.localPosition = posicionLocalOriginalSombra;
        }

        if (cartelColisionado != null)
        {
            float distanciaCartel;
            distanciaCartel = Mathf.Abs(transform.position.x - cartelColisionado.transform.position.x);
            //  Debug.Log("Distancia" + distanciaCartel);
        }
        ActualizarInterfaz();  //Limite vida y barra

        ComprobarMuerte(); // Scene 0

    }


    private void FixedUpdate()
    {
        if (vivo && Time.timeScale == 1)
        {
            Salto();

            if (knockBack == true)
            {
                KnockBack();
            }
        }
        else if (vivo == false)
        {
            if (knockBack == true)
            {
                KnockBack();
            }
        }
    }

    void CambioAnimator()
    {
        cameraSigueme = true;
        if (tengoCartel)
        {
            anim.runtimeAnimatorController = controllerCartel;
        }
        else
        {
            anim.runtimeAnimatorController = controllerBasico;
        }
        speed = speedOriginal;
        puedoSerAtacado = true;
        tomandoCartel = false;
        puedoAtacar = true;
    }

    void InputPlayer()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal") + joy.Horizontal;
        float verticalInput = Input.GetAxisRaw("Vertical") + joy.Vertical;
        print("Vertical Input: " + verticalInput + " Horizontal Input: " + horizontalInput);
        Vector2 inputDirection = new Vector2(horizontalInput, verticalInput);


        // Normalizar la direcci�n del input (convertirla en un vector unitario)
        inputDirection.Normalize();



        // Asignar la velocidad normalizada al Rigidbody2D
        if (knockBack == false && ejecutandoSalto == false)
        {
            //movimiento
            rb.linearVelocity = inputDirection * speed * modificadorSpeed;
            tiempoKnock = 0;
        }
        else if (knockBack == false && ejecutandoSalto == true)
        {
            if (rb.linearVelocityX != 0) rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocityX) * modificadorSpeed * speed * _airSpeed, inputSalto.y * fuerzaSalto); // horiozntalinput * speed * modificador (original en x)
            else rb.linearVelocity = new Vector2(rb.linearVelocityX, inputSalto.y * fuerzaSalto);

            /* if (verticalInput == 0) //Sin Y, salto auto.
             {
                 rb.velocity = new Vector2(inputDirection.x * speed * modificadorSpeed, inputSalto.y * modificadorSpeed) ; // (inputDirection * speed) + inputSalto;
             }else
             {
                 rb.velocity = new Vector2(horizontalInput, verticalInput) * speed * modificadorSpeed;
             }*/

        }


        //limite
        if (ejecutandoSalto == false && knockBack == false)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.6f, 0.4f), transform.position.z);
        }
        else if (tiempoSalto > 0.25 || tiempoKnock > 0.25f) //tiempo es tiempo knock
        {
            //  transform.position = new Vector3(transform.position.x,Mathf.MoveTowards(transform.position.y, Mathf.Clamp(transform.position.y, -3.6f, 0.4f), Time.deltaTime));

        }

        //Sombra


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



        if (android == false)
        {
            if (Input.GetKeyDown(KeyCode.L) && puedoAtacar && ejecutandoSalto == false)   //Ataque
            {
                puedoAtacar = false;
                corrutinaAParar = StartCoroutine(Atacar());
            }
            else if (Input.GetKeyDown(KeyCode.L) && puedoAtacar == false && ejecutandoSalto == false)
            {
                float tiempoNormalizado = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

                if (tiempoNormalizado > porcentajeMinimo && stateInfo.IsName("Punch"))
                {
                    StopCoroutine(corrutinaAParar);
                    corrutinaAParar = StartCoroutine(Atacar());
                    puedoAtacar = false;
                    speed = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.L) && puedoAtacar == true && ejecutandoSalto == true && tiempoSalto < .45f)
            {
                puedoAtacar = false;
                StartCoroutine(AtaqueAire());
            }

            if (Input.GetKeyDown(KeyCode.Space) && puedoSaltar == true && puedoAtacar)
            {
                AnimatorStateInfo clipAnim = anim.GetCurrentAnimatorStateInfo(0);

                if (!clipAnim.IsName("Salto2"))
                {
                    puedoSaltar = false;
                    posicionYPreviaSalto = transform.position.y;
                    ejecutandoSalto = true;
                }
                else
                {
                    Debug.LogError("Salto impedido desde salto");
                }
            }

            if (Input.GetKey(KeyCode.LeftShift) && ejecutandoSalto == false)
            {
                modificadorSpeed = 1.5f;
            }
            else if (ejecutandoSalto == false)
            {
                modificadorSpeed = 1;
            }
        }

        anim.SetFloat("Move", Mathf.Abs(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput))); //Anim movimiento
        anim.SetFloat("ModificadorSpeed", modificadorSpeed);


        if (horizontalInput > 0 && puedoGirarme && ejecutandoSalto == false)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
        else if (horizontalInput < 0 && puedoGirarme && ejecutandoSalto == false)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
    }
    void ActualizarInterfaz()
    {
        textBombas.text = "x" + bombasHumo;
        vida = Mathf.Clamp(vida, 0, 500);
        barraVida.fillAmount = vida / vidaInicial;
    }

    void ComprobarMuerte()
    {
        if (vida <= 0 && vivo)
        {
            vivo = false;
            anim.Play("Muerte");
        }
        else if (vivo == false)
        {

        }
    }


    bool ataqueFuerte = false;
    IEnumerator Atacar()
    {

        puedoSerAtacado = false;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (tengoCartel == false)
        {
            if (stateInfo.IsName("Punch"))
            {
                anim.Play("Punch2");
            }
            else if (stateInfo.IsName("Punch2"))
            {
                anim.Play("Punch3");
                ataqueFuerte = true;
            }
            else
            {
                anim.Play("Punch");
            }
        }
        else
        {
            anim.Play("AtaqueSenal");
        }

        Enemigo enemigo = GameObject.FindObjectOfType<Enemigo>(); //Esto ser� el que reciba el golpe.
        if (enemigo != null)
        {
            if (enemigo.golpesRecibidos >= 2 || tengoCartel)
            {
                GameObject instanciaAtaque;
                if (tengoCartel)
                {
                    instanciaAtaque = Instantiate(ataqueSenal, respawnAtaque.transform.position, Quaternion.identity);

                }
                else
                {
                    instanciaAtaque = Instantiate(ataqueMano, respawnAtaque.transform.position, Quaternion.identity);

                }

                instanciaAtaque.tag = "GolpePlayerFuerte";
            }
            else
            {
                GameObject instanciaAtaque = Instantiate(ataqueMano, respawnAtaque.transform.position, Quaternion.identity);
                instanciaAtaque.tag = "GolpePlayer";

            }

        }
        else //Ataque sin enemigos
        {
            GameObject instanciaAtaque = Instantiate(ataqueMano, respawnAtaque.transform.position, Quaternion.identity);
            instanciaAtaque.tag = "GolpePlayer";
        }

        speed = 0;
        /*if (ataqueFuerte == false)
        {
          GameObject instanciaAtaque=  Instantiate(ataqueMano, respawnAtaque.transform.position, Quaternion.identity);
          instanciaAtaque.tag = "GolpePlayer";
        }
        else
        {
            ataqueFuerte = false;
            GameObject instanciaAtaque = Instantiate(ataqueMano, respawnAtaque.transform.position, Quaternion.identity);
            instanciaAtaque.tag = "GolpePlayerFuerte";
        }*/


        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(stateInfo.length * 0.75f);
        Debug.Log("Devuelvo velocidad" + stateInfo.length);
        puedoSerAtacado = true;
        puedoAtacar = true;
        speed = speedOriginal;
    }

    IEnumerator AtaqueAire()
    {
        GameObject instanciaAtaque;
        int randomValor = Random.Range(0, 5);

        if (tengoCartel)
        {
            randomValor = 0;
            instanciaAtaque = Instantiate(ataqueSenal, respawnAtaque.transform.position, Quaternion.identity);

        }
        else
        {
            instanciaAtaque = Instantiate(ataqueSenal, respawnAtaque.transform.position, Quaternion.identity);

        }

        if (randomValor <= 1)
        {
            instanciaAtaque.tag = "GolpePlayerFuerte";
        }
        else
        {
            instanciaAtaque.tag = "GolpePlayer";
        }
        anim.Play("AtaqueSalto");

        yield return new WaitForSeconds(.5f);
        // puedoSerAtacado = true;
        puedoAtacar = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (puedoSerAtacado == false && vivo == true && collision.tag == "Golpe")
        {
            /*   int randomValorAudio = Random.Range(0, 10);
               if (randomValorAudio < 7)
               {
                   audioAtacado.Play();
               }*/
        }

        if (puedoSerAtacado && vivo)
        {
            if (ejecutandoSalto == false)
            {
                if (collision.tag == "Golpe")
                {
                    Atacado(1);
                    if (vida <= 0) //Para q la muerte te empuje
                    {
                        tiempoKnock = 0;
                        Transform padreColisionador;
                        if (collision.gameObject.GetComponent<ModificarTag>() != null)
                        {
                            padreColisionador = collision.gameObject.GetComponent<ModificarTag>().padre;
                            Debug.Log("Padre es: " + padreColisionador.name);
                        }
                        else
                        {
                            padreColisionador = collision.gameObject.transform;
                            Debug.LogError("Falta componente");

                        }

                        if (padreColisionador.position.x > transform.position.x)
                        {
                            knockBackdirection = -1;
                        }
                        else
                        {
                            knockBackdirection = 1;
                        }
                    }
                    Destroy(collision.gameObject);

                }

                if (collision.tag == "GolpeFuerte")
                {
                    Transform padreColisionador;
                    if (collision.gameObject.GetComponent<ModificarTag>() != null)
                    {
                        padreColisionador = collision.gameObject.GetComponent<ModificarTag>().padre;
                        Debug.Log("Padre es: " + padreColisionador.name);
                    }
                    else
                    {
                        padreColisionador = collision.gameObject.transform;
                        Debug.LogError("Falta componente");

                    }

                    if (padreColisionador.position.x > transform.position.x)
                    {
                        knockBackdirection = -1;
                    }
                    else
                    {
                        knockBackdirection = 1;
                    }
                    Atacado(2);
                    Destroy(collision.gameObject);
                }

                if (collision.tag == "Cartel")
                {
                    cartelColisionado = collision.gameObject;
                    enCartel = true;
                }

            }
            else //si me pilla en el aire
            {
                if (collision.tag == "Golpe" || collision.tag == "GolpeFuerte")
                {
                    atacadoEnAire = true;
                    if (collision.gameObject.transform.position.x > transform.position.x)
                    {
                        knockBackdirection = -1;
                    }
                    else
                    {
                        knockBackdirection = 1;
                    }
                    Atacado(2);
                    Destroy(collision.gameObject);


                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Cartel")
        {
            enCartel = false;
        }
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
    public void Atacado(int vidaPerdida)
    {
        vida -= vidaPerdida;
        speed = 0;
        puedoAtacar = false;
        puedoSerAtacado = false;
        puedoGirarme = false;
        ejecutandoSalto = false;
        puedoSaltar = false;
        audioAtacado.Play();
        golpesRecibidos += 1;
        tiempoUltimoAtaque = 1;
        posicionSombra = sombra.transform.position;

        if (vivo && vida > 0)
        {

            if (vidaPerdida == 1)
            {
                SangreDientes();

                StartCoroutine(RetrocesoEstatico());
                anim.Play("AtacadoSimple");
                StartCoroutine(RestablecerAtacado(0.4f));
            }
            else //golpe fuerte
            {
                golpesRecibidos = 0;
                StartCoroutine(SacudidaCamara());
                knockBack = true;

                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    anim.Play("AtacadoFuerte");
                    StartCoroutine(RestablecerAtacado(1));
                }
                else
                {
                    anim.Play("AtacadoFuerteVoltereta");
                    StartCoroutine(RestablecerAtacado(0.65f));
                }
            }
        }
        else if (vivo == false || vida <= 0)
        {
            knockBack = true;
            //    Debug.Log("Knoc deat");
            StartCoroutine(KnockMuerte());
        }

    }

    public void SangreDientes()
    {
        float probabilidad = 0;

        probabilidad = (barraVida.fillAmount * 100);  //  0   y   100.  La probabilidad es "inversa"

        if (Random.Range(0, 100) >= probabilidad - 15)
        {
            GameObject instancia = Instantiate(fxSangreDiente[Random.Range(0, fxSangreDiente.Length)], respawnFx.transform.position, Quaternion.identity);
            instancia.GetComponent<RandomizarAnimStart>().padreInstanciador = gameObject;
        }
    }
    public void ApagarEncenderVibracion()
    {
        VibracionCamera = !VibracionCamera;
        if (VibracionCamera == true)
        {
            estadoCamera.color = Color.green;
            estadoCamera.text = "La camara vibra";
        }
        else
        {
            estadoCamera.color = Color.red;

            estadoCamera.text = "La camara NO vibra";
        }
    }
    public TextMeshProUGUI estadoCamera;
    public bool cameraSigueme = true;
    bool VibracionCamera = true;
    public IEnumerator RetrocesoEstatico() // 0.075f
    {

        if (VibracionCamera == false)
        {
            cameraSigueme = false;

        }
        else
        {
            cameraSigueme = true;
        }



        Vector3 direccionRetroceso = Vector3.zero;
        float valorRetroceso = 0.15f; // era 0.1

        direccionRetroceso.x = valorRetroceso;

        transform.position -= direccionRetroceso;
        yield return new WaitForSeconds(0.05f);
        transform.position += direccionRetroceso * 2;
        yield return new WaitForSeconds(0.05f);
        transform.position -= direccionRetroceso;

        yield return new WaitForSeconds(0.05f);
        transform.position -= direccionRetroceso;
        yield return new WaitForSeconds(0.05f);
        transform.position += direccionRetroceso;
        cameraSigueme = true;


    }

    public void KnockBack()
    {
        tiempoKnock += Time.fixedDeltaTime;
        //     Debug.Log("fUERZA" + fuerzaKnock + "direction" + knockBackdirection + " fuerza" + fuerzaKnock);
        //   Debug.Log("Rb velocity es: " + rb.velocity);
        if (vivo == false)
        {
            //  tiempoKnock += Time.fixedUnscaledDeltaTime;
        }

        if (tiempoKnock > 0.25f) //bajar
        {
            rb.linearVelocity = new Vector2(fuerzaKnock * knockBackdirection, -fuerzaKnock);

        }
        else if (tiempoKnock < 0.25f) //subir
        {
            rb.linearVelocity = new Vector2(fuerzaKnock * knockBackdirection, fuerzaKnock);

        }
        if (tiempoKnock >= 0.5f)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }


    public void Salto()
    {
        if (ejecutandoSalto)
        {
            puedoSaltar = false;

            if (tiempoSalto == 0)
            {
                posicionSombra = sombra.transform.position;
                anim.SetTrigger("triggerSalto");
                anim.SetInteger("random", 0);
                anim.SetBool("saltando", true);
                audioSalto.Play();
            }

            tiempoSalto += Time.fixedDeltaTime;

            if (Mathf.Abs(transform.position.y - posicionYPreviaSalto) < 0.01f && tiempoSalto > .5f * modificadorTiempoSaltoGeneral)
            {
                tiempoSalto = .94f * modificadorTiempoSaltoGeneral;
            }

            float duracion = 0.94f * modificadorTiempoSaltoGeneral;
            float mitad = duracion / 2f;
            float t = tiempoSalto / duracion;
            // float factor = Mathf.Abs(t - 0.5f) * 2f;

            float velocidadActual = 0;
            float factor = Mathf.Abs(t - 0.5f) * 2f;
            float factorSuave = 1 - Mathf.Pow(1 - factor, 3); // curva cúbica inversa para subida/bajada suave
            float velocidadBase = fuerzaSalto * 0.15f; // velocidad general mucho menor

            if (tiempoSalto < mitad)
            {
                velocidadActual = velocidadBase * Mathf.Lerp(0.05f, 1f, factorSuave);
            }
            else if (tiempoSalto < duracion)
            {
                velocidadActual = -velocidadBase * Mathf.Lerp(0.05f, 1.65f, factorSuave);
            }

            float nuevaY = transform.position.y + velocidadActual * Time.fixedDeltaTime;
            if (nuevaY < posicionYPreviaSalto)
            {
                nuevaY = posicionYPreviaSalto;
                velocidadActual = 0;
            }

            inputSalto.y = velocidadActual;
            transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);

            if (tiempoSalto >= duracion)
            {
                transform.position = new Vector3(transform.position.x, posicionYPreviaSalto, transform.position.z);
                inputSalto.y = 0;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                AnimatorStateInfo clip = anim.GetCurrentAnimatorStateInfo(0);
                if (clip.IsName("AtaqueSalto")) anim.Play("Idle");
                anim.SetBool("saltando", false);
                puedoSaltar = true;
                tiempoSalto = 0;
                ejecutandoSalto = false;
                posicionSombra = Vector3.zero;
            }
        }
    }


    IEnumerator KnockMuerte()
    {
        yield return new WaitForSeconds(0.65f);
        knockBack = false;
        tiempoKnock = 0;
    }
    IEnumerator RestablecerAtacado(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        speed = speedOriginal;
        puedoAtacar = true;
        puedoGirarme = true;

        knockBack = false;

        anim.SetBool("saltando", false);
        if (vivo && grabbedEnemy == false)
        {
            anim.Play("Idle");
        }
        puedoSaltar = true;
        modificadorSpeed = 1;
        tiempoSalto = 0;
        ejecutandoSalto = false;
        excepcionPorAtacadoSaltando = false;
        posicionSombra = Vector3.zero;

        yield return new WaitForSeconds(0f); // era 0.65f
        puedoSerAtacado = true;
    }

    public void AudioLevantarse()
    {
        audioLevantarse.Play();
    }

    public void AudioMuerte()
    {
        audioMuerte.Play();
    }




    void RecibirCombo()
    {
        if (Time.timeScale >= 1)
        {
            if (tiempoUltimoAtaque > 0)
            {
                tiempoUltimoAtaque -= Time.deltaTime;
            }
            else
            {
                golpesRecibidos = 0;
                tiempoUltimoAtaque = 0;
            }


        }

    }

    public IEnumerator SacudidaCamara()
    {
        Camera.main.orthographicSize = 5.3f;
        yield return new WaitForSeconds(0.1f);
        Camera.main.orthographicSize = 5.5f;
        yield return new WaitForSeconds(0.1f);
        Camera.main.orthographicSize = 5.37f;
        yield return new WaitForSeconds(0.1f);
        Camera.main.orthographicSize = 5.5f;
        yield return new WaitForSeconds(0.1f);
        Camera.main.orthographicSize = 5.42f;
        yield return new WaitForSeconds(0.1f);
        Camera.main.orthographicSize = 5.5f;
    }
    bool corriendo;
    //Control tactil

    public void CorrerTactil()
    {
        if (grabbedEnemy == null)
        {
            corriendo = !corriendo;
            if (ejecutandoSalto == false && corriendo)
            {
                modificadorSpeed = 1.5f;
            }
            else if (ejecutandoSalto == false && corriendo == false)
            {
                modificadorSpeed = 1;
            }
            Debug.Log("Modif" + modificadorSpeed);
        }

    }
    public void AtaqueTactil()
    {
        if (grabbedEnemy != null)
        {
            return;
        }

        if (enCartel)
        {
            tomandoCartel = true;
            cameraSigueme = false;
            puedoSerAtacado = false;
            puedoAtacar = false;

            float distanciaCartel;
            distanciaCartel = Mathf.Abs(transform.position.x - cartelColisionado.transform.position.x);
            Debug.Log("Distancia" + distanciaCartel);
            if (transform.position.x > cartelColisionado.transform.position.x)
            {
                //transform.position = cartelColisionado.transform.GetChild(0).transform.position;
                transform.position = cartelColisionado.transform.position + new Vector3(.85f, 0, 0);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            }
            else
            {
                transform.position = cartelColisionado.transform.position - new Vector3(.8f, 0, 0);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            }
            //  Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            Destroy(cartelColisionado);

            tengoCartel = true;
            rb.linearVelocity = Vector2.zero;
            speed = 0;
            anim.Play("SacarCartel");
        }
        else
        {

            if (puedoAtacar && ejecutandoSalto == false && vivo)
            {
                puedoAtacar = false;
                corrutinaAParar = StartCoroutine(Atacar());
            }
            else if (puedoAtacar == false && ejecutandoSalto == false)
            {
                float tiempoNormalizado = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

                if (tiempoNormalizado > porcentajeMinimo && stateInfo.IsName("Punch"))
                {
                    StopCoroutine(corrutinaAParar);
                    corrutinaAParar = StartCoroutine(Atacar());
                    puedoAtacar = false;
                    speed = 0;
                }
            }
            else if (puedoAtacar == true && ejecutandoSalto == true && tiempoSalto < .9f * modificadorTiempoSaltoGeneral) // era 0.45f
            {
                puedoAtacar = false;
                StartCoroutine(AtaqueAire());
            }
        }
    }

    float inicioSalto;
    //   float finSalto;
    public void AumentarSalto()
    {
        fuerzaSalto = fuerzaSaltoOriginal;
        inicioSalto = Time.time;
    }
    public void SaltoTactil()
    {
        if (grabbedEnemy != null)
        {
            return;
        }
        if (puedoSaltar == true && puedoAtacar && vivo)
        {
            AnimatorStateInfo clipAnim = anim.GetCurrentAnimatorStateInfo(0);

            if (!clipAnim.IsName("Salto2"))
            {
                Debug.Log("Entra salto");
                puedoSaltar = false;
                posicionYPreviaSalto = transform.position.y;
                ejecutandoSalto = true;

            }
            else
            {
                Debug.LogError("Salto impedido desde salto");
            }
        }
    }

    public void BajarSalto()
    {
        if (ejecutandoSalto)
        {
            if (tiempoSalto >= 0.4f * modificadorTiempoSaltoGeneral) //era 0.2
            {
                tiempoSalto = 0.5f * modificadorTiempoSaltoGeneral;// era 0.5

            }
            else
            {
                tiempoSalto = 0.25f; // era 0.4
            }
        }
    }


    private GameObject grabbedEnemy;
    public float distanciaAgarre = 1.75f;
    bool puedoLanzar = false;
    public GameObject reposicionEneLanzado;

    public void AgarrarEnemigo()
    {
        AnimatorStateInfo animActual = anim.GetCurrentAnimatorStateInfo(0);

        if (tengoCartel == true || (!animActual.IsName("Idle") && !animActual.IsName("Walk") && !animActual.IsName("Corriendo") && !animActual.IsName("Agarrar") && !animActual.IsName("AgarrarQuieto")))
        {
            return;
        }

        if (grabbedEnemy != null)
        {
            if (puedoLanzar)
            {

                puedoLanzar = false;
                puedoAtacar = false;
                puedoSaltar = false;
                puedoSerAtacado = false;
                modificadorSpeed = 0;

                anim.Play("LanzarEne");


            }
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, distanciaAgarre);  // DISTANCIA�����������������������
        if (colliders.Length == 0) return;

        Vector2 playerDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Collider2D closestEnemy = null;
        float closestDistance = distanciaAgarre; // M�xima distancia permitida       DISTANCIA ���������������������������

        foreach (var col in colliders)
        {
            Enemigo enemy = col.GetComponent<Enemigo>(); // Verifica si tiene el script "Enemigo"
            if (enemy == null) continue;

            Vector2 toEnemy = (col.transform.position - transform.position).normalized;
            if (Vector2.Dot(playerDirection, toEnemy) > 0.5f) // Solo enemigos en frente
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance <= closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = col;
                }
            }
        }

        if (closestEnemy != null)
        {
            closestEnemy.transform.SetParent(reposicionEneLanzado.transform);
            grabbedEnemy = closestEnemy.gameObject; // Guarda referencia
            grabbedEnemy.transform.localPosition = Vector3.zero; // new Vector2(0.246f, 0.033f);

            // grabbedEnemy.GetComponent<Enemigo>().InstanciaSangreExterna();
            grabbedEnemy.GetComponent<Enemigo>().enabled = false;
            grabbedEnemy.GetComponent<Enemigo>().sombra.SetActive(false);
            grabbedEnemy.GetComponent<BoxCollider2D>().enabled = false;
            grabbedEnemy.GetComponent<Animator>().Play("EneAgarrado");

            Invoke("PuedoLanzar", 0.5f);
            anim.Play("Agarrar");
            modificadorSpeed = 1;
            corriendo = false;

        }
    }


    public void PuedoLanzar()
    {
        puedoLanzar = true;
    }

    public void RestablecerTrasLanzamiento()
    {
        puedoLanzar = true;
        puedoAtacar = true;
        puedoSaltar = true;
        puedoSerAtacado = true;
        modificadorSpeed = 1;

        grabbedEnemy.transform.parent = null;
        grabbedEnemy.GetComponent<EneLanzado>().enabled = true;
        grabbedEnemy.GetComponent<Animator>().Play("EneLanzado");

        grabbedEnemy = null;
    }


    public void BombaHumo()
    {
        if (grabbedEnemy != null)
        {
            return;
        }
        if (puedoAtacar == true && ejecutandoSalto == false && vivo)
        {
            if (bombasHumo >= 1)
            {
                speed = 0;
                puedoGirarme = false; // new
                puedoSerAtacado = false;
                puedoSaltar = false;
                puedoAtacar = false;
                bombasHumo -= 1;
                anim.Play("BombaHumo");
            }
        }
    }
    public void AudioBomba()
    {
        audioBombaHumo.Play();
    }

    public void RestablecerBomba()
    {
        //  AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        //  yield return new WaitForSeconds(stateInfo.length);

        puedoSerAtacado = true;
        puedoSaltar = true;
        puedoAtacar = true;
        speed = speedOriginal;

        int direccionTeletransporte;
        if (transform.localScale.x > 0) { direccionTeletransporte = 1; } else { direccionTeletransporte = -1; }
        transform.position += new Vector3(5 * direccionTeletransporte, 0, 0);

        if (transform.localScale.x > 0)
        {
            transform.localScale = new Vector2(-5, transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(5, transform.localScale.y);

        }
        puedoGirarme = true; // new
        StartCoroutine(Parpadeo());
    }

    public void IniciarParpadeo()
    {
        StartCoroutine(Parpadeo());

    }

    IEnumerator Parpadeo()
    {
        parpadeando = true;
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

        yield return new WaitForSeconds(0.14f); // era 0.14
        parpadeando = false;
    }

    public void QuitarImagen()
    {
        sp.enabled = false;
        sombra.SetActive(false);
    }
}
