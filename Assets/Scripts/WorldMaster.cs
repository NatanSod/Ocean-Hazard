using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A static class that is what it's called
/// </summary>
public static class WorldMaster
{
    public enum Scenes
    {
        NoWhere,
        Loading,
        TestScene,
    }
    public enum Doors
    {
        Start,
        Left,
        Right,
        Up,
        Down,
        Other,
    }
    private static Dictionary<Doors, GameObject> _doorsInScene = new Dictionary<Doors, GameObject>();

    public static void AddDoorToList(Doors Door, GameObject Target)
    {
        if (_doorsInScene.ContainsKey(Door))
        {
            throw new System.ArgumentException($"{_doorsInScene[Door].name} and {Target.name} are both {Door} doors, one of them is going to have to change");
        }
        Debug.Log($"{Target.name}, the {Door} door, has been added to the list");
        _doorsInScene.Add(Door, Target);
    }

    private static float? playerHealth = null;
    private static int? playerFaceDir = null;

    private static Player player = null;
    /// <summary>
    /// Get the player object's <see cref="Player"/> component
    /// </summary>
    /// <remarks>
    /// Finds the player through <see cref="GameObject.Find(string)"/>
    /// and saves it in the private <see cref="player"/> field for easy access in the future
    /// </remarks>
    /// <returns>The player object's <see cref="Player"/> component</returns>
    public static Player GetPlayer()
    {
        if (player == null)
        {
            if (GameObject.Find("Player") == null) { return null; }
            player = GameObject.Find("Player").GetComponent<Player>();
        }
        return player;
    }

    /// <summary>
    /// Copies and remembers the player's current values which need to carry over between scenes
    /// </summary>
    /// <remarks>
    /// Load the saved stats with <see cref="PastePlayerStats()"/>
    /// </remarks>
    public static void CopyPlayerStats()
    {
        if (GetPlayer() == null)
        {
            Debug.Log("Ayo! There's no player to get the stats from! How are we supposed to do our job now?");
            return;
        }

        playerHealth = player.health;
        playerFaceDir = player.faceDir;
    }

    /// <summary>
    /// Overwrites the player's values that need to carry over between scenes
    /// </summary>
    /// <remarks>
    /// It overwrites it with the stats it remembers from <see cref="CopyPlayerStats()"/>
    /// </remarks>
    public static void PastePlayerStats()
    {
        if (GetPlayer() == null)
        {
            Debug.Log("There is no player in this scene, therefore this should be a menu. If it isn't, shame on you");
            return;
        }
        player.health = playerHealth ?? player.maxHealth;
        player.faceDir = playerFaceDir ?? 1;
    }

    /// <summary>
    /// Sets the remembered stats to null
    /// </summary>
    public static void ClearPlayerStats()
    {
        playerHealth = null;
        playerFaceDir = null;
    }

    /// <summary>
    /// A <see cref="MonoBehaviour"/> class that <see cref="WorldMaster"/> uses to run coroutines
    /// </summary>
    private class Coroutiner : MonoBehaviour 
    {
        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
    
    static GameObject coroutiner;
    /// <summary>
    /// Changes the scene
    /// </summary>
    /// <remarks>
    /// This should only be called by <see cref="EventTrigger"/>
    /// </remarks>
    /// <param name="Scene">The scene to change to</param>
    /// <param name="ToDoor">The "door" to put the player in front of</param>
    public static void LoadScene(Scenes Scene, Doors? ToDoor)
    {
        GetPlayer();
        _doorsInScene.Clear();

        coroutiner = new GameObject("Coroutiner");
        coroutiner.AddComponent<Coroutiner>().StartCoroutine(LoadAsync(Scene, ToDoor));
    }

    private static IEnumerator LoadAsync(Scenes Scene, Doors? ToDoor)
    {
        float time = Time.realtimeSinceStartup;
        Debug.Log($"Loading began at {time}");
        SceneManager.LoadScene(Scenes.Loading.ToString());
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Scene.ToString());

        while(!asyncOperation.isDone) {
            Debug.Log("Another try");
            yield return null;
           
        }
        Debug.Log($"Loading finished at {Time.realtimeSinceStartup}, it took {Time.realtimeSinceStartup - time} seconds");

        

        GetPlayer();
        if (player != null && ToDoor != null)
        {
            PastePlayerStats();

            MoveToDoor((Doors)ToDoor);

            Object.Destroy(coroutiner);
        }
        else if (ToDoor != null)
        {
            throw new System.NullReferenceException($"The {SceneManager.GetActiveScene().name} scene is entered as if it is a level, however there is no player object." + 
                "I don't know what is wrong, but I know something is wrong and you need to fix it");
            
        }
    }

    /// <summary>
    /// Puts the player in front of a door
    /// </summary>
    /// <param name="destination">The door to put the player in front of</param>
    public static void MoveToDoor(Doors destination)
    {
        if (GetPlayer() == null)
        {
            throw new System.NullReferenceException("How on EARTH, am I supposed to put the player in front of the door, if there is NO PLAYER?");
        }
        GameObject door = _doorsInScene[destination];
        if (door != null)
        {
            player.gameObject.transform.position = door.transform.position;
        }
        else
        {
            throw new System.NullReferenceException($"The {destination} door does not exist in the {SceneManager.GetActiveScene().name} scene, please fix this fuck-up appropriately");
        }
    }
}
