using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SwitchScenes : MonoBehaviour, IPointerClickHandler
{
    public GameObject playerInfoPanel, settingsPanel, optionsPanel, challengeToday;
    public Text winText, loseText, drawText, percentageText;
    public Text rank, title;

    private Canvas parentCanvas;

    private void Start()
    {
        parentCanvas = GetComponent<Canvas>();
        SetPlayerInfo();
        if (InfoLoader.user.challenges.Count != 0)
        {
            challengeToday.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            settingsPanel.SetActive(true);
    }

    public void EnterCollection()
    {
        SceneManager.LoadScene("Collection");
    }

    public void EnterWar()
    {
        SceneManager.LoadScene("PlayerMatching");
    }

    public void EnterRecruitment()
    {
        SceneManager.LoadScene("Recruitment");
    }

    private void SetPlayerInfo()
    {
        winText.text = InfoLoader.user.total.win.ToString();
        loseText.text = InfoLoader.user.total.lose.ToString();
        drawText.text = InfoLoader.user.total.draw.ToString();
        percentageText.text = InfoLoader.user.total.percentage.ToString();
        rank.text = InfoLoader.user.rank.ToString();
        title.text = Range.FindTitle(InfoLoader.user.rank);
    }

    public void ShowPlayerInfo()
    {
        SetPlayerInfo();
        playerInfoPanel.SetActive(true);
    }

    public void ChangeChallenge()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (challengeToday.activeSelf) challengeToday.SetActive(false);
        else if (playerInfoPanel.activeSelf)
        {
            Vector2 mousePosition = AdjustedMousePosition();
            Rect rect = playerInfoPanel.GetComponent<RectTransform>().rect;
            // rect.x and rect.y are negative
            if (mousePosition.x < rect.x || mousePosition.x > -rect.x || mousePosition.y < rect.y || mousePosition.y > -rect.y)
                playerInfoPanel.SetActive(false);
        }
    }

    private Vector2 AdjustedMousePosition()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Input.mousePosition, parentCanvas.worldCamera, out mousePosition);
        return mousePosition;
    }
}
