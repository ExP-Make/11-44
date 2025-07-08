using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatManager : PersistentSingleton<CombatManager>
{
    public event Action OnFloatStart;


    private void Start()
    {
        OnFloatStart?.Invoke();
        Debug.Log("KK");
    }

    // 특정 조건에 의해 실행되는 StartCombat
    public void StartCombat()
    {
        OnFloatStart?.Invoke();
        
        // TODO
        // 카메라 줌아웃 후 고정
        // 보스 생성
        // 컷씬 등등..
    }
}
