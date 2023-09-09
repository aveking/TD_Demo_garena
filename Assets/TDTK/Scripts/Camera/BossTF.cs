using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTF : MonoBehaviour
{
    void Start()
    {
        FollowCamera.Boss_tf = transform;
    }

    void Update()
    {
    }

    private void OnDestroy()
    {
        FollowCamera.Boss_tf = null;
    }
}
