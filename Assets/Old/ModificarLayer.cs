using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModificarLayer : MonoBehaviour
{
    public SpriteRenderer target;
    SpriteRenderer sp;
    int layerOriginal;
    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        layerOriginal = sp.sortingOrder;
    }

    // Update is called once per frame
    void Update()
    {
        if (target.transform.position.y > transform.position.y )
        {
            sp.sortingOrder = target.sortingOrder + 1;
        }
        else
        {
            sp.sortingOrder = layerOriginal;
        }
    }
}
