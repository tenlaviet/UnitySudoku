using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberButton : MonoBehaviour
{
    public int value;
    private TextMeshProUGUI _textComponent;
    private Button _button;

    private void Awake()
    {
        _button = this.GetComponentInChildren<Button>();
        _textComponent = this.GetComponentInChildren<TextMeshProUGUI>();

        _textComponent.text = value.ToString();
        _button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        Debug.Log(value);
        //SudokuBoard.Instance.PlaceNumber(value);
    }
}
