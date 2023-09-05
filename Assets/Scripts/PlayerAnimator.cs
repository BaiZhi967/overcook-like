using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour {
    // const 使变量固定，不再可变，避免后面的代码中错误改变 IS_WALKING 的值
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private Player player;
    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if(!IsOwner)return;
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
