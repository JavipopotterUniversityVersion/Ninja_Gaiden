using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Trucos : MonoBehaviour
{
    public Player player;
    public Enemigo enemigo;
    public TextMeshProUGUI textoInvencible;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    

   
    public void TodosInvencibles()
    {
        player.invencible = !player.invencible;
        enemigo.invencible = !enemigo.invencible;

        textoInvencible.text = "Invulnerabilidad: " + player.invencible;

        if (player.invencible==true)
        {
            textoInvencible.color = Color.green;
            player.vidaPreInvencible = player.vida;
            enemigo.vidaPreInvencible = enemigo.vida;

        }
        else
        {
            textoInvencible.color = Color.red;
        }
    }

    

    public void SumarVida()
    {
        player.vida = player.vidaInicial;
        player.vidaPreInvencible = player.vida;

    }

    public void CeroVida()
    {
        player.vida = 3;
        player.vidaPreInvencible = player.vida;

    }

    public void EnemigoVida()
    {
        if (enemigo.gameObject.activeInHierarchy)
        {
            enemigo.vida = enemigo.vidaMax;
            enemigo.vidaPreInvencible = enemigo.vida;
        }

    }

    public void EnemigoCeroVida()
    {
        if (enemigo.gameObject.activeInHierarchy)
        {

            enemigo.vida = 1;
            enemigo.vidaPreInvencible = enemigo.vida;
        }


    }
}
