using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenes : MonoBehaviour {

    public void EnterCollection()
    {
        SceneManager.LoadScene("Collections");
    }

    public void EnterWar()
    {
        SceneManager.LoadScene("PlayerMatching");
    }

    public void EnterRecruitment()
    {
        SceneManager.LoadScene("Recruitment");
    }
}
