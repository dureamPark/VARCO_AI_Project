using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : StateCore
{
    public Patrol patrol;
    private void Start()
    {
        SetupInstance();
        machine.Set(patrol);
    }

    private void Update()
    {
        if (state.isComplete)
        {

        }
        state.DoBranch();
    }

    private void FixedUpdate()
    {
        state.FixedDoBranch();
    }
}
