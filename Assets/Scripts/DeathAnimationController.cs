using UnityEngine;

public class DeathAnimationController : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemyController = animator.gameObject.GetComponent<EnemyController>();
        if (enemyController != null)
            enemyController.Respawn();
    }
}
