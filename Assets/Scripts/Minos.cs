using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Minos : MonoBehaviour
{
    private float _repeatSpan = 1; // 繰り返し感覚
    private float _timeElapsed = 0; // 経過時間

    // ミノの移動
    private Vector3 right = new Vector3(-2, 0, 0);
    private Vector3 left = new Vector3(2, 0, 0);
    private Vector3 down = new Vector3(0, -2, 0);

    // 接触フラグ
    private bool isTouchLeft = false;
    private bool isTouchRight = false;

    // 停止フラグ
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

        // _repeatSpan秒ごとに実行
        if (_timeElapsed >= _repeatSpan)
        {
            mov += down;

            _timeElapsed = 0;
        }

        // 左右移動
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

        // DownArrowを押している間だけ落下速度を変更
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _repeatSpan = 0.5f;
        }
        else
        {
            _repeatSpan = 1;
        }

        // 移動の適用
        if (mov != Vector3.zero)
        {
            this.transform.position += mov;
        }
    }

    public void LeftEventHandler(Collider collider)
    {
        Debug.Log($"左側の検出 : {collider.name}");
        isTouchLeft = true;
    }

    public void RightEventHandler(Collider collider)
    {
        Debug.Log($"右側の検出 : {collider.name}");
        isTouchRight = true;
    }

    public void BottomEventHandler(Collider collider)
    {
        Debug.Log($"下側の検出 : {collider.name}");
        isFix = true;
    }
}
