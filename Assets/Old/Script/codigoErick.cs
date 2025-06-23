using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class codigoErick : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void NormalMovement(Vector2 dir, float moveSpeed = 1)
    {
        if (dir.magnitude!=0)
        {
            float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            //   transform.rotation = Quaternion.Euler(transform.eulerAngles.x, angle, transform.eulerAngles.z);
            transform.eulerAngles = transform.up * angle;
            rb.linearVelocity = transform.forward * moveSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        NormalMovement(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1);
    }
}
