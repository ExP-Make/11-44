using System;
using UnityEngine;

public class FloatableObject : MonoBehaviour
{
    public Transform FloatingPoint;
    public float FloatingSpeed;

    private BoxCollider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private bool _isFloating; 
    
    public event Action OnFloatEnd;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _collider.enabled = false;

        // CombatManager에 등록
        CombatManager.Instance.OnFloatStart += StartFloat;
    }

    private void StartFloat()
    {
        _isFloating = true;
    }

    private void EndFloat()
    {
        _isFloating = false;
        _collider.enabled = true;
        
        OnFloatEnd?.Invoke();
    }

    private void Update()
    {
        if (_isFloating)
        {
            Vector3 moveDir = (FloatingPoint.position - transform.position).normalized;
        
            transform.Translate(moveDir * (FloatingSpeed * Time.deltaTime));
            if (Vector3.Distance(transform.position, FloatingPoint.position) < 0.1f)
            {
                transform.position = FloatingPoint.position;
                EndFloat();
            }
        }
    }
}