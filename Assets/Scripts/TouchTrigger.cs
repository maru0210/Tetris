using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class TouchTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // ���̃X�N���v�g���A�^�b�`���ꂽGameObject�ɁACollider���ڐG�����onColliderEnter�C�x���g�𔭐�������
    public UnityEvent<Collider> onColliderEnter;

    private void OnCollisionEnter(Collision collision)
    {
        onColliderEnter.Invoke(collision.collider);
    }
}
