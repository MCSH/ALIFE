using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{

    private Master master;
    private float timer = 8;
    // Start is called before the first frame update
    void Start()
    {
        master = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<Master>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!master.paused) timer -= Time.deltaTime;
        if(timer <= 0) Destroy(this.gameObject);
    }
}
