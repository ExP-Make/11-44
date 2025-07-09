using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float _moveSpeed = 5f;
    public float _jumpForce = 12f;  
    private bool _isGround;
    
    public LayerMask _groundLayer;
    
    // Components
    private Rigidbody2D _rigidbody;
 

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (CanMove())
        {
            Move();
            Jump();
        }
        
        GroundCheck();
    }

    private bool CanMove()
    {
        if (DialogManager.Instance.IsDialogOpen()) return false;
        return true;
    }
    
    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        _rigidbody.linearVelocity = new Vector2(moveInput * _moveSpeed, _rigidbody.linearVelocity.y);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGround)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpForce);
        }
    }

    private void GroundCheck()
    {
        RaycastHit2D hit;
        float distance = 1f;
        Vector2 dir = Vector2.down;

        hit = Physics2D.Raycast(transform.position, dir, distance, _groundLayer);

        if(hit.collider != null)
        {
            _isGround = true;
        }
        else
        {
            _isGround = false;
            //Debug.Log("Not Ground");
        }
    }
}