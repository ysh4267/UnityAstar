using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeObject : MonoBehaviour
{
    public bool isWall = false;
    
    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Wall"))
        {
            isWall = true;
        }
    }
}
