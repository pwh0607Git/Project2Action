using System.Collections;
using UnityEngine;

public class SpawnerEnemy : Spawner
{
    [SerializeField] EventEnemySpawnBefore eventEnemySpawnBefore;
    [SerializeField] EventEnemySpawnAfter eventEnemySpawnAfter;

    [Space(10)]
    CharacterControl control;

    void OnEnable()
    {
        // 이벤트가 등록 되면 발동, 등록 안하면 작동 안함.  [트리거]
        eventEnemySpawnBefore?.Register(OnEventEnemySpawnBefore);
    }

    void OnDisable()
    {
        // 이벤트가 등록 되면 발동, 등록 안하면 작동 안함.
        eventEnemySpawnBefore?.Unregister(OnEventEnemySpawnBefore);
    }


    void OnEventEnemySpawnBefore(EventEnemySpawnBefore e){

        control = Instantiate(e.enemyCharacter);
        control.transform.SetPositionAndRotation(spawnPoint.position, Quaternion.LookRotation(transform.forward));
        control.Profile = actorProfile;
        StartCoroutine(SpawnAfter());
    }

    IEnumerator SpawnAfter(){
        yield return new WaitForEndOfFrame();

        eventEnemySpawnAfter.character = control;
        eventEnemySpawnAfter.eyePoint = control.eyePoint;
        eventEnemySpawnAfter?.Raise();
    }
}