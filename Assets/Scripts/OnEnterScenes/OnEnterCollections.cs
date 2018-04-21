using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnEnterCollections : MonoBehaviour {

    public GameObject selectBoardPanel, createLineupPanel;

    public void Back()
    {
        if (createLineupPanel.activeSelf)
        {
            Destroy(createLineupPanel.transform.Find("BoardPanel/Board/LineupBoard(Clone)").gameObject);
            createLineupPanel.SetActive(false);
            selectBoardPanel.SetActive(true); // User should not click back when open a lineup
        }
        else SceneManager.LoadScene(InfoLoader.switchSceneCaller);
    }
}
