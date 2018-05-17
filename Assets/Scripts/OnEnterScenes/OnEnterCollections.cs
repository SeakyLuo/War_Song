using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class OnEnterCollections : MonoBehaviour
{
    public GameObject selectBoardPanel, createLineupPanel, settingsPanel;
    public GameObject oreDropdown, healthDropdown, goldDropdown;

    private CollectionManager collectionManager;
    private GameObject[] closeObjects;

    private void Start()
    {
        collectionManager = transform.Find("Collection").GetComponent<CollectionManager>();
        closeObjects = new GameObject[] { oreDropdown, healthDropdown, goldDropdown };
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            settingsPanel.SetActive(true);
        else if (Input.GetMouseButtonUp(0))
        {
            foreach (GameObject close in closeObjects)
            {
                if (close.activeSelf)
                {
                    Vector2 mousePosition = Input.mousePosition;
                    Rect rect = close.transform.parent.GetComponent<RectTransform>().rect;
                    // Might have bug
                    float leftBound = close.transform.parent.position.x;
                    float upperBound = close.transform.parent.position.y;
                    if (mousePosition.x < leftBound + rect.x || mousePosition.x > leftBound - rect.x || mousePosition.y < upperBound + rect.y || mousePosition.y > upperBound - rect.y)
                        close.SetActive(false);
                    break;
                }
            }
        }
    }

    public void Back()
    {
        if (createLineupPanel.activeSelf)
        {
            collectionManager.SetCardsPerPage(8);
            Destroy(createLineupPanel.transform.Find("BoardPanel/Board/LineupBoard(Clone)").gameObject);
            createLineupPanel.SetActive(false);
            if (LineupsManager.modifyLineup == -1)
                selectBoardPanel.SetActive(true);
        }
        else if (selectBoardPanel.activeSelf)
            selectBoardPanel.SetActive(false);
        else
            SceneManager.LoadScene(InfoLoader.switchSceneCaller);
    }

    public void Recruit()
    {
        SceneManager.LoadScene("Recruitment");
    }
}
