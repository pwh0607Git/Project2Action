using System;
using CustomInspector;
using UnityEngine;

public class CursorSelectable : MonoBehaviour
{
    [HideInInspector] public CharacterControl target;

    public CursorType type;
    public Renderer[] rds;
    [Tooltip("Outline Material")]
    public Material selectableMaterial;

    [Tooltip("Outline Thickness")]
    public float selectableThickness = 0.05f;

    [ReadOnly] public bool on;

    //모델이 생성된 후에 호출.
    public void SetupRenderer(){
        if(rds.Length > 0) return;

        // Character Control이 있으면 사용 없으면 무시.
        target = GetComponentInParent<CharacterControl>();

        rds = GetComponentsInChildren<SkinnedMeshRenderer>();

        if(rds.Length <= 0)
            rds = GetComponentsInChildren<MeshRenderer>();
    }

    public void Select(bool on)
    {
        if(rds == null || rds.Length <=0) return;

        foreach(var rd in rds){
            string layername = on ? "Outline" : "Default";
            
            this.on = on;
            rd.gameObject.layer = LayerMask.NameToLayer(layername);

            if(selectableMaterial != null)
                selectableMaterial.SetFloat("_Thickness", selectableThickness);
        }
    }
}