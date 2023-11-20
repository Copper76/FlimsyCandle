using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostMortem : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y < -5f)
        {
            Destroy(this.gameObject);
        }

        if (rb.velocity.y == 0)
        {
            foreach (Collider col in transform.GetChild(0).GetChild(0).GetComponents<Collider>())
            {
                col.enabled = false;
            }
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            Destroy(this);
        }
    }
}
