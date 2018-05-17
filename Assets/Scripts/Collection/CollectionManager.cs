using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour {

    public int cardsPerPage = 8;
    public Dictionary<string, int> pageLimits = new Dictionary<string, int>();
    public KeyValuePair<string, int> currentPage, notFound = new KeyValuePair<string, int>("", 0);
    public Vector3 raise = new Vector3(0, 0, 10);
    public GameObject clearSearch, searchPanel, selectedBoardPanel, createLineupPanel, noCollectionPanel;
    public Transform tabsObj;
    public Button createLineupButton;
    public Text TitleText, pageText, createLineupButtonText;
    public InputField searchByInput;

    private static GameObject[] tabs;

    private List<string> types;
    private List<Collection> displayCollections, searchedCollections;
    private Dictionary<string, List<Collection>> collectionDict = new Dictionary<string, List<Collection>>();
    private Dictionary<string, List<Collection>> originalDict = new Dictionary<string, List<Collection>>();
    private int pageNumber = 1, 
                searchByGoldValue = -1, 
                searchByOreValue = -1, 
                searchByHealthValue = -1;
    private string searchByKeyword = "";
    private GameObject[] cards;
    private Text[] counters;

    // Use this for initialization
    void Start () {
        types = Database.types;
        tabs = new GameObject[types.Count];
        cards = new GameObject[cardsPerPage];
        counters = new Text[cardsPerPage];
        foreach (string type in types)
        {
            pageLimits.Add(type, 0);
            collectionDict.Add(type, new List<Collection>());
            originalDict.Add(type, new List<Collection>());
        }
        for (int i = 0; i < cardsPerPage; i++)
        {
            Transform slot = GameObject.Find("Slot" + i.ToString()).transform;
            cards[i] = slot.Find("Card").gameObject;
            counters[i] = slot.Find("Count/CountText").GetComponent<Text>();
        }
        for (int i = 0; i < types.Count; i++)
            tabs[i] = tabsObj.Find(types[i]).gameObject;

        LoadUserCollections();
        SetPageLimits();
        ShowNoCollection(InfoLoader.user.collection.Count == 0);
        foreach (Collection collection in displayCollections)
            originalDict[collection.type].Add(collection);
        if (InfoLoader.user.collection.Count != 0) SetCurrentPage(FirstPage());
    }

    public void AddCollection(Collection collection)
    {
        bool found = false;
        foreach (Collection target in collectionDict[collection.type])
        {
            if (target.name == collection.name && target.health == collection.health)
            {
                target.count++;
                found = true;
                break;
            }
        }
        if (!found)
        {
            collection.count = 1;
            if(!collection.name.StartsWith("Standard ")) InfoLoader.user.AddCollection(collection);
            collectionDict[collection.type].Add(collection);
            SetPageLimits();
        }
        ShowNoCollection(false);
    }

    public bool RemoveCollection(Collection collection)
    {
        Collection found = new Collection();
        foreach(Collection c in collectionDict[collection.type])
            if(c.name == collection.name && c.health == collection.health)
            {                
                found = c;
                found.count--;
                break;
            }
        if (found.IsEmpty()) return false;
        if (found.count == 0)
        {
            InfoLoader.user.collection.Remove(found);
            displayCollections.Remove(found);
            collectionDict[found.type].Remove(found);
            SetPageLimits();
        }
        ShowNoCollection(InfoLoader.user.collection.Count == 0);
        return true;
    }

    public void RemoveStandardCards()
    {
        LoadUserCollections();
        SetPageLimits();
        ShowNoCollection(InfoLoader.user.collection.Count == 0);
    }

    private void LoadUserCollections()
    {
        displayCollections = InfoLoader.user.collection;
        LoadCollections();
    }
    private void LoadCollections()
    {
        foreach (string type in types)
            collectionDict[type].Clear();
        foreach (Collection collection in displayCollections)
            collectionDict[collection.type].Add(collection);
    }

    private void SetPageLimits()
    {
        int count;
        for(int i = 0; i < types.Count; i++)
        {
            count = collectionDict[types[i]].Count;
            if (count > 0)
            {
                tabs[i].SetActive(true);
                // don't try to optimize the following 2 lines idiot!
                if (count % cardsPerPage == 0) count = (int)Mathf.Floor(count / cardsPerPage);
                else count = (int)Mathf.Floor(count / cardsPerPage) + 1;
            }
            else tabs[i].SetActive(false);
            pageLimits[types[i]] = count;
        }
    }

    public void SetCurrentPage(string type, int page)
    {
        // Hightlight Tab
        if (currentPage.Key != "" || type != "")
            for (int i = 0; i < types.Count; i++)
                if (types[i] != currentPage.Key)
                {
                    ColorBlock colorBlock = tabs[i].GetComponent<Button>().colors;
                    if(currentPage.Key != null && currentPage.Key != "") // Only used when initializing
                        tabs[types.IndexOf(currentPage.Key)].GetComponent<Button>().colors = colorBlock; // Resume tab
                    if(type != "")
                    {
                        colorBlock.normalColor = Color.white;
                        tabs[types.IndexOf(type)].GetComponent<Button>().colors = colorBlock;
                    }
                    break;
                }

        currentPage = new KeyValuePair<string, int>(type, page);
        ShowCurrentPage();
    }

    private void SetCurrentPage(KeyValuePair<string, int> page)
    {
        SetCurrentPage(page.Key, page.Value);
    }

    public void SetCardsPerPage(int number)
    {
        cardsPerPage = number;
        SetPageLimits();
        if (pageLimits[currentPage.Key] != 0)
        {
            if (currentPage.Value > pageLimits[currentPage.Key])
                SetCurrentPage(currentPage.Key, pageLimits[currentPage.Key]);
        }
        else
        {
            int tab = types.IndexOf(currentPage.Key);
            int index = tab;
            while(index > 0)
            {
                if (pageLimits[types[--index]] != 0)
                {
                    SetCurrentPage(types[index], pageLimits[types[index]]);
                    return;
                }
            }
            index = tab;
            while(index < types.Count - 1)
            {
                if (pageLimits[types[++index]] != 0)
                {
                    SetCurrentPage(types[index], pageLimits[types[index]]);
                    return;
                }
            }
            ShowNoCollection(true);
        }
    }

    public KeyValuePair<string, int> FirstPage()
    {
        foreach (string type in types)
            if (pageLimits[type] != 0)
                return new KeyValuePair<string, int>(type, 1);
        return notFound;
    }

    public KeyValuePair<string, int> LastPage()
    {
        for (int i = types.Count - 1; i >= 0; i--)
            if (pageLimits[types[i]] != 0)
                return new KeyValuePair<string, int>(types[i], pageLimits[types[i]]);
        return notFound;
    }

    public void ShowNoCollection(bool noCollection)
    {
        noCollectionPanel.SetActive(noCollection);
        createLineupButton.interactable = !noCollection;
        if (noCollection) createLineupButtonText.text = "No Allies";
        else createLineupButtonText.text = "Create Lineup";
    }

    public void ShowCurrentPage()
    {
        // Disable things
        for (int i = 0; i < cardsPerPage; i++)
        {
            cards[i].GetComponent<CardInfo>().Clear();
            cards[i].SetActive(false);
            counters[i].text = "";
            counters[i].transform.parent.gameObject.SetActive(false);
        }
        pageText.text = "";

        // Calculate Page Number
        string type = currentPage.Key;
        pageNumber = currentPage.Value;
        foreach (string cardType in types)
        {
            if (cardType != type) pageNumber += pageLimits[cardType];
            else break;
        }
        pageText.text = "Page " + pageNumber.ToString();

        if (currentPage.Equals(notFound) || collectionDict[type].Count == 0)
        {
            TitleText.text = "Not Found";
            return;
        }

        int page = currentPage.Value - 1;
        TitleText.text = type;

        GameObject card;
        Collection collection;
        int previousCards = cardsPerPage * page;
        List<Collection> collectionOfType = collectionDict[type];        
        for (int i = 0; i < Mathf.Min(cardsPerPage, collectionOfType.Count - previousCards); i++)
        {
            card = cards[i];
            collection = collectionOfType[previousCards + i];
            card.GetComponent<CardInfo>().SetAttributes(collection);
            card.SetActive(true);
            if (collection.count == 1) counters[i].text = "";
            else if (collection.count > 99) counters[i].text = "×99+";
            else counters[i].text = "×" + collection.count.ToString();
            counters[i].transform.parent.gameObject.SetActive(collection.count != 1);
        }
    }

    public void PreviousPage()
    {
        // Turn page animation    
        string type = currentPage.Key;
        int page = currentPage.Value;
        if (currentPage.Value == 1)
        {
            int index = types.IndexOf(type) - 1;
            while (true)
            {
                type = types[index];
                page = pageLimits[type];
                if (page != 0 || index == 0) break;
                index--;
            }           
        }
        else page--;
        SetCurrentPage(type, page);
    }
    public void NextPage()
    {
        // Turn page animatioin      
        string type = currentPage.Key;
        int page = currentPage.Value;
        if (currentPage.Value == pageLimits[type])
        {
            int index = types.IndexOf(type) + 1;
            while (true)
            {
                type = types[index];
                page = 1;
                if (pageLimits[type] != 0 || index == types.Count - 1) break;
                index++;
            }
        }
        else page++;
        SetCurrentPage(type, page);
    }
    public void ClickTab(string cardType)
    {
        SetCurrentPage(cardType, 1);
    }
    public void ClickTab(GameObject obj)
    {
        SetCurrentPage(obj.name, 1);
    }

    private void ShowSearchedCollection()
    {
        displayCollections = searchedCollections;
        LoadCollections();
        SetPageLimits();
        for (int i = types.Count - 1; i >= 0; i--)
        {
            // only show tab with result
            if (collectionDict[types[i]].Count == 0) tabs[i].SetActive(false);
            else tabs[i].SetActive(true);
        }
        SetCurrentPage(FirstPage());
        ShowCurrentPage();
    }
    public void Search(string word = "", int gold = -1, int ore = -1, int health = -1)
    {
        searchedCollections = InfoLoader.user.collection;
        if (word == "" && gold == -1 && ore == -1 && health == -1)
        {
            word = searchByKeyword;
            gold = searchByGoldValue;
            ore = searchByOreValue;
            health = searchByHealthValue;
        }
        List<Collection> newSearched = new List<Collection>();
        if (word != "" && searchedCollections != null)
        {
            foreach (Collection collection in searchedCollections)
            {
                if (collection.name.Contains(word) ||
                    (collection.type == "Tactic" && Database.FindTacticAttributes(collection.name).description.Contains(word)) ||
                    (collection.type != "Tactic" && Database.FindPieceAttributes(collection.name).description.Contains(word)))
                    newSearched.Add(collection);
            }
            searchedCollections = newSearched;
            newSearched = new List<Collection>();
        }
        if (gold != -1 && searchedCollections != null)
        {
            foreach (Collection collection in searchedCollections)
            {
                if (collection.type == "Tactic")
                {
                    int goldCost = Database.FindTacticAttributes(collection.name).goldCost;
                    if ((gold == 5 && goldCost >= gold) ||
                        (gold < 5 && goldCost == gold))
                        newSearched.Add(collection);
                }
                // search description
            }
            searchedCollections = newSearched;
            newSearched = new List<Collection>();
        }
        if (ore != -1 && searchedCollections != null)
        {
            foreach (Collection collection in searchedCollections)
            {
                int oreCost;
                if (collection.type == "Tactic") oreCost = Database.FindTacticAttributes(collection.name).oreCost;
                else oreCost = Database.FindPieceAttributes(collection.name).oreCost;
                if ((ore == 5 && oreCost >= ore) ||
                    (ore < 5 && oreCost == ore))
                    newSearched.Add(collection);
            }
            searchedCollections = newSearched;
            newSearched = new List<Collection>();
        }
        if (health != -1 && searchedCollections != null)
        {
            foreach (Collection collection in searchedCollections)
            {
                if (collection.type != "Tactic")
                {
                    int Health = Database.FindPieceAttributes(collection.name).health;
                    if ((health == 5 && Health >= health) ||
                        (health < 5 && Health == health))
                        newSearched.Add(collection);
                }
            }
            searchedCollections = newSearched;
            newSearched = new List<Collection>();
        }
        ShowSearchedCollection();
    }
    public void ShowSearchPanel()
    {
        searchPanel.SetActive(!searchPanel.activeSelf);
    }
    public void InputFieldSearch()
    {
        searchByKeyword = searchByInput.text.Trim();
        if (searchByKeyword != "")
        {
            clearSearch.SetActive(true);
            Search(searchByKeyword);
        }
        else searchPanel.SetActive(false);
    }
    public void ClearSearch()
    {
        searchByInput.text = "";
        searchByKeyword = "";
        clearSearch.SetActive(false);
        Search(searchByKeyword, searchByGoldValue, searchByOreValue, searchByHealthValue);
    }
    public void SetSearchByGold(int value)
    {
        searchByGoldValue = value;
        Search(searchByKeyword, searchByGoldValue, searchByOreValue, searchByHealthValue);
    }
    public void SetSearchByOre(int value)
    {
        searchByOreValue = value;
        Search(searchByKeyword, searchByGoldValue, searchByOreValue, searchByHealthValue);
    }
    public void SetSearchByHealth(int value)
    {
        searchByHealthValue = value;
        Search(searchByKeyword, searchByGoldValue, searchByOreValue, searchByHealthValue);
    }

}
