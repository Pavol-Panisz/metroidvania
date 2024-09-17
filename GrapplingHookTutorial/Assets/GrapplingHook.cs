using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(mouseWorldPos);

        Vector2 mouseWorldPos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        // posun je translation
        // translation = goal - start
        Vector2 translation = mouseWorldPos2D - rb.position;

        Debug.DrawLine(rb.position, rb.position + translation);

        // pridavaj silu len ked drzis mysku
        if (Input.GetMouseButton(0) == true)
        {
            rb.AddForce(translation);
        }
    }
}
