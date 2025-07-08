using System;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private BoxCollider2D _collider;
    private PlatformEffector2D _platformEffector;
    
    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _platformEffector = GetComponent<PlatformEffector2D>();
    }

    private void Start()
    {
        _collider.usedByEffector = true;
        _platformEffector.useOneWay = true;
    }

    void Update()
    {
        
    }
}
