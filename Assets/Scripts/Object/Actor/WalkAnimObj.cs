using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAnimObj : MonoBehaviour
{
    Animator m_Animator;
    public bool animatorEnabled { get { return m_Animator.enabled; } set { m_Animator.enabled = value; } }
    bool isEnabled = false;

    public float animSpeed = 1f;
    public bool isAutoRotation = true;
    public bool isLookTarget = false;
    public Vector3 lookTargetPosition;

    private Vector3 moveDir;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        lookTargetPosition = transform.position + transform.forward;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_Animator.speed = animSpeed;
        moveDir = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            if (isAutoRotation)
            {
                transform.rotation = Quaternion.LookRotation(moveDir);
            }
            m_Animator.SetFloat("Forward", animSpeed, 0.1f, Time.deltaTime);
        }
    }

    public void SetMoveDir(Vector3 dir)
    {
        dir.y = transform.position.y;
        moveDir = dir.normalized;
    }

    public void SetAnimSpeed(float speed)
    {
        animSpeed = speed;
        m_Animator.speed = animSpeed;
    }

    public void AnimOn()
    {
        isEnabled = true;
    }
    public void AnimOff()
    {
        isEnabled = false;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (isLookTarget)
        {
            this.m_Animator.SetLookAtWeight(1.0f, 0.8f, 1.0f, 0.0f, 0f);
            this.m_Animator.SetLookAtPosition(lookTargetPosition);
        }
    }
}
