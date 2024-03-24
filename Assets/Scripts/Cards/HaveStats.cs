using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HaveStats : MonoBehaviour
{
    [SerializeField] protected TMP_Text damage, hp, title;
    protected int currentHP, currentAtk;
    // Start is called before the first frame update

    public virtual void StatsChange(int atk = 0, int health = 0)
    {
        currentAtk += atk;
        if (currentAtk < 0) currentAtk = 0;
        currentHP += health;
        updText();
    }

    protected void updText()
    {
        hp.text = currentHP.ToString();
        if(damage!= null) damage.text = currentAtk.ToString();
    }
}
