using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsBox : Menu
{
    public static OptionsBox current = null;

    public OptionButton optionButtonPrefab;

    [SerializeField] private RectTransform box;

    private DropDownButton calledDropDown = null;
    private Vector2 position = new Vector2(0f, 0f);
    private Vector2 size = new Vector2(256f, 256f);

    protected override void Update()
    {
        if (!IsCurrentActive()) return;

        base.Update();
        
        if (STBInput.GetButtonDown("Cancel"))
        {
            SoundManager.instance.PlayUiEfx(UiEfx.CANCEL);
            OnCancelled();
        }
    }

    // Methods for setting, opening and closing

    public static void Open(DropDownButton dropDown, float width = 256f)
    {
        if (current == null)
        {
            Debug.Log("No available options box.");
            return;
        }

        MenuController.currentMenu = current;
        current.Set(dropDown, width);
        current.FadeIn(0.3f);
    }

    void Set(DropDownButton dropDown, float width)
    {
        calledDropDown = dropDown;
        int count = dropDown.options.Length;
        buttons = new OptionButton[count];

        SetSize(width, count);
        SetPosition();

        for (int i = 0; i < count; i++)
        {
            OptionButton button = Instantiate(optionButtonPrefab, transform);
            button.ControlName = dropDown.options[i];
            button.value = i;
            button.SetWidth(width);
            button.SetPosition(position.x, position.y + size.y / 2f - 60f * i - 42f);
            button.onClick.AddListener(delegate () { OnSelected(button.value); });
            buttons[i] = button;
            if (button.value == calledDropDown.value) defaultButton = button;
        }
        
        for (int i = 0; i < count; i++)
        {
            buttons[i].navigation = new Navigation()
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = i == 0 ? null : buttons[i - 1],
                selectOnDown = i == count - 1 ? null : buttons[i + 1]
            };
        }
    }

    void SetSize(float width, int optionCount)
    {
        size.x = width;
        size.y = 60f * optionCount + 24f;
        box.sizeDelta = size;
    }

    void SetPosition()
    {
        RectTransform rectTransform = calledDropDown.GetComponent<RectTransform>();
        position = rectTransform.anchoredPosition;
        position.x += calledDropDown.pressWidth;
        position.y += rectTransform.sizeDelta.y / 2 - size.y / 2f;
        box.anchoredPosition = position;
    }

    protected override void OnClosed()
    {
        foreach (BaseButton button in buttons)
        {
            Destroy(button.gameObject);
        }
        base.OnClosed();
    }

    //Methods for events

    public void OnSelected(int value)
    {
        MenuController.currentMenu = calledDropDown.ParentMenu;
        EventSystem.current.SetSelectedGameObject(null);
        calledDropDown.SetValue(value);
        calledDropDown.SetPressed(false);
        FadeOut(0.2f);
    }

    public void OnCancelled()
    {
        MenuController.currentMenu = calledDropDown.ParentMenu;
        EventSystem.current.SetSelectedGameObject(null);
        calledDropDown.SetPressed(false);
        FadeOut(0.2f);
    }
}