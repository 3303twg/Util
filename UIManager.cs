using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
public class UIManager : MonoBehaviour
{
    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();

    [SerializeField]
    public GameObject[] uiObjects;
    GameObject[] objects;




    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetUI();
    }

    private void GetUI()
    {
        GameObject parent = GameObject.Find("Canvas");
        if (parent != null)
        {
            objects = new GameObject[parent.transform.childCount];

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                objects[i] = parent.transform.GetChild(i).gameObject;
            }

        }

        uiObjects = objects
            .Where(obj => obj != null && obj.CompareTag("UI"))
            .ToArray();


        foreach (var uiObject in uiObjects)
        {
            uiElements[uiObject.name] = uiObject;
            //UI Object Init
            //uiObject.GetComponent<BaseUI>().Init_Active();
        }
    }
    
    public void ShowUI(string uiName)
    {
        if (uiElements.ContainsKey(uiName))
        {
            uiElements[uiName].SetActive(true);
        }
    }

    public void HideUI(string uiName)
    {
        if (uiElements.ContainsKey(uiName))
        {
            uiElements[uiName].SetActive(false);
        }
    }

    public void ToggleUI(string uiName)
    {
        if (uiElements.ContainsKey(uiName))
        {
            uiElements[uiName].SetActive(!uiElements[uiName].activeSelf);
        }
    }
}
