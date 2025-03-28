using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

public class DOTSEventManager : MonoBehaviour
{
public static DOTSEventManager Instance { get; private set; }

    public event EventHandler onBarracksUnitQueueChanged;
    public event EventHandler onHQDead;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerOnBarracksUnitQueueChanged(NativeList<Entity> entityNativeList)
    {
        foreach (Entity entity in entityNativeList) 
        {
            onBarracksUnitQueueChanged?.Invoke(entity, EventArgs.Empty);
        }
    }

    public void TriggerOnHQDead()
    {
        onHQDead?.Invoke(this, EventArgs.Empty);
    }


}
