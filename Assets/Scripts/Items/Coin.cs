using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IItem
{
    public int score = 200;

    public void Use(GameObject target)
    {
        GameManager.Instance.AddScore(score);
        Destroy(gameObject);
    }
}
