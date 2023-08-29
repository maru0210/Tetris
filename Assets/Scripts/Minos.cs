using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Minos : MonoBehaviour
{
    private float _repeatSpan = 1; // �J��Ԃ����o
    private float _timeElapsed = 0; // �o�ߎ���

    // �~�m�̈ړ�
    private Vector3 right = new Vector3(-2, 0, 0);
    private Vector3 left = new Vector3(2, 0, 0);
    private Vector3 down = new Vector3(0, -2, 0);

    // �ڐG�t���O
    private bool isTouchLeft = false;
    private bool isTouchRight = false;

    // ��~�t���O
    public bool isFix = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isFix) return;

        _timeElapsed += Time.deltaTime;
        Vector3 mov = Vector3.zero;

        // _repeatSpan�b���ƂɎ��s
        if (_timeElapsed >= _repeatSpan)
        {
            mov += down;

            _timeElapsed = 0;
        }

        // ���E�ړ�
        if (Input.GetKeyDown(KeyCode.LeftArrow) && isTouchLeft == false)
        {
            isTouchRight = false;
            mov += left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && isTouchRight == false)
        {
            isTouchLeft = false;
            mov += right;
        }

        // DownArrow�������Ă���Ԃ����������x��ύX
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _repeatSpan = 0.5f;
        }
        else
        {
            _repeatSpan = 1;
        }

        // �ړ��̓K�p
        if (mov != Vector3.zero)
        {
            this.transform.position += mov;
        }
    }

    public void LeftEventHandler(Collider collider)
    {
        Debug.Log($"�����̌��o : {collider.name}");
        isTouchLeft = true;
    }

    public void RightEventHandler(Collider collider)
    {
        Debug.Log($"�E���̌��o : {collider.name}");
        isTouchRight = true;
    }

    public void BottomEventHandler(Collider collider)
    {
        Debug.Log($"�����̌��o : {collider.name}");
        isFix = true;
    }
}
