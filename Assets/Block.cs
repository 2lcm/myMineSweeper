using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    int number;
    // 0 : basic, 1 : safe, 2 : suspected(flag), 3 : mine
    public int state;
    public bool selected;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
