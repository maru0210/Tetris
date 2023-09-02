using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Master : MonoBehaviour
{
    [SerializeField] GameObject[] minos;
    [SerializeField] GameObject _leftBottom;

    [SerializeField] TextMeshProUGUI _deleteLinesNum;
    [SerializeField] TextMeshProUGUI _timer;

    // 線形合同法による乱数
    private int x, a, b, m;

    // 現在操作中のミノ
    private Minos currentMino;
    private int countMinos = 0;

    // ミノを生成するポジション
    private Vector3 origin;

    // 移動量
    private Vector3 up = new Vector3(0, 2, 0);
    private Vector3 down = new Vector3(0, -2, 0);
    private Vector3 right = new Vector3(2, 0, 0);

    private int countDeleteLine = 0;
    private float Timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        Screen.fullScreen = false;
        
        origin = this.transform.position;
        m = minos.Length; // 線形合同法乱数の法

        createMino();
    }

    // Update is called once per frame
    void Update()
    {
        UIupdate();

        if (currentMino.isFix == true)
        {
            // 消去列の確認
            deleteLine();
            createMino();
        }
    }

    private void UIupdate()
    {
        Timer += Time.deltaTime;
        _timer.text = $"Time :  {((int)Timer / 60).ToString("D2")} : {((int)Timer % 60).ToString("D2")}.{(int)(Timer * 10) % 10}";
        _deleteLinesNum.text = $"DeleteLine :  {countDeleteLine}";
    }

    private void createMino()
    {
        currentMino = Instantiate(minos[rand], origin, Quaternion.identity).GetComponent<Minos>();

        // 出現位置の調整
        string minoType = currentMino.tag;
        if (minoType == "T" || minoType == "Z") currentMino.transform.position += new Vector3(1, -1, 0);

        currentMino.name = $"mino_{countMinos}";

        countMinos++;
    }

    // 線形合同法による乱数生成
    private int rand => _rand();
    private int _rand()
    {
        if (countMinos % m == 0)
        {
            x = Random.Range(0, m);
            a = 8;
            b = Random.Range(1, m);

            return x;
        }
        else
        {
            x = (x * a + b) % m;
            return x;
        }
    }

    private void deleteLine()
    {
        // ステージブロックの取得
        GameObject[,] stageBlock = new GameObject[20, 10];
        for (int i = 0; i < 20; i++)
        {
            string name = $"{i} : ";
            for (int j = 0; j < 10; j++)
            {
                Collider[] _block = Physics.OverlapBox(_leftBottom.transform.position + up * i + right * j, new Vector3(0.5f, 0.5f, 0.5f));
                stageBlock[i, j] = _block.Any() ? _block[0].gameObject : null;

                name += _block.Any() ? $"{_block[0].name}, " : "null, ";
            }
            // Debug.Log(name);
        }

        // ブロックの削除
        for (int i = 0; i < 20; i++)
        {
            bool isDelete = true;

            for (int j = 0; j < 10; j++)
            {
                if (stageBlock[i, j] == null)
                {
                    isDelete = false;
                    break;
                }
            }
            if (isDelete == false) continue;

            countDeleteLine++;
            for (int j = 0; j < 10; j++) Destroy(stageBlock[i, j]);

            for (int k = i + 1; k < 20; k++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (stageBlock[k, j] != null) stageBlock[k, j].transform.position += down;
                }
            }
        }
    }
}
