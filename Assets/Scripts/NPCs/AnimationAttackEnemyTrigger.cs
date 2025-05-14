using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttackEnemyTrigger : MonoBehaviour
{
    public Enemy enemy;

    public void Attack()
    {
        enemy.AttackReciever();
    }
}
