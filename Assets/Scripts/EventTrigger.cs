using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EventTrigger : MonoBehaviour
{
    public interface ITrigger
    {
        public void TriggerEvent();
    }
    public enum TriggerType
    {
        Auto,
        OnKey,
        OnAttack,
        OnCall,
    }
    public enum EventType
    {
        ChangeScene,
        StartDialogue,
        CallFunction,
    }
    public Collider2D collide;
    private bool mustTouch = false, isTouching = false;
    
    public delegate bool IsTriggerTripped();
    public event IsTriggerTripped CheckTrigger;

    public UnityEvent OnTrigger;

    public TriggerType triggerType;
    public EventType eventType;
    public int[] enums;

    ITrigger @event;
    
    // Start is called before the first frame update
    void Start()
    {
        switch (triggerType)
        {
            case TriggerType.Auto:
                CheckTrigger = Auto;
                break;
            case TriggerType.OnKey:
                CheckTrigger = OnKey;
                break;
            case TriggerType.OnAttack:
                if (collide == null || !collide.isTrigger)
                {
                    Debug.LogError($"The event in {gameObject.name} can't be triggered due to lack of a trigger collider");
                }
                CheckTrigger = OnAttack;
                break;
            case TriggerType.OnCall:
                CheckTrigger = OnCall;
                break;
        }

        collide = gameObject.GetComponent<Collider2D>();
        if (collide != null && collide.isTrigger)
        {
            mustTouch = true;
        }
        
        switch (eventType)
        {
            case EventType.ChangeScene:
                @event = new ChangeScene((ChangeScene.DoorType) enums[0],  (WorldMaster.Scenes) enums[1], (WorldMaster.Doors) enums[2]);
                break;
            case EventType.StartDialogue:
                throw new System.ArgumentException("Please don't make a dialogue calling event yet, it's not ready yet and needs some time alone");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((!mustTouch || isTouching) && CheckTrigger())
        {
            if (eventType != EventType.CallFunction)
            {
                @event.TriggerEvent();
            }
            OnTrigger.Invoke();
        }
    }

    public bool TryTrigger()
    {
        if ((!mustTouch || isTouching) && CheckTrigger())
        {
            if (eventType != EventType.CallFunction)
            {
                @event.TriggerEvent();
            }
            OnTrigger.Invoke();
            return true;
        }
        return false;
    }

    public void TriggerNow()
    {
        if (eventType != EventType.CallFunction)
        {
            @event.TriggerEvent();
        }
        OnTrigger.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Player>() != null && triggerType != TriggerType.OnAttack)
        {
            isTouching = true;
        } 
        else if (triggerType == TriggerType.OnAttack && collision.gameObject.GetComponent<Hitbox>() != null)
        {
            isTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null && triggerType != TriggerType.OnAttack)
        {
            isTouching = false;
        }
        else if (triggerType == TriggerType.OnAttack && collision.gameObject.GetComponent<Hitbox>() != null)
        {
            isTouching = false;
        }
    }

    private bool Auto()
    {
        return true;
    }
    private bool OnKey()
    {
        // Put some code here for displaying the "hit the E key" icon/animation
        return Input.GetKeyDown(KeyCode.E);
    }
    private bool OnAttack()
    {
        return true;
    }
    private bool OnCall()
    {
        return false;
    }

    /// <summary>
    /// A class for remembering and executing Change Scene events
    /// </summary>
    public class ChangeScene : ITrigger {
        public enum DoorType
        {
            ToLevel,
            ToMenu,
            Door,
            SameRoomDoor,
        }

        DoorType type;
        WorldMaster.Scenes sceneDestination;
        WorldMaster.Doors? door;

        public ChangeScene (DoorType Type, WorldMaster.Scenes SceneDestination, WorldMaster.Doors Door)
        {
            sceneDestination = SceneDestination;
            door = Door;
            type = Type;

            if (door == WorldMaster.Doors.Start && type != DoorType.ToLevel)
                Debug.LogWarning("The Start door is reserved for when you start a new level. If you're starting a level, set the ChangeScene type to \"ToLevel\"");

            switch (type)
            {
                case DoorType.ToLevel:
                    door = WorldMaster.Doors.Start;
                    break;
                case DoorType.ToMenu:
                    door = null;
                    break;
                case DoorType.Door:
                    break;
                case DoorType.SameRoomDoor:
                    break;
            }
            if (SceneDestination == WorldMaster.Scenes.NoWhere && type != DoorType.SameRoomDoor)
                throw new System.NullReferenceException("*Cowboy Voice* Where do you think you're going, pardner? Seriously, where? This thing leads to NoWhere (a scene which doesn't exist)");
        }

        public void TriggerEvent()
        {
            if (type == DoorType.SameRoomDoor)
            {
                WorldMaster.MoveToDoor((WorldMaster.Doors)door);
            }
            else
            {
                if (type == DoorType.ToLevel)
                {
                    WorldMaster.ClearPlayerStats();
                }
                else
                {
                    WorldMaster.CopyPlayerStats();
                }
                WorldMaster.LoadScene(sceneDestination, door);
            }
        }

    }
}