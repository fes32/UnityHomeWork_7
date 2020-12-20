using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float MinGroundNormalY = .65f;
    [SerializeField] private float GravityModFilter = 1f;
    [SerializeField] private Vector2 Velocity;
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _speed;

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2D;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);
    protected Animator _animator;
    protected const float minMoveDistance = 0.01f;
    protected const float shellRadius = 0.001f;


    void OnEnable()
    {
        rb2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(LayerMask);
        contactFilter.useLayerMask = true;
        _animator.SetTrigger("Idle");
    }

    void Update()
    {
        targetVelocity = new Vector2(Input.GetAxis("Horizontal"), 0);

        if (targetVelocity.x > 0)
            GetComponent<SpriteRenderer>().flipX = false;     
        else if (targetVelocity.x < 0)
            GetComponent<SpriteRenderer>().flipX = true;

        if (targetVelocity.magnitude == 0 & grounded)
            _animator.SetTrigger("Idle");


        if (targetVelocity.x != 0 )
            _animator.SetTrigger("Run");

        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            _animator.SetTrigger("Jump");
            Velocity.y = _jumpForce;
            grounded = false;
        }
    }

    void FixedUpdate()
    {
        Velocity += GravityModFilter * Physics2D.gravity * Time.deltaTime;
        Velocity.x = targetVelocity.x;

        grounded = false;

        Vector2 deltaPosition = Velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            int countObstacles = rb2D.Cast(move, contactFilter, hitBuffer, distance + shellRadius);

            hitBufferList.Clear();

            for (int i = 0; i < countObstacles; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;

                if (currentNormal.y > MinGroundNormalY)
                {
                    grounded = true;

                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }
                float projection = Vector2.Dot(Velocity, currentNormal);

                if (projection < 0)
                {
                    Velocity = Velocity - projection * currentNormal;
                }
                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        rb2D.position = rb2D.position + move.normalized * distance * _speed;
    }
}