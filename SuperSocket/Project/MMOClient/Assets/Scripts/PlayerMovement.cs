using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float m_Speed;
    private Rigidbody2D m_myRigidbody;
    private Vector3 m_change;
    private Animator m_animator;


    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_myRigidbody = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        m_change = Vector3.zero;
        m_change.x = Input.GetAxisRaw("Horizontal");
        m_change.y = Input.GetAxisRaw("Vertical");

        UpdateAnimationAndMove();
    }

    void UpdateAnimationAndMove()
    {
        if (m_change != Vector3.zero)
        {
            MoveCharacter();
            m_animator.SetFloat("moveX", m_change.x);
            m_animator.SetFloat("moveY", m_change.y);
            m_animator.SetBool("moving", true);
        }
        else
        {
            m_animator.SetBool("moving", false);
        }
    }

    void MoveCharacter()
    {
        m_myRigidbody.MovePosition(
           transform.position + m_change * m_Speed * Time.deltaTime
            );


    }

}
