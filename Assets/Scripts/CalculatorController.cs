using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalculatorController : MonoBehaviour
{

    public TMP_InputField ExpressionText;
    public TMP_InputField resultText;


    private string expression = "";

    void Start()
    {
        UpdateUI();
    }

    public void AddNumber(string n)
    {
        expression += n;
        UpdateUI();
    }

    public void AddOperator(string op)
    {
        if (string.IsNullOrEmpty(expression)) return;

        char last = expression[expression.Length - 1];
        if ("+-*/".Contains(last.ToString()))
        {
            // replace last operator instead of stacking
            expression = expression.Substring(0, expression.Length - 1) + op;
        }
        else
        {
            expression += op;
        }

        UpdateUI();
    }

    public void Calculate()
    {
        var (ok, res) = CalculatorEvaluation.Calculate(expression);
        resultText.text = res;
    }

    public void Clear()
    {
        expression = "";
        UpdateUI();
    }

    public void ResetAll()
    {
        expression = "";
        resultText.text = "";
        UpdateUI();
    }

    private void UpdateUI()
    {
        ExpressionText.text = expression;
    }

}
