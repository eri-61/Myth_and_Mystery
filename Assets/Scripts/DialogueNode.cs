using UnityEngine;
using System;
using System.Collections.Generic;
[Serializable]
public class DialogueNode
{
    public string CharacterName;
    public string DialogueText;
    public string CharacterExpressions;
    public List<Choice> Choices = new List<Choice>();
}

[Serializable]
public class Choice
{
    public string ChoiceText;
    public int NextNodeIndex;
}