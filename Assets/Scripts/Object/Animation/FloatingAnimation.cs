using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    private float moveLength = 0.3f;

    private float moveSpeed = 1f;

    private bool isEnable = false;

    private Vector3 initPos;
    private Vector3 currentMovePos;
    private float floating;

    public void SetUp(float moveLength = 0.1f, float moveSpeed = 10f)
    {
        this.moveLength = moveLength;
        this.moveSpeed = moveSpeed;
        initPos = transform.position;
        currentMovePos = Vector3.zero;
        floating = this.moveLength;
        isEnable = false;
    }

    public void StartAction()
    {
        isEnable = true;
        transform.position = initPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnable) return;
        var pos = currentMovePos;
        floating += -pos.y * Time.deltaTime * moveSpeed;
        pos.y += floating * Time.deltaTime * moveSpeed;
        currentMovePos = pos;
        transform.position = initPos + currentMovePos;
    }
}
