using UnityEngine;
using System.Collections;
public class GruzMother : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] Transform groundCheckUp;
    [SerializeField] Transform groundCheckDown;
    [SerializeField] Transform wallCheck;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    private Vector2 lastVelocity;

    private float wallCheckRadius = 0.02f;
    private float groundCheckRadius = 0.02f;

    private void Start() {
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (this.GetComponent<Animator>().GetBool("isChargingAttack"))
        {
            this.GetComponent<Animator>().SetBool("isChargingAttack", false);
                

            float speed = lastVelocity.magnitude;
            Vector2 direction = Vector2.Reflect(lastVelocity.normalized, other.contacts[0].normal);
            rb.velocity = direction * Mathf.Max(speed, 0f);
            StartCoroutine(ReboundForce());

        }
    }

    IEnumerator ReboundForce()
    {


        // rb.AddForce(new Vector2(-rb.velocity.x, -rb.velocity.y).normalized * Time.fixedDeltaTime * 3);
        yield return new WaitForSeconds(0.1f);
        rb.velocity = Vector2.zero;
        rb.GetComponent<Animator>().Play("GruzMother_Pause");

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(groundCheckUp.position, groundCheckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheckDown.position, groundCheckRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(wallCheck.position, wallCheckRadius);
    }
}