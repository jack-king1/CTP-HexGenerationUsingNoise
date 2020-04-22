using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveToRayHit : MonoBehaviour
{
    public LayerMask layerToHit;
    RaycastHit hit;

    public float rayDist = 100f;
    public bool grounded = false;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (!grounded)
        {
            //Debug.DrawRay(transform.position, -transform.up * rayDist, Color.red);
            //if (Physics.Raycast(transform.position + transform.forward * 3, transform.TransformDirection(-Vector3.up), out hit, rayDist, layerToHit))
            //{
            //    if (hit.distance >= 0.05f)
            //    {
            //        transform.Translate(-Vector3.up * Time.deltaTime * 10);
            //    }
            //    else
            //    {
            //        grounded = true;
            //        Debug.Log("Grounded");
            //    }
            //}
            if(transform.position.y < -1)
            {
                Destroy(this.gameObject);
            }
            else
            {
                //Debug.Log(gameObject.GetComponentInParent<HexCell>().Elevation);
                float speed = Vector3.Distance(gameObject.transform.position, 
                    new Vector3(gameObject.transform.position.x, 0 + gameObject.GetComponentInParent<HexCell>().Elevation, gameObject.transform.position.z));
                if(speed < 1)
                {
                    speed = 1f;
                }

                transform.position += new Vector3(0, (speed * -1) * Time.deltaTime, 0);
            }
            
        }
        //Unfortunately raycast cant be detected from underneath so will have to move it above the mesh everytime something is updated.
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 8)
        {
            rb.isKinematic = false;
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)   
    {
        if(grounded)
        {
            rb.isKinematic = true;
            grounded = false;
        }
    }

}
