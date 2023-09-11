using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class global_gamesetting : MonoBehaviour
{
    public float tower_attack_rate = 1f;
    public float tower_bullet_fly_speed = 1f;
    public float boss_move_speed = 1f;

    static public global_gamesetting _inst;
    // Start is called before the first frame update
    void Start()
    {
        _inst = this;
    }

    private void OnDestroy()
    {
        _inst = null;
    }

}
