using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class CodePuzzle : PuzzleBehaviour
{
    string code;
    string input="";
    TMP_Text codeText;
    Button[] buttons;
  
    public override void Init(PuzzleData _newPuzzleData, PuzzleManager _manager)
    {
        base.Init(_newPuzzleData, _manager);

        CodePuzzleData codeData = (CodePuzzleData)_newPuzzleData;

        code = codeData.code;
        input = "";
        _manager.codePuzzleHolder.SetActive(true);

        codeText = _manager.codePuzzleHolder.transform.Find("CodeText").GetComponent<TMP_Text>();
        buttons = _manager.codePuzzleHolder.GetComponentsInChildren<Button>();
        codeText.text = input;
        foreach (Button button in buttons)
        {
            if (button.name == "EnterButton")
            { 
                button.onClick.AddListener(CheckCode);
                continue;
            }
            if (button.name == "EraseButton")
            {
                button.onClick.AddListener(EraseInput);
                continue;
            }

            button.onClick.AddListener(() => Input(button.name));
        }
    }
    public override void Input(string _input)
    {
        if (input.Length >= code.Length)
            return;

        base.Input(_input);
        input += _input;
        codeText.text = input;

    }
    public void CheckCode()
    {
        if (input == code)
            Debug.Log("Codigo correcto");
        else
        {
            input = "";
            codeText.text = input;
            Debug.Log("Codigo incorrecto");
        }
    }
    public void EraseInput()
    {
        input = "";
        codeText.text = input;
    }
}
