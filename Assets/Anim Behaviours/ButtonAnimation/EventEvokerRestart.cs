using UnityEngine;

public class EventEvokerRestart : StateMachineBehaviour
{
    float cnt = 0;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cnt = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cnt += Time.deltaTime;
        if(cnt >= stateInfo.length)
        {
            if (GlobalManager.Instance.saves[0] != null)
            {
                GlobalManager.Instance.LoadGame(GlobalManager.Instance.saves[0]);
            }
        }
    }
}
