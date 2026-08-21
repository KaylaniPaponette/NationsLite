using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UIDropdown : UIInteractable
{
    const string kDropdownClassName = "ui-dropdown";
    const string kDropdownButtonClassName = "ui-dropdown__button";
    const string kDropdownOptionsContainerClassName = "ui-dropdown__options-container";
    const string kDropdownOptionClassName = "ui-dropdown__option";


    public Action<string> OnSelected;
    List<string> _options = new List<string>();
    UIContainer optionsContainer;
    UIButton dropdownButton;
    UIButton selectedOption;
    bool isOpened = false;

    [UxmlAttribute]
    public List<string> options
    {
        get => _options ?? new List<string>();
        set
        {
            _options = value;
            OnOptionsChanged();
        }
    }

    public void OnOptionsChanged()
    {
        RebuildOptions();
    }

    void RebuildOptions()
    {
        optionsContainer.Clear();
        optionsContainer.Add(selectedOption);

        if (_options == null) return;

        foreach (string optionText in _options)
        {
            UILabel optionLabel = new UILabel(optionText);
            var optionButton = new UIButton(optionLabel);
            optionButton.AddToClassList(kDropdownOptionClassName);
            optionButton.clicked += () => OnOptionSelected(optionText);
            optionsContainer.Add(optionButton);
        }
    }

    public UIDropdown()
    {
        this.name = kDropdownClassName;
        this.AddToClassList(kDropdownClassName);

        dropdownButton = new UIButton();
        dropdownButton.name = "DropdownButton";
        dropdownButton.AddToClassList(kDropdownButtonClassName);
        dropdownButton.clicked += OnDropdownClicked;
        Add(dropdownButton);

        selectedOption = new UIButton("None");
        selectedOption.AddToClassList(kDropdownOptionClassName);
        selectedOption.clicked += () => OnOptionSelected(selectedOption.text);

        optionsContainer = new UIContainer();
        optionsContainer.name = "OptionsContainer";
        optionsContainer.AddToClassList(kDropdownOptionsContainerClassName);
        optionsContainer.Add(selectedOption);
        Add(optionsContainer);

        RebuildOptions();
    }

    public void OnDropdownClicked()
    {
        isOpened = !isOpened;
        optionsContainer.style.display = isOpened ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void OnOptionSelected(string optionText)
    {
        dropdownButton.text = optionText;
        isOpened = false;
        optionsContainer.style.display = DisplayStyle.None;
        OnSelected?.Invoke(optionText);
    }
}
