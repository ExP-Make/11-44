using System;
using UnityEngine;
using UnityEngine.AI;
using EnemyAI.EnemyEventsArgs;

namespace EnemyAI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class EnemyAI : MonoBehaviour
    {
        [Header("기본 스탯")]
        [Tooltip("이 적이 사용할 스탯 데이터")]
        [SerializeField] private EnemyStats stats;

        public int  CurrentHp { get; private set; }
        public bool IsDead { 
            get {
                return CurrentHp <= 0;
            }
        }

        [Header("타깃 지정")]
        [Tooltip("타깃 태그. 씬에 단 하나만 존재해야 함. 기본 설정 시 Player 태그를 사용")]
        [SerializeField] private string hostileTag = "Player";

        [Header("시야/감지")]
        [Tooltip("플레이어를 감지하는 반경. 0이면 씬 전체")]
        [SerializeField] private float detectionRadius = 0f;

        [Tooltip("장애물 고려 활성화 여부")]
        [SerializeField] private bool lineOfSightBlockedByObstacles = true;

        [Tooltip("장애물 고려 시 사용할 장애물 레이어")]
        [SerializeField] private LayerMask obstacleMask = -1;

        [Header("근접 반경")]
        [Tooltip("플레이어와 가까워졌다고 판단하는 반경")]
        [SerializeField] private float nearRadius = 3f;

        [Tooltip("히트박스 기준 적이 공격을 시도하는 반경")]
        [SerializeField] private float attackRadius = 1.5f;

        [Header("놓침")]
        [Tooltip("플레이어를 놓치는 거리")]
        [SerializeField] private float loseSightRadius = 20f;
        [Header("눈 보정치")]
        [Tooltip("적의 눈 보정치. 눈 좌표가 없으면 1.5f를 사용. 장애물 레이캐스트 시 사용")]
        [SerializeField] private float eyeHeight = 1.5f;

        [Header("비행 설정 (isFlying=true 일 때)")]
        [Tooltip("비행 시 회전 속도")]
        [SerializeField] private float flyingRotationSpeed = 5f;

        public event EventHandler OnSpawn;
        public event EventHandler<PlayerEventArgs> OnPlayerDetected;
        public event EventHandler<PlayerEventArgs> OnPlayerNear;
        public event EventHandler<PlayerEventArgs> OnPlayerLost;
        public event EventHandler<PlayerEventArgs> OnAttack;

        private NavMeshAgent _agent;
        private Transform _target;
        private bool _playerDetected;
        private bool _playerNear;

        private void Awake()
        {
            _agent   = GetComponent<NavMeshAgent>();
            CurrentHp = stats != null ? stats.maxHp : 1;
        }

        private void Start()
        {
            OnSpawn?.Invoke(this, EventArgs.Empty);

            if (!string.IsNullOrWhiteSpace(hostileTag))
            {
                var playerObj = GameObject.FindGameObjectWithTag(hostileTag);
                _target = playerObj != null ? playerObj.transform : null;
            }

            if (_target == null)
                Debug.LogWarning($"{name}: Target Found Failed");

            if (stats == null || _agent == null)
            {
                Debug.LogWarning($"{name}: Stats or NavMeshAgent is not assigned.");
                return;
            }

            _agent.speed = stats.moveSpeed;
            _agent.baseOffset = stats.isFlying ? 2.0f : 0f;
            _agent.obstacleAvoidanceType = stats.isFlying
                ? ObstacleAvoidanceType.NoObstacleAvoidance
                : ObstacleAvoidanceType.HighQualityObstacleAvoidance;

            if (stats.isFlying) // flying enemy movement setting
            {
                _agent.updatePosition = false;
                _agent.updateRotation = false;
                _agent.isStopped = true;
                if (TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }
            else
            {
                _agent.updatePosition = true;
                _agent.updateRotation = true;
                _agent.isStopped = false;
            }
        }

        private void Update()
        {
            if (_target == null || IsDead)
            {
                if (IsDead && _agent != null && !stats.isFlying) _agent.isStopped = true;
                return;
            }

            float dist = Vector3.Distance(transform.position, _target.position);

            if (!_playerDetected) // check is player detected
            {
                if ((detectionRadius <= 0f || dist <= detectionRadius) && HasLineOfSight())
                {
                    _playerDetected = true;
                    OnPlayerDetected?.Invoke(this, new PlayerEventArgs(_target.gameObject));
                    if (!stats.isFlying && _agent != null) _agent.isStopped = false;
                }
                else return;
            }

            if (dist > loseSightRadius || !HasLineOfSight()) // check lost
            {
                _playerDetected = false;
                _playerNear = false;
                OnPlayerLost?.Invoke(this, new PlayerEventArgs(_target.gameObject));
                if (_agent != null)
                {
                    if (!stats.isFlying) _agent.ResetPath();
                }
                return;
            }

            if (_playerDetected && !IsDead)
            {
                if (stats.isFlying)
                {
                    Vector3 directionToTarget = (_target.position - transform.position).normalized;
                    transform.position = Vector3.MoveTowards(transform.position, _target.position, stats.moveSpeed * Time.deltaTime);

                    if (directionToTarget != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, flyingRotationSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    if (_agent != null && _agent.isOnNavMesh)
                    {
                         if (dist > attackRadius)
                         {
                            if (_agent.isStopped) _agent.isStopped = false;
                            _agent.SetDestination(_target.position);
                         }
                         else
                         {
                            if (!_agent.isStopped) _agent.isStopped = true;
                         }
                    }
                }
            }

            if (!_playerNear && dist <= nearRadius) // check is player near
            {
                _playerNear = true;
                OnPlayerNear?.Invoke(this, new PlayerEventArgs(_target.gameObject));
            }
            else if (_playerNear && dist > nearRadius)
            {
                _playerNear = false;
            }

            if (dist <= attackRadius)
            {
                if (stats.isFlying)
                {
                    Vector3 directionToTarget = (_target.position - transform.position).normalized;
                     if (directionToTarget != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, flyingRotationSpeed * Time.deltaTime * 2);
                    }
                }
                PerformAttack();
            }
        }

        private bool HasLineOfSight()
        {
            if (!lineOfSightBlockedByObstacles) return true;

            Vector3 dir = (_target.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, _target.position);
            Vector3 rayStart = transform.position + Vector3.up * eyeHeight;
            return !Physics.Raycast(rayStart, dir, dist, obstacleMask);
        }

        private void PerformAttack()
        {
            OnAttack?.Invoke(this, new PlayerEventArgs(_target.gameObject));
        }

        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            CurrentHp -= amount;
            Debug.Log($"{name} took {amount} damage. Current HP: {CurrentHp}");

            if (CurrentHp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log($"{name} died.");
            if (_agent != null)
            {
                _agent.enabled = false;
            }

            if (TryGetComponent<Collider>(out var col)) col.enabled = false;

            Destroy(gameObject, 3f);
        }
    }
}
