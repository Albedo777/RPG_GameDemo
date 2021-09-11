using UnityEngine;
using Cinemachine;
using UnityEngine.AI;

/**
 * 渲染和动画相关组件
 * @author ligzh
 * @date 2021/9/2
 */

public class RenderComponent : IComponent, ITransmit
{
    private UnitBase unit;
    private Animator animator;
    private Rigidbody rb;
    private CinemachineFreeLook freelookCamera;
    private CinemachineCameraOffset cameraOffset;

    private float walkSpeed = 4.0f;

    public void OnInitialize(UnitBase unit)
    {
        this.unit = unit;
    }

    public void OnBirth()
    {
        GameObject renderer =
            ResourceManager.Instance.LoadGameObjectResource(FactoryType.GameFactory, unit.Data.modelName);
        renderer.transform.SetParent(unit.gameObject.transform);
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localEulerAngles = Vector3.zero;
        animator = renderer.GetComponentInChildren<Animator>();
        rb = renderer.GetComponent<Rigidbody>();
        unit.Data.unitTrans = renderer.transform;
        if (unit.Data.id == 1)
        {
            CinemachineInit();
        }
    }

    public void OnDead()
    {
    }

    public void OnUpdate()
    {
    }

    public void OnTransmit(ETransmitType type, BaseTransmitArg arg)
    {
        if (type == ETransmitType.Walk)
        {
            NormalWalk((WalkArg) arg);
        }

        if (type == ETransmitType.Attack)
        {
            animator.SetTrigger("Attack");
        }

        if (type == ETransmitType.AutoMove)
        {
            if (unit.Data.isDead)
            {
                return;
            }

            unit.Data.unitTrans.GetComponent<NavMeshAgent>()
                .SetDestination(UnitManager.Instance.GetPlayerUnit().Data.unitTrans.position);
        }

        if (type == ETransmitType.SkillRelase)
        {
            animator.SetTrigger("Attack");
        }

        if (type == ETransmitType.BeHit)
        {
            animator.SetTrigger("Damage");
        }

        if (type == ETransmitType.Dead)
        {
            animator.SetTrigger("Dead");
        }
    }

    private void NormalWalk(WalkArg arg)
    {
        var forwardAmount = arg.forwardAmount;
        var moveDir = arg.moveDir;
        float speed = forwardAmount;
        rb.MovePosition(unit.Data.unitTrans.position + moveDir * walkSpeed * Time.fixedDeltaTime);
        animator.SetFloat("Speed", forwardAmount, 0.02f, Time.fixedDeltaTime);
        animator.SetFloat("AbsSpeed", Mathf.Abs(speed));
        if (moveDir != Vector3.zero)
        {
            unit.Data.unitTrans.rotation = Quaternion.RotateTowards(unit.Data.unitTrans.rotation,
                Quaternion.LookRotation(moveDir, Vector3.up), 1000 * Time.deltaTime);
        }

        animator.SetFloat("Direction", arg.turnAmount, 0.02f, Time.fixedDeltaTime);
    }

    #region 虚拟相机设置

    private void CinemachineInit()
    {
        Camera.main.gameObject.AddComponent<CinemachineBrain>();
        var virtualCameraObj = new GameObject("VirtualCamera");
        freelookCamera = virtualCameraObj.AddComponent<CinemachineFreeLook>();
        cameraOffset = freelookCamera.gameObject.AddComponent<CinemachineCameraOffset>();
        SetCinemachineTarget(unit.Data.unitTrans.transform);
    }

    private void SetCinemachineTarget(Transform unitTrans)
    {
        if (freelookCamera == null)
        {
            return;
        }

        freelookCamera.LookAt = unitTrans;
        freelookCamera.Follow = unitTrans;
        freelookCamera.m_Lens.FieldOfView = 45;
        freelookCamera.m_YAxis.m_InvertInput = true;
        freelookCamera.m_YAxis.m_Recentering.m_enabled = false;
        freelookCamera.m_XAxis.m_InvertInput = false;
        freelookCamera.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
        freelookCamera.m_YAxis.m_MaxSpeed = 0;
        freelookCamera.m_XAxis.m_MaxSpeed = 0;
        for (int i = 0; i <= 2; i++)
        {
            freelookCamera.m_Orbits[i] = new CinemachineFreeLook.Orbit(5, 5);
            freelookCamera.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XDamping = 0;
            freelookCamera.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_YDamping = 0;
            freelookCamera.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = 0;
        }

        cameraOffset.m_ApplyAfter = CinemachineCore.Stage.Body;
        cameraOffset.m_Offset.y = -2;
    }

    #endregion
}