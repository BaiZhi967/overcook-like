using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;
    public static void ResetStaticData() {
        OnAnyPlayerSpawned = null;
    }
    public static Player LocalInstance { get; private set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private LayerMask countersLayMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spwanPositionList;
    [SerializeField] private PlayerVisuall playerVisuall;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake() {
        //Instance = this;
    }

    private void Start() {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisuall.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = spwanPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) {
            return;
        }

        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) {
            return;
        }

        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void Update() {
        if (!IsOwner)
        {
            return;
        }
        
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        // 移动方向
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayMask)) {
            // 检测到前方有橱柜
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                //clearCounter.Interact();
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }
            }
            else {
                // 检测到前方有东西，但不是 BaseCounter
                SetSelectedCounter(null);
            }
        }
        else {
            // 前面没有东西
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement() {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        // 移动方向
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        // 检测前方是否有碰撞体，被检测到的物体需要具有碰撞体
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one*playerRadius, moveDir, Quaternion.identity,moveDistance, collisionsLayerMask);

        if (!canMove) {
            // 不能朝 moveDir 方向移动
            // 尝试仅朝 x 轴方向移动，由于只朝一个分量移动，那么分量一定 <= 1，所以需要再次进行归一化，否则撞墙后会出现明显减速，造成手感不好
            // 好家伙破案了，所以王者就没有再次进行归一化处理？我经常被追到墙角感觉跑得慢了！！！
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0).normalized;
            // 此处加了 moveDir.x != 0 的判断，为了分离出 moveDir.x = 0 且 moveDir.z = 0 的情况，使 Player 在停下时可以略微转向
            // 也就是转向时有一个 (moveDir.x, 0f, 0) 或者 (moveDir.z, 0f, 0) 逐渐变为 (0, 0, 0) 的过程，当然效果并不是特别明显
            // 改变 moveDir.x 的判断条件是因为在使用手柄时不容易做到完全向一个方向移动，再经过归一化很容易会在碰到墙壁时出现贴着墙壁走的现象，此时我们希望将出现误差的方向的移动忽略掉，例如使用手柄出现了一个方向 (0.0001,0.9)
            canMove = (moveDir.x < -.5f || moveDir.x > .5f) && !Physics.BoxCast(transform.position, Vector3.one*playerRadius, moveDirX, Quaternion.identity,moveDistance, collisionsLayerMask);

            if (canMove) {
                // 能够仅朝 x 轴方向移动
                moveDir = moveDirX;
            }
            else {
                // 不能仅朝 x 轴方向移动
                // 尝试仅朝 z 轴方向移动
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                // 此处加了 moveDir.z != 0 的判断，为了分离出 moveDir.x = 0 且 moveDir.z = 0 的情况，使 Player 在停下时可以略微转向
                // 也就是转向时有一个 (moveDir.x, 0f, 0) 或者 (moveDir.z, 0f, 0) 逐渐变为 (0, 0, 0) 的过程，当然效果并不是特别明显
                // 改变 moveDir.z 的判断条件是因为在使用手柄时不容易做到完全向一个方向移动，再经过归一化很容易会在碰到墙壁时出现贴着墙壁走的现象，此时我们希望将出现误差的方向的移动忽略掉，例如使用手柄出现了一个方向 (0.9,0.0001)
                canMove = (moveDir.z < -.5f || moveDir.z > .5f) && !Physics.BoxCast(transform.position, Vector3.one*playerRadius, moveDirZ, Quaternion.identity,moveDistance, collisionsLayerMask);
                if (canMove) {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveDistance;
        }

        // 每个设备的帧率是不同的，Update 是每一帧都调用，部分设备由于帧率高导致别人一帧的时间它的设备已经跑了 n 多帧
        // 这就导致同一段时间内，一个设备上的 Player 只移动了一小段距离，另一个设备上的 Player 已经一下窜出去没有影子了。
        // 所以需要用到 deltaTime，它是从上一帧到当前帧的间隔，以秒为单位。这就保证了每一帧只移动上一帧到这一帧时间应该走过的距离。
        //transform.position += moveDir * Time.deltaTime * moveSpeed;

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        // 使用 Slerp() 更平滑的转动 Player 的朝向
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return (kitchenObject != null);
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
