using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnEnterCollections : MonoBehaviour {

    public GameObject selectBoardPanel, createLineupPanel, settingsPanel;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            settingsPanel.SetActive(true);
    }

    public void Back()
    {
        if (createLineupPanel.activeSelf)
        {
            Destroy(createLineupPanel.transform.Find("BoardPanel/Board/LineupBoard(Clone)").gameObject);
            createLineupPanel.SetActive(false);
            selectBoardPanel.SetActive(true); // User should not click back when open a lineup
        }
        else if (selectBoardPanel.activeSelf)
            selectBoardPanel.SetActive(false);
        else
            SceneManager.LoadScene(InfoLoader.switchSceneCaller);
    }
}
