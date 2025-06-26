using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;     // �¿� �̵� �ӵ�
    public float jumpForce = 12f;    // ���� ��
    public LayerMask groundLayer;    // ������ �ν��� ���̾�

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!GameManager.Instance.isDialogOpen)
        {
            Move();
            Jump();
            CheckGround();
        }
    }

    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void CheckGround()
    {
        // �߹����� ������ ���
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, groundLayer);

        if (hit.collider != null)
            isGrounded = true;
        else
            isGrounded = false;

        Debug.DrawRay(transform.position, Vector2.down * 1.0f, Color.red);
    }
}