using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventTrigger))]
public class CE_SceneChange : Editor
{
    public EventTrigger.TriggerType type;

    EventTrigger eventTrigger;
    public override void OnInspectorGUI()
    {
        eventTrigger = target as EventTrigger;
        EditorGUILayout.LabelField("Cause", EditorStyles.boldLabel);


        eventTrigger.triggerType = (EventTrigger.TriggerType)EditorGUILayout.EnumPopup("Type", eventTrigger.triggerType);

        EditorGUILayout.Space();

        switch (eventTrigger.triggerType)
        {
            case EventTrigger.TriggerType.Auto:
                DisplayAuto();
                break;
            case EventTrigger.TriggerType.OnKey:
                DisplayOnKey();
                break;
            case EventTrigger.TriggerType.OnAttack:
                DisplayOnAttack();
                break;
            case EventTrigger.TriggerType.OnCall:
                DisplayOnCall();
                break;
        }

        if (eventTrigger.triggerType != EventTrigger.TriggerType.OnAttack)
        {
            Collider2D collide = eventTrigger.gameObject.GetComponent<Collider2D>();
            if (collide != null && collide.isTrigger)
            {
                EditorGUILayout.HelpBox("This will only trigger if the player is touching the collider and the previous condition has been met", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("If you give this object a collider and turn on \"isTrigger\", then the event will only trigger if the player is touching the collider", MessageType.Info);
            }
        }


        EditorGUILayout.LabelField("Effect", EditorStyles.boldLabel);

        eventTrigger.eventType = (EventTrigger.EventType)EditorGUILayout.EnumPopup("Type", eventTrigger.eventType);

        switch (eventTrigger.eventType)
        {
            case EventTrigger.EventType.ChangeScene:
                DisplayChangeScene();
                break;
            case EventTrigger.EventType.StartDialogue:
                DisplayStartDialogue();
                break;
            case EventTrigger.EventType.CallFunction:
                DisplayCallFunction();
                break;
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTrigger"));
        serializedObject.ApplyModifiedProperties();
    }


    private void DisplayAuto () 
    {
        EditorGUILayout.HelpBox("This will trigger automatically", MessageType.Info);
    }
    private void DisplayOnKey () 
    {
        EditorGUILayout.HelpBox("This will trigger when the player presses 'E'", MessageType.Info);
    }
    private void DisplayOnAttack () 
    {
        EventTrigger eventTrigger = target as EventTrigger;
        Collider2D collide = eventTrigger.gameObject.GetComponent<Collider2D>();

        if (collide != null && collide.isTrigger)
        {
            if (collide.isTrigger)
            {
                EditorGUILayout.HelpBox("This will trigger if the player attacks it", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Set the Collider2D \"isTrigger\" to true, or this will never trigger", MessageType.Error);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("This object needs a Collider2D with \"isTrigger\" set to true, without them it can't be triggered", MessageType.Error);
        }
    }
    private void DisplayOnCall () 
    {
        EditorGUILayout.HelpBox("This will trigger when \"TriggerNow()\" is called. \"TryTrigger()\" will TRY to trigger it and return false if it couldn't", MessageType.Info);
    }


    private void DisplayChangeScene()
    {
        if (eventTrigger.enums == null || eventTrigger.enums.Length != 3)
        {
            eventTrigger.enums = new int[] { 0, 0, 0 };
        }
        eventTrigger.enums = new int[] 
        { 
            (int)(EventTrigger.ChangeScene.DoorType)EditorGUILayout.EnumPopup("Door Type", (EventTrigger.ChangeScene.DoorType)eventTrigger.enums[0]),
            
            eventTrigger.enums[0] != (int)EventTrigger.ChangeScene.DoorType.SameRoomDoor ?
            (int)(WorldMaster.Scenes)EditorGUILayout.EnumPopup("Scene Destination", (WorldMaster.Scenes)eventTrigger.enums[1]) : 0,

            eventTrigger.enums[0] == (int)EventTrigger.ChangeScene.DoorType.Door ||eventTrigger.enums[0] == (int)EventTrigger.ChangeScene.DoorType.SameRoomDoor?
            (int)(WorldMaster.Doors)EditorGUILayout.EnumPopup("Door Destinatio", (WorldMaster.Doors)eventTrigger.enums[2]) : 0,
        };
    }

    private void DisplayStartDialogue()
    {
        EditorGUILayout.HelpBox("This code is not ready! Please Stop!", MessageType.Error);
    }

    private void DisplayCallFunction()
    {
        EditorGUILayout.HelpBox("Just put it in the \"OnTrigger\" Unity Event thing", MessageType.Info);
    }
}