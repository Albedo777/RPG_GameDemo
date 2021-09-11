using UnityEngine;

/**
 * 控制移动组件
 * @author ligzh
 * @date 2021/9/2
 */

public class MoveComponent : IComponent, ITransmit
{
    private UnitBase unit;
    private GameObject renderer;
    private float groundCheckDistance = 0.4f;
    private Vector3 groundNormal;

    public void OnInitialize(UnitBase unit)
    {
        this.unit = unit;
    }

    public void OnBirth()
    {
    }

    public void OnDead()
    {
    }

    public void OnUpdate()
    {
        CheckGroundStatus();
        CheckWalk();
        CheckJump();
        CheckRun();
    }

    private void CheckWalk()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDir = (vertical * Vector3.forward + horizontal * Vector3.right).normalized;
        Vector3 localMoveDir = unit.gameObject.transform.InverseTransformDirection(moveDir);
        localMoveDir = Vector3.ProjectOnPlane(localMoveDir, groundNormal);
        float turnAmount = Mathf.Atan2(localMoveDir.x, localMoveDir.z);
        float forwardAmount = localMoveDir.z;
        if (horizontal != 0 || vertical != 0)
        {
            //有输入时进入移动状态
            var walkArg = ClassManager.Instance.Get<WalkArg>();
            walkArg.forwardAmount = forwardAmount;
            walkArg.turnAmount = turnAmount;
            walkArg.moveDir = moveDir;
            unit.Transmit(ETransmitType.Walk, walkArg);
            ClassManager.Instance.Free(walkArg);
        }
        else
        {
            //无输入时进入停滞状态
            var walkArg = ClassManager.Instance.Get<WalkArg>();
            walkArg.forwardAmount = 0;
            walkArg.turnAmount = 0;
            walkArg.moveDir = Vector3.zero;
            unit.Transmit(ETransmitType.Walk, walkArg);
            ClassManager.Instance.Free(walkArg);
        }
    }

    private void CheckGroundStatus()
    {
        if (renderer == null)
        {
            return;
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(renderer.transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo,
            groundCheckDistance))
        {
            groundNormal = hitInfo.normal;
        }
        else
        {
            groundNormal = Vector3.up;
        }
    }

    private void CheckJump()
    {
    }

    private void CheckRun()
    {
    }

    public void OnTransmit(ETransmitType type, BaseTransmitArg arg)
    {
    }
}