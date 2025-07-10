using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed;
    public float JumpForce;
    
    private Rigidbody2D _rigidbody;
    
    public Transform GroundCheckPoint, GroundCheckPoint2;
    public LayerMask GroundLayer;
    private bool _isGround;

    public Animator _animator;
    public SpriteRenderer _spriteRenderer;

    public float CoyoteTime = 0.2f;
    private float _coyoteCounter;

    public float JumpBufferLength = 0.1f;
    private float _jumpBufferCounter;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // 이동방향 결정 (Horizontal)
        _rigidbody.linearVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * MoveSpeed, _rigidbody.linearVelocity.y);
        
        GroundCheck();
        
        UpdateJumpBuffer();
        
        UpdateCoyoteTime();
        
        UpdateJump();

        FlipPlayer();
        
        UpdateAnimator();
    }

    private void GroundCheck()
    {
        // Ground 체크
        _isGround = Physics2D.OverlapCircle(GroundCheckPoint.position, 0.1f, GroundLayer) ||
                    Physics2D.OverlapCircle(GroundCheckPoint2.position, 0.1f, GroundLayer);
    }

    private void UpdateJumpBuffer()
    {
        // 점프 버퍼
        if (Input.GetButtonDown("Jump"))
        {
            _jumpBufferCounter = JumpBufferLength;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void UpdateCoyoteTime()
    {
        // 코요테 타임
        if (_isGround)
        {
            _coyoteCounter = CoyoteTime;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }
    }

    private void UpdateJump()
    {
        // Jump 시작. y방향으로 가속을 준다
        if (_jumpBufferCounter >= 0 && _coyoteCounter > 0f)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, JumpForce);
            _jumpBufferCounter = 0;
        }

        // Jump키에서 손을 떼면 고도 상승을 중지한다
        if (Input.GetButtonUp("Jump") && _rigidbody.linearVelocity.y > 0)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y * .5f);
        }
    }

    private void FlipPlayer()
    {
        if (_spriteRenderer)
        {
            // 이동 방향에 따라 flip
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                _spriteRenderer.flipX = true;
            }
            else if(Input.GetAxisRaw("Horizontal") < 0)
            {
                _spriteRenderer.flipX = false;
            }
        }
    }

    private void UpdateAnimator()
    {
        // Animation Setting
        if (_animator)
        {
            _animator.SetFloat("xSpeed", Mathf.Abs(_rigidbody.linearVelocity.x));
            _animator.SetFloat("ySpeed", _rigidbody.linearVelocity.y);
            _animator.SetBool("isGround", _isGround);
        }
    }
}
