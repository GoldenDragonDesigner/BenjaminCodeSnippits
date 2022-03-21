using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*Author: Benjamin Boese
 * Date: 3-21-2019
 * Credit: Experiements from the last two weeks
 * Purpose: to create a state machine for the guards
 */
public class BB_GuardStateMachine : FSGDN.StateMachine.MachineBehaviour
{
    //The Guard has Patrol, they are green with a yellow hew to identify whether they have been selected. They are Green
    //going in ascending order in the navpoint array and Blue going in descending order through the navpoint array.
    //The Guard Idle, they are Dark green when at a NavPoint, waiting 2 seconds before moving into the next state.
    //The Guard Angry, they are Red when moving towards the alarm in the room.  When reacing the alarm change back to 
    //patrol state.

    //Creating a serialized Field array for the Guard navigation points they are patrolling between
    [SerializeField]
    NavPoint[] guardNavPoints;

    //Creating a serialized Field for when the Guard is highlighted
    [SerializeField]
    GameObject guardHighlight;

    //creating a serialized field for the alarmnode
    [SerializeField]
    GameObject[] alarmNode;

    int alarmIndex = 0;


    //This is the int for the navigation index and it is starting at 0.
    int navIndex = 0;

    //This is for when adding a state that i have created
    public override void AddStates()
    {
        AddState<Guard_Patrol_State>();
        AddState<Guard_Idle_State>();
        AddState<Guard_Pause_State>();
        AddState<Guard_Highlight_State>();
        AddState<Guard_Angry_State>();
        AddState<Guard_Pursue_State>();

        SetInitialState<Guard_Patrol_State>();
    }


    //this is a function that is picking the next nav point
    public void PickNextNavPoint()
    {
        //this incriments the nav point array
            navIndex++;
        //this is theif check saying if the nav index is greater than or equal to 
        //the guards patroling points array (the length)
            if(navIndex >= guardNavPoints.Length)
            {
        //set the index back to zero
                navIndex = 0;
            }
    }

    public void FindAlarmPoint()
    {
        alarmIndex++;

        if(alarmIndex >= 1)
        {
            alarmIndex = 0;
        }
    }

    //this is a function for finding the next destination of nav points
    public void FindDestination()
    {
        //this is getting the component from the nav mesh agent component on the inpsector, setting the destination of the 
        //guard Nav points
        GetComponent<NavMeshAgent>().SetDestination(guardNavPoints[navIndex].transform.position);
    }

    public void FindAlarmNode()
    {
        GetComponent<NavMeshAgent>().SetDestination(alarmNode[alarmIndex].transform.position);
    }

    //this is a trigger function for when the guard enters the nav point and is triggered to the idle state
    void OnTriggerEnter(Collider other)
    {
        //this is if the game object Nav point script is true meaning it was triggered
        if (other.gameObject.GetComponent<NavPoint>())
        {
            //change the state of the guard to idle
            ChangeState<Guard_Idle_State>();
        }
    }

    //this is a helper function for setting the object color
    public void SetMainColor(Color color)
    {
        //this is getting the renderer component on the inspector and assigning it to the parameters in 
        //the parathensis from above
        GetComponent<Renderer>().material.color = color;
    }

    //this is a bool for when a key press happens and it becomes either true or false
    //it is currently set to false
    bool paused = false;

    //this is saying that the namespace FSGDNs state machines state called lastState is set to null or nothing
    FSGDN.StateMachine.State lastState = null;

    //This function is pausing the guard when the space bar is pressed in the guard manager
    public void Pause()
    {
        //toggling the pause value
        paused = !paused;

        //this if state is saying that if paused is false
        if (paused)
        {
            //storing current state for use when unpausing 
            lastState = currentState;

            //change the state to pause
            ChangeState<Guard_Pause_State>();
            GetComponent<NavMeshAgent>().isStopped = true;
        }

        else
        {
            //restore stored state from pausing earlier
            ChangeState(lastState.GetType());
            GetComponent<NavMeshAgent>().isStopped = false;
        }
    }

    //this is a private bool for the current selected object
    private bool currentlyHighlighted;
    //this is a private color reference for the color being used
    private Color previousColor;

    //this is the public function for setting the highlight of the disk
    public void SetHightlight(bool highlightOn)
    {
        //this references the private bool to the bool in the parameter of the function
        currentlyHighlighted = highlightOn;

        //the if statement for whether or not the parameter bool is true or not
        if (currentlyHighlighted)
        {
            //referencing and storing the previous color
            previousColor = guardHighlight.GetComponent<Renderer>().material.color = Color.grey;

            //This calls the object and highlights it yellow
            guardHighlight.GetComponent<Renderer>().material.color = Color.yellow;
        }

        else
        {
            //this resets the previous color which was grey
            guardHighlight.GetComponent<Renderer>().material.color = previousColor;
        }
    }

    //this is a private bool for when the guard is in an angry state
    bool isAngry = false;
    //this is a helper function that is for the guard getting andry and turring red
   public void AngryState()
   {
        isAngry = !isAngry;
        if(isAngry == true)
        {
            //this is changing the state of the guard to Angry from patrol or idle
            ChangeState<Guard_Angry_State>();
        }
        else
        {
            //if the guard is not in an angry state they are in the patrol state
            ChangeState<Guard_Patrol_State>();
        }
   }

    /*Author: Benjamin Boese
 * Date: 3-21-2019
 * Credit: the https://unity3d.com/learn/tutorials/topics/navigation/nav-meshes for pursuing a player
 * Purpose: to create a way for the guard to pursue the player
 */
    [SerializeField]
    Transform playerAgent;
    public bool isPlayer;
    int maxDist = 10;
    int minDist = 5;

    public void Pursue()
    {

        ChangeState<Guard_Pursue_State>();
        if(maxDist <= 10)
        {
            isPlayer = true;
            GetComponent<NavMeshAgent>().SetDestination(playerAgent.position);
        }
        else
        {
            isPlayer = false;
            ChangeState<Guard_Patrol_State>();
        }
    }
    //this is a private bool that is declared naming it forward and is set as true
    bool forward = true;

    //this is the helper function for the guard moving forward or Reverse depending on
    //the key press
    public void MovingForwardOrReverse(bool isForward)
    {
        //this is saying forward is true is set to forward is not true.
        forward = isForward;

        if (forward)
        { 
            ChangeState<Guard_Patrol_State>();
        }
        else
        {
            SetMainColor(Color.blue);
            
        }


    }
    public override void Update()
    {
        base.Update();
    }

}








//this is a new base class for NavAgent States that will give us some utility
//functions
public class GuardNavAgentState : FSGDN.StateMachine.State
{
    //this is an accessor for getting our state machine script reference
    protected BB_GuardStateMachine GuardStateMachine()
    {
        return ((BB_GuardStateMachine)machine);
    }
}

//this is the state for the guard patrolling
public class Guard_Patrol_State : GuardNavAgentState
{
    public override void Enter()
    {
        base.Enter();
            GuardStateMachine().SetMainColor(Color.green);
            GuardStateMachine().FindDestination();
    }
}

//this is the state for the guard when they are idle
public class Guard_Idle_State : GuardNavAgentState
{
    float timer = 0;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        GuardStateMachine().SetMainColor(new Color(0.0f, 0.5f, 0.0f));
    }

    public override void Execute()
    {
        //base.Execute();
        timer += Time.deltaTime;
        if(timer >= 2.0f)
        {
            machine.ChangeState<Guard_Patrol_State>();
            GuardStateMachine().PickNextNavPoint();
        }
    }
}

//this is the state when the guard is Paused
public class Guard_Pause_State : GuardNavAgentState
{
    public override void Enter()
    {
        base.Enter();
        GuardStateMachine().SetMainColor(Color.grey);
    }
}

//this is the state when the guard is selected
public class Guard_Highlight_State : GuardNavAgentState
{
    public override void Enter()
    {
        base.Enter();
        GuardStateMachine().SetMainColor(Color.yellow);
    }
}

//this is the state when the guard is angry
public class Guard_Angry_State : GuardNavAgentState
{

    public override void Enter()
    {
        base.Enter();
        GuardStateMachine().SetMainColor(Color.red);
        GuardStateMachine().FindAlarmNode();
    }
}

public class Guard_Pursue_State : GuardNavAgentState
{

    public override void Enter()
    {
        base.Enter();
        GuardStateMachine().SetMainColor(Color.cyan);
    }
}
