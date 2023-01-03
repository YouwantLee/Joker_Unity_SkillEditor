using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill : MonoBehaviour
{

    public Skill_Player Skill_Player;
    public SkillConfig SkillConfig;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Skill_Player.PlaySkill(SkillConfig);
        }
    }
}
