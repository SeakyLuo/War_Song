using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class CollectionManager : MonoBehaviour {

    public int cardsPerPage = 8;
    public Dictionary<string, int> pageLimits = new Dictionary<string, int>();
    public KeyValuePair<string, int> currentPage = new KeyValuePair<string, int>("General", 1), notFound = new KeyValuePair<string, int>("", 0);
    public Vector3 raise = new Vector3(0, 0, 10);
    public GameObject left, right, clearSearch, selectedBoardPanel, createLineupPanel;
    public Text TitleText, pageText;
    public InputField searchByInput;
    public Dropdown searchByGold, searchByOre, searchByHealth;

    private static string[] types = Collection.types;
    private List<Collection> displayCollections, searchedCollections;
    private Dictionary<string, List<Collection>> collectionDict = new Dictionary<string, List<Collection>>();
    private Dictionary<string, List<Collection>> originalDict = new Dictionary<string, List<Collection>>();
    private int pageNumber = 1, 
        searchByGoldValue = -1, 
        searchByOreValue = -1, 
        searchByHealthValue = -1;
    private string searchByKeyword = "";
    private static GameObject[] tabs = new GameObject[types.Length];
    private GameObject[] cards;
    private Text[] counters;
    private Vector3 mousePosition;

    // Use this for initialization
    void Start () {
        cards = new GameObject[cardsPerPage];
        counters = new Text[cardsPerPage];
        foreach (string type in types)
        {
            pageLimits.Add(type, 1);
            collectionDict.Add(type, new List<Collection>());
            originalDict.Add(type, new List<Collection>());
        }
        for (int i = 0; i < cardsPerPage; i++)
        {
            Transform slot = GameObject.Find("Slot" + i.ToString()).transform;
            cards[i] = slot.Find("Card").gameObject;
            counters[i] = slot.Find("Count/CountText").GetComponent<Text>();
        }
        for (int i = 0; i < types.Length; i++)
        {
            tabs[i] = GameObject.Find("Tabs/" + types[i]);
            tabs[i].SetActive(true);
        }
        LoadUserCollections();
        foreach (Collection collection in displayCollections)
            originalDict[collection.type].Add(collection);
        SetPageLimits();
        ShowCurrentPage();
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
            if(!collection.name.StartsWith("Standard ")) InfoLoader.user.collections.Add(collection);
            collectionDict[collection.type].Add(collection);
            SetPageLimits();
        }
        ShowCurrentPage();
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
            InfoLoader.user.collections.Remove(found);
            displayCollections.Remove(found);
            collectionDict[found.type].Remove(found);
            SetPageLimits();
        }
        ShowCurrentPage();
        return true;
    }

    public void RemoveStandardCards()
    {
        LoadUserCollections();
        SetPageLimits();
    }

    private void LoadUserCollections()
    {
        displayCollections = InfoLoader.user.collections;
        LoadCollections();
    }

    private void LoadCollections()
    {
        foreach (KeyValuePair<string, List<Collection>> pair in collectionDict)
            pair.Value.Clear();
        // Sort?
        foreach (Collection collection in displayCollections)
            collectionDict[collection.type].Add(collection);
    }

    private void SetPageLimits(string type = "")
    {
        int count;
        foreach (KeyValuePair<string, List<Collection>> item in collectionDict)
        {
            count = item.Value.Count;
            if (count > 0)
            {
                if (count % cardsPerPage == 0) count = (int)Mathf.Floor(count / cardsPerPage);
                else count = (int)Mathf.Floor(count / cardsPerPage) + 1;
            }
            pageLimits[item.Key] = count;            
        }
    }

    public void SetCurrentPage(string type,int page)
    {
        if (page <= pageLimits[type])
            currentPage = new KeyValuePair<string, int>(type, page);
    }

    public void SetCardsPerPage(int number)
    {
        cardsPerPage = number;
        SetPageLimits();
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
        for (int i = types.Length - 1; i >= 0; i--)
            if (pageLimits[types[i]] != 0)
                return new KeyValuePair<string, int>(types[i], pageLimits[types[i]]);
        return notFound;
    }

    public void ShowCurrentPage()
    {
        // Disable things
        if (currentPage.Equals(FirstPage())) left.SetActive(false);
        else left.SetActive(true);
        if (currentPage.Equals(LastPage())) right.SetActive(false);
        else right.SetActive(true);
        for (int i = 0; i < cardsPerPage; i++)
        {
            cards[i].GetComponent<CardInfo>().Clear();
            cards[i].SetActive(false);
            counters[i].text = "";
            counters[i].transform.parent.gameObject.SetActive(false);
        }
        pageText.text = "";

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
        tabs[Array.IndexOf(types, type)].GetComponent<Button>().Select(); // Doesn't always work
        SortCollections();        

        GameObject card;
        Collection collection;
        List<Collection> collectionWithType = collectionDict[type];        
        for (int i = 0; i < Mathf.Min(cardsPerPage, collectionWithType.Count - cardsPerPage * page); i++)
        {
            card = cards[i];
            collection = collectionWithType[page * cardsPerPage + i];
            card.GetComponent<CardInfo>().SetAttributes(collection);
            card.SetActive(true);
            if (collection.count == 1)
            {
                counters[i].text = "";
                counters[i].transform.parent.gameObject.SetActive(false);
            }
            else if (collection.count > 99)
            {
                counters[i].text = "×99+";
                counters[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                counters[i].text = "×" + collection.count.ToString();
                counters[i].transform.parent.gameObject.SetActive(true);
            }
        }
    }

    public void PreviousPage()
    {
        // Turn page animatioin    
        string type = currentPage.Key;
        int page = currentPage.Value;
        if (currentPage.Value == 1)
        {
            int index = Array.IndexOf(CollectionManager.types, type) - 1;
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
        ShowCurrentPage();
    }

    public void NextPage()
    {
        // Turn page animatioin      
        string type = currentPage.Key;
        int page = currentPage.Value;
        if (currentPage.Value == pageLimits[type])
        {
            int index = Array.IndexOf(types, type) + 1;
            while (true)
            {
                type = types[index];
                page = 1;
                if (pageLimits[type] != 0 || index == types.Length - 1) break;
                index++;
            }
        }
        else page++;
        SetCurrentPage(type, page);
        ShowCurrentPage();
    }

    public void ClickTab(string cardType)
    {
        //tabs[Array.IndexOf(types, cardType)].GetComponent<Button>().Select();
        SetCurrentPage(cardType, 1);
        ShowCurrentPage();
    }

    public void ClickTab(GameObject obj)
    {
        //obj.GetComponent<Button>().Select();
        SetCurrentPage(obj.name, 1);
        ShowCurrentPage();
    }

    private void SortCollections()
    {

    }


    private void ShowSearchedCollection()
    {
        displayCollections = searchedCollections;
        LoadCollections();
        SetPageLimits();
        for (int i = types.Length - 1; i >= 0; i--)
        {
            // only show tab with result
            if (collectionDict[types[i]].Count == 0) tabs[i].SetActive(false);
            else tabs[i].SetActive(true);
        }
        currentPage = FirstPage();
        ShowCurrentPage();
    }

    public void Search(string word = "", int gold = -1, int ore = -1, int health = -1)
    {
        searchedCollections = InfoLoader.user.collections;
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
                    (collection.type == "Tactic" && Resources.Load<TacticAttributes>("Tactics/Info/" + collection.name + "/Attributes").description.Contains(word)) ||
                    (collection.type != "Tactic" && Resources.Load<PieceAttributes>("Pieces/Info/" + collection.name + "/Attributes").description.Contains(word)))
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
                    int goldCost = Resources.Load<TacticAttributes>("Tactics/Info/" + collection.name + "/Attributes").goldCost;
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
                if (collection.type == "Tactic") oreCost = Resources.Load<TacticAttributes>("Tactics/Info/" + collection.name + "/Attributes").oreCost;
                else oreCost = Resources.Load<PieceAttributes>("Pieces/Info/" + collection.name + "/Attributes").oreCost;
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
                    // IDK whether ∞ is 5+ or not
                    int Health = Resources.Load<PieceAttributes>("Pieces/Info/" + collection.name + "/Attributes").health;
                    if ((health == 0 && Health == health) ||
                        (health == 5 && Health >= health) ||
                        (health < 5 && Health == health))
                        newSearched.Add(collection);
                }
            }
            searchedCollections = newSearched;
            newSearched = new List<Collection>();
        }
        ShowSearchedCollection();
    }

    public bool InCollection(string name)
    {
        foreach (Collection collection in InfoLoader.user.collections)
            if (collection.name == name)
                return true;
        return false;
    }

    public void InputFieldSearch()
    {
        searchByKeyword = searchByInput.text.Trim();
        if (searchByKeyword != "")
        {
            clearSearch.SetActive(true);
            Search(searchByKeyword);
        }
    }

    public void ClearSearch()
    {
        searchByInput.text = "";
        clearSearch.SetActive(false);
        Search(searchByKeyword, searchByGoldValue, searchByOreValue, searchByHealthValue);
    }

    public void SetSearchByGold()
    {
        if (searchByGold.value == 0) searchByGoldValue = -1;
        else searchByGoldValue = searchByGold.value - 1;
        Search(searchByKeyword, searchByGoldValue, searchByOreValue, searchByHealthValue);
    }

    public void SetSearchByOre()
    {
        if (searchByOre.value == 0) searchByOreValue = -1;
        else searchByOreValue = searchByOre.value - 1;
        Search(searchByKeyword, searchByGoldValue, searchByOreValue, searchByHealthValue);
    }

    public void SetSearchByHealth()
    {
        if (searchByHealth.value == 0) searchByHealthValue = -1;
        else if (searchByHealth.value == 6) searchByHealthValue = 0;
        else searchByHealthValue = searchByHealth.value;
        Search(searchByKeyword, searchByGoldValue, searchByOreValue, searchByHealthValue);
    }


}
