using UnityEngine;
using TMPro;
public class CodePuzzle : PuzzleBehaviour
{
    string code = "1234";
    string input="";
    [SerializeField] TMP_Text codeText;
    
    public override void Init()
    {
        base.Init();
        input = "";
        Debug.Log("CodePuzzle Init");
    }
    public override void Input(string _input)
    {
        if (input.Length >= 4)
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
