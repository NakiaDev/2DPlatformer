using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : PhysicsObject
{
    [SerializeField] float maxSpeed;
    [SerializeField] int attackPower = 10;
    [SerializeField] Vector2 rayCastOffset;
    [SerializeField] float rayCastLength = 2;
    [SerializeField] LayerMask rayCastLayerMask;

    RaycastHit2D rightLedgeRayCastHit;
    RaycastHit2D leftLedgeRayCastHit;
    RaycastHit2D rightWallRayCastHit;
    RaycastHit2D leftWallRayCastHit;
    int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocity = new Vector2(maxSpeed * direction, 0);

        rightLedgeRayCastHit = Physics2D.Raycast(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y + rayCastOffset.y), Vector2.down, rayCastLength);
        Debug.DrawRay(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y + rayCastOffset.y), Vector2.down * rayCastLength, Color.blue);
        if (rightLedgeRayCastHit.collider == null)
            direction = - 1;

        leftLedgeRayCastHit = Physics2D.Raycast(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y + rayCastOffset.y), Vector2.down, rayCastLength);
        Debug.DrawRay(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y + rayCastOffset.y), Vector2.down * rayCastLength, Color.green);
        if (leftLedgeRayCastHit.collider == null)
            direction = 1;

        rightWallRayCastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, rayCastLength, rayCastLayerMask);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.right * rayCastLength, Color.yellow);
        if (rightWallRayCastHit.collider != null)
            direction = -1;

        leftWallRayCastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, rayCastLength, rayCastLayerMask);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.left * rayCastLength, Color.cyan);
        if (leftWallRayCastHit.collider != null)
            direction = 1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            NewPlayer player = collision.gameObject.GetComponent<NewPlayer>();
            player.AddHealthValue(-attackPower);
        }
    }
}
