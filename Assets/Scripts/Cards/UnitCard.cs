using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Атакующие")]
public class UnitCard : BasicCard
{
    public override void OnClick()
    {
        if(!CheckSteps()) return;
    }
}
