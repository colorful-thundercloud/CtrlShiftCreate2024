using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "�����/���������")]
public class UnitCard : BasicCard
{
    public override void OnClick()
    {
        if(!CheckSteps()) return;
    }
}
