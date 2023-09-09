using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    static public Transform Boss_tf;
    void Start()
    {

    }

    void Update()
    {
        if (Boss_tf != null)
        {
            transform.position = Boss_tf.position;
        }
    }

    private void OnDestroy()
    {
        Boss_tf = null;
    }
}
