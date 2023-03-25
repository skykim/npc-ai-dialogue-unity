using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PromptManager : MonoBehaviour
{
    public string apiKey;
    
    [Multiline]
    public string role;
    private string _accumulatedQuery = "";

    public TMP_InputField userInputField;
    public GameObject npcTextParent;
    public GameObject npcTextPrefab;
    public GameObject userTextPrefab;
    public ScrollRect npcScrollRect;

    public Animator npcAnimator;
    private static readonly int IsLoading = Animator.StringToHash("IsLoading");

    private void Start()
    {
        //assign specific role
        _accumulatedQuery = role + "\n\n";
    }

    public async void CompletedUserTyping()
    {
        _accumulatedQuery += userInputField.text + "\n";
        AppendTextPrefab(userTextPrefab, userInputField.text);

        //reset user inputfield
        userInputField.text = "";
        userInputField.ActivateInputField();

        //call ChatGPT
        npcAnimator.SetBool(IsLoading, true);
        await OpenAI.OpenAIUtil.InvokeChat(apiKey, _accumulatedQuery);

        //print message
        var receivedMessage = OpenAI.OpenAIUtil.ReceivedMessage;
        AppendTextPrefab(npcTextPrefab, receivedMessage);
        _accumulatedQuery += receivedMessage + "\n";
        
        npcAnimator.SetBool(IsLoading, false);
    }
    
    private GameObject AppendTextPrefab(GameObject prefab, string say)
    {
        var newObject = Instantiate(prefab, npcTextParent.transform);
        newObject.GetComponent<TextMeshProUGUI>().text = say;
        return newObject;
    }
    
    public void MoveScrollToBottom()
    {
        npcScrollRect.verticalNormalizedPosition = 0.0f;
    }
}
