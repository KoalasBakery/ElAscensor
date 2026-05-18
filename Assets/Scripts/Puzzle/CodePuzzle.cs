using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class CodePuzzle : PuzzleBehaviour
{
    string code;
    string input="";
    TMP_Text codeText;
    Button[] buttons;
  
    public override void Init(PuzzleData _newPuzzleData)
    {
        base.Init(_newPuzzleData);

        CodePuzzleData codeData = (CodePuzzleData)_newPuzzleData;

        code = codeData.code;
        input = "";
        PuzzleManager.Instance.codePuzzleHolder.SetActive(true);

        codeText = PuzzleManager.Instance.codePuzzleHolder.transform.Find("CodeText").GetComponent<TMP_Text>();
        buttons = PuzzleManager.Instance.codePuzzleHolder.GetComponentsInChildren<Button>();
        codeText.text = input;
        Debug.Log(buttons.Length);
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
