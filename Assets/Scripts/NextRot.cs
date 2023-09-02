using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NextRot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public UnityEvent<GameObject> onTriggerEnter;
    public UnityEvent<GameObject> onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(this.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit.Invoke(this.gameObject);
    }

}
