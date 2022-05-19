using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnCommands : MonoBehaviour
{
    public CanvasGroup dialogueGroup;
    public TextInput textInput;
    public DialogueRunner dialogueRunner;
    public CustomLine lineView;
    static private YarnCommands yarnCommands;
    void Start () {
        yarnCommands = this;
        foreach(DialogueViewBase view in dialogueRunner.dialogueViews) {
            if (view.GetType() == typeof(CustomLine)) {
                lineView = view as CustomLine;
            }
        }
    }

    void Update() {
        if (!yarnCommands.textInput.isSelected && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))) {
            yarnCommands.lineView.UserRequestedViewAdvancement();
        }
    }

    static Dictionary<string, string> strings = new Dictionary<string, string>();
    [YarnCommand("await_input")]
    static IEnumerator WaitForInput(string type, string name)
    {
        if (type == "text") {
            yarnCommands.textInput.AwaitInput();
            while (yarnCommands.textInput.waiting) { yield return null; }
            string text = yarnCommands.textInput.text;
            if (strings.ContainsKey(name)) {
                strings[name] = text;
            }
            else {
                strings.Add(name, text);
            }
        }
    }

    [YarnFunction("get_text")]
    public static string GetTextInput(string name)
    {
        return strings[name];
    }

    [YarnCommand("change_text")]
    public static void ChangeText(string name, string newText) {
        strings[name] = newText;
    }

    static bool fadeNot;
    [YarnCommand("do_not_fade")]
    static private void DoNotFade() {
        fadeNot = true;
        yarnCommands.StartCoroutine(yarnCommands.FadeNot());
    }
    public IEnumerator FadeNot() {
        while (fadeNot) {
            yield return new WaitForEndOfFrame();
            dialogueGroup.alpha = 1;
        }
        dialogueGroup.alpha = 0;
    }
    [YarnCommand("allow_fade")]
    static public void AllowFade() {
        yarnCommands.StartCoroutine(Fade());
        fadeNot = false;
    }
    static  IEnumerator Fade () {
        yarnCommands.dialogueGroup.alpha = 0;
        yield return null;
        yarnCommands.dialogueGroup.alpha = 0;
    }
    [YarnFunction("is_blank")]
    static bool IsOnlySpace(string Text) {
        foreach(char c in Text)
        {
            if(c != ' '){
                return false;
            }
        }
        return true;
    }
    [YarnFunction("has_number")]
    static public bool HasNumber(string Text) {
        string text;
        if (Text[0] == '$') {
            text = strings[Text];
        } else {
            text = Text;
        }
        for (int i = 0; i < 10; i ++) {
            if (text.Contains($"{i}")) {
                return true;
            }
        }
        return false;
    }

    [YarnFunction("capitalize_first")]
    static public string CapitaliseFirst(string Text) {
        string text;
        if (Text[0] == '$') {
            text = strings[Text];
        } else {
            text = Text;
        }
        string capitalized = text.Substring(0, 1).ToUpper() + text.Substring(1);
        return capitalized;
    }
}
