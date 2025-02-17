using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    public int hp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Damage(int damage)
    {
        hp -= damage;

        if(hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
