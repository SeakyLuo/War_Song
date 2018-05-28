using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionInfo : MonoBehaviour {

    public Text missionName;
    public Text description;
    public Text reward;
    public Text progress;

    public void SetAttributes(Mission mission)
    {
        MissionAttributes missionAttributes = Database.FindMissionAttributes(mission.Name);
        missionName.text = mission.Name;
        description.text = missionAttributes.description;
        reward.text = missionAttributes.reward.ToString();
        progress.text = string.Format("{0}/{1}", mission.progress, missionAttributes.requirement);
    }
}
