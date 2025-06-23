using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopiarSprite : MonoBehaviour
{
     Image image;
    public SpriteRenderer spTarget;
    private void Awake()
    {
        image = GetComponent<Image>();

        if (spTarget == null)
        {
            spTarget = GameObject.FindObjectOfType<Player>().GetComponent<SpriteRenderer>();
        }
        image.sprite = spTarget.sprite;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spTarget == null)
        {
            Destroy(gameObject);
        }
        else
        {
            image.sprite = spTarget.sprite;
        }
    }
}
