using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Minos : MonoBehaviour
{
    private float _repeatSpan = 1; // 繰り返し感覚
    private float _timeElapsed = 0; // 経過時間

    // BodyとSideColliderとNextRotColliderの参照
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _sideCollider;
    [SerializeField] private GameObject _nextRotCollider;

    // ミノの移動
    private Vector3 right = new Vector3(2, 0, 0);
    private Vector3 left = new Vector3(-2, 0, 0);
    private Vector3 up = new Vector3(0, 2, 0);
    private Vector3 down = new Vector3(0, -2, 0);

    // 接触フラグ
    private bool[] isTouchNextRot = { false, false, false };
    private bool[] isTouchNextRotIfMove = { false, false, false, false };

    // 停止フラグ : _isFix -> フレーム更新待ち
    private bool _isFix = false;
    public bool isFix = false;

    // 右回転値（0~3の値を保持 | 0: 0, 1: 90, 2: 180, 3: 270)
    private int minoRot = 0;

    private GameObject[][] minoSides;

    // Start is called before the first frame update
    void Start()
    {
        // SideColliderを取得
        minoSides = Array.ConvertAll(GetChildren(_sideCollider), sides => GetChildren(sides));
    }

    private GameObject[] GetChildren(GameObject parent)
    {
        GameObject[] children = new GameObject[parent.transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
            // Debug.Log(children[i].name);
        }

        return children;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFix) return;

        _timeElapsed += Time.deltaTime;

        // 0.1秒待たないとバグる
        if (_isFix)
        {
            if (_timeElapsed >= 0.1f) isFix = true;

            return;
        }

        Vector3 mov = Vector3.zero;

        // _repeatSpan秒ごとに実行 : 下に移動
        if (_timeElapsed >= _repeatSpan)
        {
            // 下に移動できなかったら固定化
            GameObject[] minoBottoms = minoSides[(6 - minoRot) % 4];
            foreach (GameObject bottom in minoBottoms)
            {
                // なぜかOverlapBoxの第一引数にnew Vector3(0, -0.01f, 0)を足さないとバグる
                Collider[] _block = Physics.OverlapBox(bottom.transform.position + new Vector3(0, -0.01f, 0), bottom.GetComponent<Collider>().bounds.size / 2);
                if (_block.Any())
                {
                    // Debug.Log(_block[0].transform.parent);
                    ImmobilizeMino();  // ミノの固定化

                    return;
                }
            }

            mov += down;

            _timeElapsed = 0;
        }

        // 左右移動
        if (Input.GetKeyDown(KeyCode.RightArrow) && isTouchRight == false)mov += right;
        if (Input.GetKeyDown(KeyCode.LeftArrow) && isTouchLeft == false)mov += left;

        // DownArrowを押したとき落下速度を変更
        if (Input.GetKeyDown(KeyCode.DownArrow)) _repeatSpan = 0.1f;

        // DownArrowを話したとき落下速度を初期化
        if (Input.GetKeyUp(KeyCode.DownArrow)) _repeatSpan = 1;

        // UpArrowを押したとき落下速度を0にする（瞬間移動）
        if (Input.GetKeyDown(KeyCode.UpArrow)) _repeatSpan = 0;

        // 移動の適用
        if (mov != Vector3.zero) this.transform.position += mov;

        Quaternion rot = Quaternion.identity;

        // 右回転
        if (Input.GetKeyDown(KeyCode.Space)) rot *= Quaternion.Euler(0, 0, -90);

        // 回転の適用
        if (rot != Quaternion.identity && ControlRot == true)
        {
            this.transform.rotation *= rot;

            minoRot = ((360 - (int)this.transform.localEulerAngles.z) / 90) % 4;
            // Debug.Log($"minoRot: {minoRot}");
        }
    }

    // ミノの固定化
    private void ImmobilizeMino()
    {
        // Minosの子要素に設定
        transform.parent = GameObject.Find("Minos").transform;

        // SideColliderとNextRotColliderを削除
        Destroy(_sideCollider);
        Destroy(_nextRotCollider);

        // BodyのBoxColliderサイズを2x2x2に設定
        BoxCollider[] _body_children = _body.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider block in _body_children)
        {
            block.size = new Vector3(2, 2, 2);
        }

        // 落下速度の初期化
        _repeatSpan = 1;

        // 固定
        _isFix = true;
    }

    // 回転の制御と補正
    // 回転可能なときはtrueを返し、必要があればミノの位置を補正
    private bool ControlRot => _controlRot();
    private bool _controlRot()
    {
        string minoType = this.gameObject.tag;

        Vector3 mov = Vector3.zero;

        if (minoType == "I")
        {
            // 特殊条件なので順序注意
            if (isTouchNextRot[1] == true)
            {
                if (isTouchNextRot[2] == false && isTouchNextRotIfMove[0] == false && isTouchNextRotIfMove[1] == false)
                {
                    if (minoRot == 0) mov += right * 2;
                    else if (minoRot == 1) mov += down * 2;
                    else if (minoRot == 2) mov += left * 2;
                    else if (minoRot == 3) mov += up * 2;
                }
                else return false;
            }
            if (isTouchNextRot[0] == true)
            {
                if (isTouchNextRot[1] == false && isTouchNextRot[2] == false && isTouchNextRotIfMove[0] == false)
                {
                    if (minoRot == 0) mov += right;
                    else if (minoRot == 1) mov += down;
                    else if (minoRot == 2) mov += left;
                    else if (minoRot == 3) mov += up;
                }
                else return false;
            }
            if (isTouchNextRot[2] == true)
            {
                if (isTouchNextRot[0] == false && isTouchNextRot[1] == false && isTouchNextRotIfMove[2] == false)
                {
                    if (minoRot == 0) mov += left;
                    else if (minoRot == 1) mov += up;
                    else if (minoRot == 2) mov += right;
                    else if (minoRot == 3) mov += down;
                }
                else return false;
            }
        }
        else if (minoType == "S")
        {
            if (isTouchNextRot[0] == true)
            {
                if (isTouchNextRot[1] == false && isTouchNextRotIfMove[0] == false)
                {
                    if (minoRot == 0) mov += right;
                    else if (minoRot == 1) mov += down;
                    else if (minoRot == 2) mov += left;
                    else if (minoRot == 3) mov += up;
                }
                else return false;
            }
            if (isTouchNextRot[1] == true)
            {
                if (isTouchNextRot[0] == false && isTouchNextRotIfMove[2] == false)
                {
                    if (minoRot == 0) mov += left;
                    else if (minoRot == 1) mov += up;
                    else if (minoRot == 2) mov += right;
                    else if (minoRot == 3) mov += down;
                }
                else return false;
            }
        }
        else if (minoType == "O")
        {

        }
        else if (minoType == "T")
        {
            if (isTouchNextRot[0] == true)
            {
                if (isTouchNextRotIfMove[0] == false && isTouchNextRotIfMove[1] == false && isTouchNextRotIfMove[2] == false)
                {
                    if (minoRot == 0) mov += up;
                    else if (minoRot == 1) mov += right;
                    else if (minoRot == 2) mov += down;
                    else if (minoRot == 3) mov += left;
                }
                else return false;
            }
        }
        else if (minoType == "Z")
        {
            if (isTouchNextRot[0] == true)
            {
                if (isTouchNextRot[1] == false && isTouchNextRotIfMove[0] == false)
                {
                    if (minoRot == 0) mov += right;
                    else if (minoRot == 1) mov += down;
                    else if (minoRot == 2) mov += left;
                    else if (minoRot == 3) mov += up;
                }
                else return false;
            }
            if (isTouchNextRot[1] == true)
            {
                if (isTouchNextRot[0] == false && isTouchNextRotIfMove[1] == false && isTouchNextRotIfMove[2] == false)
                {
                    if (minoRot == 0) mov += left;
                    else if (minoRot == 1) mov += up;
                    else if (minoRot == 2) mov += right;
                    else if (minoRot == 3) mov += down;
                }
                else return false;
            }
        }
        else if (minoType == "L")
        {
            if (isTouchNextRot[0] == true)
            {
                if (isTouchNextRotIfMove[0] == false && isTouchNextRotIfMove[2] == false && isTouchNextRotIfMove[3] == false)
                {
                    if (minoRot == 0) mov += left * 2;
                    else if (minoRot == 1) mov += up * 2;
                    else if (minoRot == 2) mov += right * 2;
                    else if (minoRot == 3) mov += down * 2;
                }
                else return false;
            }
            if (isTouchNextRot[1] == true)
            {
                if (isTouchNextRot[0] == false && isTouchNextRotIfMove[0] == false && isTouchNextRotIfMove[1] == false)
                {
                    if (minoRot == 0) mov += left;
                    else if (minoRot == 1) mov += up;
                    else if (minoRot == 2) mov += right;
                    else if (minoRot == 3) mov += down;
                }
                else return false;
            }
        }
        else if (minoType == "J")
        {
            if (isTouchNextRot[0] == true)
            {
                if (isTouchNextRotIfMove[2] == false)
                {
                    if (minoRot == 0) mov += right;
                    else if (minoRot == 1) mov += down;
                    else if (minoRot == 2) mov += left;
                    else if (minoRot == 3) mov += up;
                }
                else return false;
            }
            if (isTouchNextRot[1] == true)
            {
                if (isTouchNextRotIfMove[0] == false && isTouchNextRotIfMove[1] == false)
                {
                    if (minoRot == 0) mov += left;
                    else if (minoRot == 1) mov += up;
                    else if (minoRot == 2) mov += right;
                    else if (minoRot == 3) mov += down;
                }
                else return false;
            }
        }

        // 補正移動の適用
        this.transform.position += mov;

        return true;
    }

    // 右側にブロックがあるか
    private bool isTouchRight => _isTouchRight();
    private bool _isTouchRight()
    {
        GameObject[] minoRights = minoSides[(5 - minoRot) % 4];
        foreach (GameObject right in minoRights)
        {
            Collider[] _block =
                Physics.OverlapBox(right.transform.position + new Vector3(0.01f, 0, 0), right.GetComponent<Collider>().bounds.size / 2);
            if (_block.Any()) return true;
        }
        return false;
    }

    private bool isTouchLeft => _isTouchLeft();
    private bool _isTouchLeft()
    {
        GameObject[] minoLefts = minoSides[3 - minoRot];
        foreach (GameObject left in minoLefts)
        {
            Collider[] _block =
                Physics.OverlapBox(left.transform.position + new Vector3(-0.01f, 0, 0), left.GetComponent<Collider>().bounds.size / 2);
            if (_block.Any()) return true;
        }
        return false;
    }

    // 次の右回転時の位置でオブジェクトと接触しているかの判定
    public void NextRot_EnterEventHandler(GameObject me) => isTouchNextRot[int.Parse(me.name)] = true;
    public void NextRot_ExitEventHandler(GameObject me) => isTouchNextRot[int.Parse(me.name)] = false;

    public void NextRotIfMove_EnterEventHandler(GameObject me) => isTouchNextRotIfMove[int.Parse(me.name)] = true;
    public void NextRotIfMove_ExitEventHandler(GameObject me) => isTouchNextRotIfMove[int.Parse(me.name)] = false;

}
