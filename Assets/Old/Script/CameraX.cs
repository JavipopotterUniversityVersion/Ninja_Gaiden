using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraX : MonoBehaviour
{
    Player player;
    public float speed = 10;
    public float minX = -2.8f;
    public float maxX = 59;
    
    private void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float valorX = player.transform.position.x;
        valorX = Mathf.Clamp(valorX, minX, maxX);

        if (player.cameraSigueme)
        {
            if (Mathf.Abs(transform.position.x - player.transform.position.x) < 0.5f && speed != 7) 
            {
                speed = 50;
                //transform.position = Vector3.MoveTowards(transform.position, new Vector3(valorX, transform.position.y, transform.position.z), speed * Time.deltaTime);
            }
            else
            {
                speed = 8;
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(valorX, transform.position.y, transform.position.z), speed * Time.deltaTime);

            // transform.position = new Vector3(valorX, transform.position.y, transform.position.z);
        }else if (player.tomandoCartel)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(valorX, transform.position.y, transform.position.z), .8f * Time.deltaTime);

        }



    }
}
