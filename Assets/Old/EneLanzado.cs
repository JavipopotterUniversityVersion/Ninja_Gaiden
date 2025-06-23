using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EneLanzado : MonoBehaviour
{
    float initialDistance;
    //bool restablecerCamera;

    private void OnEnable()
    {
        GetComponent<Animator>().ResetTrigger("Devorar");

        Start();
    }
    void Start()
    {
        initialDistance = Vector2.Distance(transform.position, new Vector2(transform.position.x, 1.5f));

    }

    void Update()
    {
       
        if (this.enabled)
        {
            Vector2 targetPosition = new Vector2(transform.position.x, 1.5f);
            float remainingDistance = Vector2.Distance(transform.position, targetPosition);
            float progress = (initialDistance - remainingDistance) / initialDistance;

            float scaleFactor = Mathf.Lerp(5f, 3f, progress);
          //  float sizeCamera = Mathf.Lerp(5.5f, 4.5f, progress);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 6 * Time.deltaTime);
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);

         /*   if (restablecerCamera == false)
            {
                Camera.main.orthographicSize = sizeCamera;
            }
            else
            {
                Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, 5.5f, 2 * Time.deltaTime);

            }*/

            if (transform.position.y >= 0.9f)
            {
                GetComponent<Animator>().SetTrigger("Devorar");
            }
        }
    }

    public void RestablecerCamera()
    {
      //  restablecerCamera = true;
    }

    public void Destruir()
    {
        // Camera.main.orthographicSize = 5.5f;
        GetComponent<Enemigo>().Reiniciar();
      //  Destroy(gameObject);
    }

    private void OnDestroy()
    {
       // Camera.main.orthographicSize = 5.5f;

    }



}
