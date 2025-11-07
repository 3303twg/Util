using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    private readonly Dictionary<Type, GameObject> UIDic = new();
    private Stack<Type> uiStack = new();

    [SerializeField] private Transform parentObj; // 반드시 인스펙터에서 지정

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += (_, _) => uiStack.Clear();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= (_, _) => uiStack.Clear();
    }

    public void OpenUI<T>() where T : Component => OpenUI(typeof(T));
    public void CloseUI<T>() where T : Component => CloseUI(typeof(T));
    public void SwitchUI<TFrom, TTo>() where TFrom : Component where TTo : Component
        => SwitchUI(typeof(TFrom), typeof(TTo));

    public void OpenUI(Type type)
    {
        GameObject ui = GetUI(type);
        if (ui == null || ui.activeSelf) return;

        ui.SetActive(true);
        uiStack.Push(type);

        // 정렬 순서 자동 증가
        if (ui.TryGetComponent(out Canvas canvas))
            canvas.sortingOrder = uiStack.Count;
    }

    public void CloseUI(Type type)
    {
        GameObject ui = GetUI(type);
        if (ui == null) return;


        uiStack = new Stack<Type>(uiStack.Where(t => t != type).Reverse());

        ui.SetActive(false);
    }

    public void SwitchUI(Type from, Type to)
    {
        CloseUI(from, isSwitching: true);
        OpenUI(to);
    }

    private GameObject GetUI(Type type)
    {
        // parentObj는 반드시 인스펙터에서 직접 지정
        if (parentObj == null)
        {
            Debug.LogError($"UIManager의 parentObj가 지정되지 않았습니다! {type.Name} UI를 생성할 수 없습니다.");
            return null;
        }

        if (UIDic.TryGetValue(type, out var ui))
            return ui;

        GameObject prefab = Resources.Load<GameObject>($"UI/{type.Name}");
        if (prefab == null)
        {
            Debug.LogError($"UI Prefab이 Resources/UI/{type.Name} 경로에 없습니다.");
            return null;
        }

        ui = Instantiate(prefab, parentObj);
        ui.SetActive(false);
        UIDic[type] = ui;
        return ui;
    }
}
