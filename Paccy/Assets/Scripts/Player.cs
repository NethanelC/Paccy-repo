using System;
using UnityEngine;

public class Player : BaseLevelDependant
{
    public event Action PacmanDeath;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _terrainLayer;
    private float[] _currentLevelPacmanNormalSpeed = new float[5]; 
    private Vector2 _currentDirection, _nextDirection;
    public override void BasePositionAndFacingSide()
    {
        base.BasePositionAndFacingSide();
        SetDirection(Vector2.left);
        _animator.SetBool("IsAlive", true);
        _animator.speed = 0;
    }
    public override void InitByLevel(byte level)
    {
        
    }
    public override void NewRound()
    {
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _animator.speed = 1;
    }
    private void Death()
    {
        _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        _animator.SetBool("IsAlive", false);
        PacmanDeath?.Invoke();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Ghost ghost = collision.GetComponent<Ghost>();
            if (!ghost.TryGetEaten())
            {
                Death();
            }
        }
        if (collision.gameObject.layer == 9)
        {
            print("pacman hit");
        }
    }
    private void Update()
    {
        bool wUpKey = Input.GetKeyDown(KeyCode.W);
        bool arrowUpKey = Input.GetKeyDown(KeyCode.UpArrow);
        bool sDownKey = Input.GetKeyDown(KeyCode.S);
        bool arrowDownKey = Input.GetKeyDown(KeyCode.DownArrow);
        bool dRightKey = Input.GetKeyDown(KeyCode.D);
        bool arrowRightKey = Input.GetKeyDown(KeyCode.RightArrow);
        bool aLeftKey = Input.GetKeyDown(KeyCode.A);
        bool arrowLeftKey = Input.GetKeyDown(KeyCode.LeftArrow);
        if (wUpKey || arrowUpKey)
        {
            SetDirection(Vector2.up);
        }
        else if (sDownKey || arrowDownKey)
        {
            SetDirection(Vector2.down);
        }
        else if (aLeftKey || arrowLeftKey)
        {
            SetDirection(Vector2.left);
        }
        else if (dRightKey || arrowRightKey)
        {
            SetDirection(Vector2.right);
        }
        if (_nextDirection != Vector2.zero)
        {
            SetDirection (_nextDirection);
        }
    }
    private void FixedUpdate()
    {
        Vector2 thing = (9 * Time.fixedDeltaTime * _currentDirection);
        _rigidBody.MovePosition(_rigidBody.position + thing);
    }
    private void SetDirection(Vector2 direction)
    {
        if (!IsObstacle(direction))
        {
            _currentDirection = direction;
            _nextDirection = Vector2.zero;
            float angle = Mathf.Atan2(direction.y, direction.x);
            transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
            return;
        }
        _nextDirection = direction;
    }
    private bool IsObstacle(Vector2 direction) => Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0, direction, 1.5f, _terrainLayer);  
}
