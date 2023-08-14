using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float moveX, moveY;

    [SerializeField]
    float moveSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -4, 4), Mathf.Clamp(transform.position.y, -3, 3.25f));
        transform.Translate(new Vector2(moveX, moveY) * moveSpeed * Time.deltaTime);
    }
}
