using System.Collections;
using System.Linq;
using UnityEngine;

public class CharacterEventControl : MonoBehaviour
{
#region Events
    [SerializeField] EventCameraSwitch eventCameraSwitch;
    [SerializeField] EventPlayerSpawnAfter eventPlayerSpawnAfter;
    [SerializeField] EventDeath eventDeath;
    [SerializeField] EventAttackAfter eventAttackAfter;

    [SerializeField] EventSensorSightEnter eventSensorSightEnter;
    [SerializeField] EventSensorSightExit eventSensorSightExit;

    [SerializeField] EventCursorHover eventCursorHover;
#endregion
    
    private CharacterControl owner;
    void Start()
    {
        if(!TryGetComponent(out owner)) Debug.LogWarning("GameEventControl - controllerControl 없음...");
        owner.Visible(false);
    }

    void OnEnable()
    {
        eventPlayerSpawnAfter.Register(OnEventPlayerSpawnAfter);
        eventCameraSwitch.Register(OnEventCameraSwitch);
        eventAttackAfter.Register(OnEventAttackAfter);
        eventDeath.Register(OnEventDeath);

        eventSensorSightEnter.Register(OnEventSensorSightEnter);
        eventSensorSightExit.Register(OnEventSensorSightExit);

        eventCursorHover.Register(OnEventCursorHover);
    }

    void OnDisable()
    {
        eventPlayerSpawnAfter.Unregister(OnEventPlayerSpawnAfter);
        eventCameraSwitch.Unregister(OnEventCameraSwitch);
        eventAttackAfter.Unregister(OnEventAttackAfter);
        eventDeath.Unregister(OnEventDeath);
        
        
        eventSensorSightEnter.Unregister(OnEventSensorSightEnter);
        eventSensorSightExit.Unregister(OnEventSensorSightExit);

        eventCursorHover.Unregister(OnEventCursorHover);
    }

    void OnEventCameraSwitch(EventCameraSwitch e){
        if(e.inout)
            owner.abilityControl.Deactivate(AbilityFlag.MoveKeyboard);
        else
            owner.abilityControl.Activate(AbilityFlag.MoveKeyboard, false, null);
    }

    void OnEventPlayerSpawnAfter(EventPlayerSpawnAfter e){
        StartCoroutine(SpawnSequence(e));
    }
    
    IEnumerator SpawnSequence(EventPlayerSpawnAfter e){
        yield return new WaitUntil(() => owner.Profile.avatar != null && owner.Profile.models != null);

        owner.Profile = owner.Profile;

        // 플레이어 모델 생성후 하위 항목인 Model에 설정
        if(owner.Profile.models == null)
            Debug.LogError("CharacterEventControl ] model 없음.");

        var model = owner.Profile.models.Random();

        var clone = Instantiate(model, owner.model);

        var feedback = clone.GetComponentInChildren<FeedbackControl>();
        if(feedback != null){
            owner.feedbackControl = feedback;
        }

        clone.GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach( m =>{
            m.gameObject.layer = LayerMask.NameToLayer("Silhouette");
        });

        if(owner.Profile.avatar == null)
            Debug.LogError("CharacterEventControl ] avatar 없음.");
        owner.animator.avatar = owner.Profile.avatar;

        yield return new WaitForSeconds(1f);

        owner.Visible(true);
        owner.PlayAnimation("SPAWN", 0f);

        PoolManager.I.Spawn(e.spawnParticle, transform.position, Quaternion.identity, null);
    
        yield return new WaitForSeconds(1f);

        foreach( var dat in owner.Profile.abilities)
            owner.abilityControl.Add(dat, true);
    }

    #region DAMAGE
    void OnEventAttackAfter(EventAttackAfter e){
        if(owner != e.to) return;              // e.to는 Player .. 피격자[공격을 받는 사람은]

        owner.abilityControl.Activate(AbilityFlag.Damage, false, e);             
        

    }
    #endregion

    #region Attack
    #endregion

    #region Death
    void OnEventDeath(EventDeath e){
        if(owner != e.target) return;
        
        owner.PlayAnimation("DEATH", 0.2f);
        owner.abilityControl.RemoveAll();
    }
    #endregion


    #region Sight
    void OnEventSensorSightEnter(EventSensorSightEnter e){
        if(owner != e.from) return;
    }

    void OnEventSensorSightExit(EventSensorSightExit e){

    }
    #endregion


    #region CursorHover
    // 커서가 호버된 타겟을 쳐다보는 이벤트.
    void OnEventCursorHover(EventCursorHover e){

    }
    #endregion
}

// 비동기(async)
/*
    1. 코루틴   
    2. Invoke
    3. async / await [UniTask]
    4. Awaitable
    5. CySharp - Unitask
    6. DoTween - DoVirtual.Delay(3f, () => {...})
*/
// 유니티는 단일 스레드 엔진이다.
// => 비동기라고 표현하지만 사실방 코어가 빠르게 번갈아가며 처리하는 것! [병렬 수행하는 것처럼 보이게 하는 것.]