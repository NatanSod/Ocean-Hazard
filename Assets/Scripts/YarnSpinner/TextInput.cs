using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextInput : MonoBehaviour
{
    public GameObject textInputObject;
    private TMP_InputField textInput;
    private CanvasGroup canvasGroup;
    public bool isSelected = false;
    public void Select() {
        isSelected = true;
    }
    public void DeSelect() {
        isSelected = false;
    }
    public bool waiting = false;
    [HideInInspector] public string text = "";
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        textInput = textInputObject.GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (waiting && Input.GetKeyDown(KeyCode.Return)) {
            StopWaiting();
        }
    }

    public void AwaitInput() {
        waiting = true;
        textInput.ActivateInputField();
        canvasGroup.alpha = 1;
    }

    private void StopWaiting() {
        canvasGroup.alpha = 0;
        text = textInput.text;
        waiting = false;
    }
}
