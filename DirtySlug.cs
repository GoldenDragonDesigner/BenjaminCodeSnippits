using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*Author:  Benjamin Boese
 * Date: 4-13-2019
 * Purpose: Creating a Slug AI
 * Credit: The Game Development Class
 * */

public class DirtySlug : FSGDN.StateMachine.MachineBehaviour
{
    [SerializeField]
    NavPoint[] slugNavPoints;

    [SerializeField]
    int navIndex = 0;

    public override void AddStates()
    {
        AddState<Slug_Dirty_State>();
        AddState<Slug_Idle_State>();
        //AddState<Slug_Dirty_Finished_State>();
        
        SetInitialState<Slug_Dirty_State>();
    }

    public void PickNextNavPoint()
    {
        navIndex++;

        if(navIndex >= slugNavPoints.Length)
        {
            navIndex = 0;
            //if(navIndex == 0)
            //{
            //    ChangeState<Slug_Dirty_Finished_State>();
            //}
        }
    }

    public void FindDestination()
    {
        GetComponent<NavMeshAgent>().SetDestination(slugNavPoints[navIndex].transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<NavPoint>())
        {
            ChangeState<Slug_Idle_State>();
        }
        //if(other.gameObject.GetComponent<NavPoint>())
        //{
        //    ChangeState<Slug_Dirty_Finished_State>();
        //}
    }
}





public class SlugState : FSGDN.StateMachine.State
{
    protected DirtySlug SlugStateMachine()
    {
        return ((DirtySlug)machine);
    }
}

public class Slug_Dirty_State : SlugState
{
    public override void Enter()
    {
        base.Enter();
        SlugStateMachine().FindDestination();
    }
}

//public class Slug_Dirty_Finished_State : SlugState
//{
//    public override void Enter()
//    {
//        base.Enter();
//        SlugStateMachine().DoneTrackingDirt();
//    }
//}

public class Slug_Idle_State : SlugState
{
    float timer = 0;

    [SerializeField]
    int navNum = 0;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
    }

    public override void Execute()
    {
        //base.Execute();
        timer += Time.deltaTime;
        if(timer >= 2.0f && navNum <= 0)
        {
            machine.ChangeState<Slug_Dirty_State>();
            SlugStateMachine().PickNextNavPoint();
        }
        //else
        //{
        //    machine.ChangeState<Slug_Dirty_Finished_State>();
        //}
    }
}

