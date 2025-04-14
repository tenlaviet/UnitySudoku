using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class CellData
{
    public int i;
    public int j;
    public int value;
    public SudokuCellBackgroundState backgroundState;
    public SudokuCellTextState textState;
    public int[] noteValues;

    public CellData()
    {
        noteValues = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }
    public void ReplaceCellData(CellData data)
    {

        this.i = data.i;
        this.j = data.j;
        this.value = data.value;
        this.backgroundState = data.backgroundState;
        this.textState = data.textState;
        for (int k = 0; k < 9; k++)
        {
            this.noteValues[k] = data.noteValues[k];
        }
    }
}
public class SudokuCell : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public Button button;
    public bool isValueValid;
    public bool isEditable;
    public bool isSelected;
    public CellData data;
    
    public List<TextMeshProUGUI> noteTextList;

    public void Initialize(int nRow, int nCol, Transform parentTransform)
    {
        string name = nRow + "x" + nCol;
        this.gameObject.name = name;
        this.transform.SetParent(parentTransform, false);//set parent for Cell
        this.transform.localScale = new Vector3(1, 1, 1);
        this.SetPosition(nRow,nCol);
    }


    
    
    public void SetPosition(int i, int j)
    {
        this.data.i = i;
        this.data.j = j;
    }

    public int GetValue()
    {
        return data.value;
    }
    public void SetCellValue(int value)
    {
        if (SudokuBoard.Instance.isPencilActive)
        {
            if (this.data.value !=0)
            {
                this.data.value = 0;
            }
            this.data.noteValues[value - 1] = value;
        }
        else
        {
            for(int i = 0; i < this.data.noteValues.Length; i++)
            {
                this.data.noteValues[i] = 0;
            }
            this.data.value = value;
        }
        UpdateCell();
    }
    
    public void SetCellState(SudokuCellBackgroundState backgroundState, SudokuCellTextState textState)
    {

        this.data.textState = textState;
        this.data.backgroundState = backgroundState;
        UpdateCell();
    }

    public void SetCellDefault()
    {
        this.isValueValid = true;
        this.isEditable = false;
        this.isSelected = false;
        this.data.backgroundState = 0;
        this.data.textState = 0;
        this.data.value = 0;
        for(int i = 0; i < 9; i++)
        {
            this.data.noteValues[i] = 0;
        }
        UpdateCell();

    }

    public void EraseCellValue()
    {
        this.isValueValid = true;
        this.data.textState = 0;
        this.data.value = 0;
        if (this.data.noteValues.Length > 0)
        {
            for(int i = 0; i < 9; i++)
            {
                this.data.noteValues[i] = 0;
            }
        }
        UpdateCell();
    }

    public void UpdateCell()
    {
        this.valueText.color = Data.cellTextFontColor[this.data.textState];
        this.button.image.color = Data.cellBackgroundColor[this.data.backgroundState];//background change
        if (this.data.value == 0)
        {
            valueText.text = "";
        }
        else
        {
            valueText.text = this.data.value.ToString();
        }

        if (this.data.noteValues.Length > 0)
        {
            for(int i = 0; i < 9; i++)
            {
                if (this.data.noteValues[i] == 0)
                {
                    this.noteTextList[i].text = "";
                }
                else
                {
                    this.noteTextList[i].text = this.data.noteValues[i].ToString();
                }
            }
        }

    }

    public void OnClick()
    {
        SudokuBoard.Instance.SelectCell(this);
    }
    
    
    
}
