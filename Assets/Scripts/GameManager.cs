using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {

    //Player public object, instance and attached script
    public GameObject playerObject;
    private GameObject player;
    PlayerScript playerScript;

    //Ingredient spawner public object, intance and attached script
    public GameObject ingSpawnerObjectPublic;
    GameObject ingSpawnerObject;
    IngredientSpawner ingredientSpawner;

    //Dish public object and instance
    public GameObject dishObject;
    GameObject completeDish;

    //Main camera object
    GameObject sceneCam;

    //Mess spawner public object, instance and attached script
    public GameObject messSpawnerObjectPublic;
    GameObject messSpawnerObject;
    messScript messSpawner;

    //Position away from mess spawner that mess should spawn
    Vector3 messOffset;
    
    //Scipt for getting ingredient types making up each dish
    MenuScript menuScript;

    //Dish currently being made. Ingredient spawner uses this to choose ingredients to spawn
    MenuScript.Dish menuDish;

    //List of all ingredients currently being used in the level
    List<GameObject> ingredientObjects;

    //Reference to script which instatiates all of the kitchen objects
    SetupKitchen kitchen;

    //Ticket gameobject
    private GameObject ticketObject;

    //Reference to script controlling the ticket
    TicketManager ticketManager;

    //Bool to pause the game when the ticket is open
    bool ticketPause;

    //Bool for pausing the game when the pause button is pressed
    bool buttonPause;

    //Reference to script controlling the slime cube
    CubeScript cube;
    
    //ints to set the current and target score, as well as the number of dishes to be made
    int scoreCurrent, scoreGoal;
    int dishesCurrent, dishesGoal;

    //array to randomly choose a station for each ingredient to go to
    int[] targetStations;

    //Type for the preperation stations with bools to check if they are complete
    struct Station
    {
        //The public gameobject of the station
        public GameObject stationObj;

        //How many times this station has been used
        public int useCount;

        //bool is true when a station minigame is completed
        public bool complete;

        //bool for locking a station from being used
        public bool locked;

        //The ingredient or ingredients that this station is expecting
        public List<MenuScript.IngredientType> targetIngredients;
    }
    
    //Stations currently in game
    Station oven;
    Station chopping;
    Station blender;
    Station windowS;

    //position if the station currently in use
    Vector3 currentStationPos;

    //private bools for checking if a station is locked with public setters
    private bool isOvenLocked, isChoppingLocked, isBlendingLocked;
    public bool SetOvenLocked { set { isOvenLocked = value; } }
    public bool SetChoppingLocked { set { isChoppingLocked = value; } }
    public bool SetBlendingLocked { set { isBlendingLocked = value; } }

    //wise events for window and pause sounds
    public AK.Wwise.Event windowOpenSound = new AK.Wwise.Event();
    public AK.Wwise.Event pauseSound = new AK.Wwise.Event();

    //A large sprite to place in front of the kitchen while a minigame is taking place
    GameObject cover;

    //Keeps track of how many games have been finished
    int minigamesComplete;

    //States for when the game is paused or playing
    public enum stateEnum
    {
        paused, playing
    }
    public stateEnum state;

    //Checks if the scene is paused due to a minigame
    bool isScenePaused;
    
    //GameObject ticket;

    GameObject pauseButton;

    public void Awake()
    {
        //Used in case kitchen is reloaded and destroys new copy of gamemanager
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start ()
    {
        //Finds pause button in the scene by its tag
        pauseButton = GameObject.FindGameObjectWithTag("PauseButton");

        // 0 games completed on startup
        minigamesComplete = 0;

        //Finds the main camera
        sceneCam = GameObject.FindGameObjectWithTag("MainCamera");

        //Sets up tiles, stations and other objects
        kitchen = GetComponent<SetupKitchen>();
        kitchen.Initialize();

        //Spawns a player object
        player = Instantiate<GameObject>(playerObject);
        
        //Gets player script
        playerScript = player.GetComponent<PlayerScript>();


        //Set ingredientspawner object as an instantiated copy and gets its script
        ingSpawnerObject = Instantiate<GameObject>(ingSpawnerObjectPublic);
        ingredientSpawner = ingSpawnerObject.GetComponent<IngredientSpawner>();

        //Set messSpawner object as an instantiated copy and gets its script
        messSpawnerObject = Instantiate<GameObject>(messSpawnerObjectPublic);
        messSpawner = messSpawnerObject.GetComponent<messScript>();

        //Chooses a dish from the menu script at random
        //Later this could possibly be chosen by the player
        ingredientSpawner.SelectDish();

        //Sets up stations for first time use
        GetStations(true);

        //Instantiates and sets a completed dish and sets it as inactive for now
        completeDish = Instantiate<GameObject>(dishObject);
        completeDish.GetComponent<Item>().Type = MenuScript.IngredientType.dish;
        completeDish.SetActive(false);

        //Finds the cover object in the scene and deactivates it
        cover = GameObject.FindGameObjectWithTag("Cover");
        cover.SetActive(false);

        //sets game state to playing
        state = stateEnum.playing;
        isScenePaused = false;

        //Finds cube object and gets its script
        cube = GameObject.FindGameObjectWithTag("Cube").GetComponent<CubeScript>();

        //initailises score variable
        scoreCurrent = 0;

        //Allows MinigameScore script to set the number of dishes to make based on difficulty
        dishesCurrent = 0;
        if(dishesGoal <= 0)
        {
            dishesGoal = MinigameScores.DishTarget;
        }

        //Gets the attached ticketmanager script
        ticketManager = GetComponent<TicketManager>();

        //Gets the ticket onject in the scene
        ticketObject = GameObject.FindGameObjectWithTag("Ticket");

        //Initialises all the sprites for the ticket
        SetupTicket();

        //Locks any station that wont be used for the first round
        if (oven.targetIngredients.Count <= 0)
        {
            oven.locked = true;
        }
        if (chopping.targetIngredients.Count <= 0)
        {
            chopping.locked = true;
        }
        if (blender.targetIngredients.Count <= 0)
        {
            blender.locked = true;
        }

        //ticket = GameObject.FindGameObjectWithTag("Ticket");

        //Ticket start as open so ticket paues is true
        ticketPause = true;
        buttonPause = false;

        //Initialises all scores
        MinigameScores.ResetScores();

        //new list to hold the currently spawned ingredients
        ingredientObjects = new List<GameObject>();
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Holdable"))
        {
            ingredientObjects.Add(g);
            g.GetComponentInChildren<GlowScript>().SetGlowing(true);
        }

    }

    private void Update()
    {

        //If the game is paused and there is now only 1 scene, unpauses
        if (isScenePaused && SceneManager.sceneCount <=1 )
        {
            UnPauseScene();
            return;
        }
        else if(isScenePaused)
        {
            return;
        }

        else if(ticketPause || buttonPause)
        {
            return;
        }

        //Checks how many scenes are loaded and pauses the game if there are more than 1
        if (SceneManager.sceneCount > 1)
        {
            PauseScene();
        }

        //Resets the kitchen if the window game has been completed
        if (windowS.useCount > 0)
        {
            dishesCurrent++;

            //Loads results screen if all dishes have been made
            if(dishesCurrent == dishesGoal)
            {
                dishesCurrent = 0;
                SceneSwitcher.SceneLoader("Result");
            }
            //Otherwise resets kitchen for another play
            else
            {
                ResetKitchen();
            }
        }

        //debug code for adding score
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    MinigameScores.BlendingScore += 100;
        //    scoreCurrent += 100;
        //    AddScore();
        //}
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    MinigameScores.ChoppingScore += 100;
        //    scoreCurrent += 100;
        //    AddScore();
        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    MinigameScores.CookingScore += 100;
        //    scoreCurrent += 100;
        //    AddScore();
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    MinigameScores.ServingScore += 100;
        //    scoreCurrent += 100;
        //    AddScore();
        //}
        //if(Input.GetKeyDown(KeyCode.Backspace))
        //{
        //    dishesCurrent = 0;
        //    SceneSwitcher.SceneLoader("Result");
        //}


    }

    private void SetupTicket()
    {
        //Gets the stations to show in the ticket
        SetTargetStations();

        //Gets which dish from the menu is currently being made
        menuDish = ingredientSpawner.GetComponent<IngredientSpawner>().CurrentDish;

        //Gets the ingredients to show in the ticket
        SetTargetIngredients();

        //Sets the sprites on the ticket script
        ticketManager.SetStationIngredients(menuDish);
        ticketManager.SetDish();
    }

    private void SetTargetStations()
    {
        //Randomises which ingredients go to which stations
        targetStations = new int[] { Random.Range(0, 3), Random.Range(0, 3), Random.Range(0, 3) };
        ticketManager.SetStationSprites(targetStations);
    }

    private void SetTargetIngredients()
    {
        for (int i = 0; i < targetStations.Length; i++)
        {
            switch (targetStations[i])
            {
                case 0:
                    oven.targetIngredients.Add(menuDish.ingredients[i]);
                    break;

                case 1:
                    chopping.targetIngredients.Add(menuDish.ingredients[i]);
                    break;

                case 2:
                    blender.targetIngredients.Add(menuDish.ingredients[i]);
                    break;
            }
        }
        ticketManager.SetStationSprites(targetStations);
    }

    //Transfers score to minigame score scripts and resets it
    private int AddScore()
    {
        int newScore = 0;
        newScore += MinigameScores.BlendingScore;
        newScore += MinigameScores.CookingScore;
        newScore += MinigameScores.ChoppingScore;
        newScore += MinigameScores.ServingScore;

        MinigameScores.BlendingScore = 0;
        MinigameScores.CookingScore = 0;
        MinigameScores.ChoppingScore = 0;
        MinigameScores.ServingScore = 0;

        return newScore;
    }

    //Sets everything necessary back to their starting values
    void ResetKitchen()
    {
        minigamesComplete = 0;

        ingredientSpawner.SelectDish();
        //ingredientSpawner.SelectDish("HandSpongeRat");

        //sets up stations for not first time use
        GetStations(false);

        //creates a new dish item object and deactivates it for now
        completeDish = Instantiate<GameObject>(dishObject);
        completeDish.GetComponent<Item>().Type = MenuScript.IngredientType.dish;
        completeDish.SetActive(false);

        //deactivates the cover
        cover.SetActive(false);

        //makes sure state is playing again
        state = stateEnum.playing;
        isScenePaused = false;

        //start with ticket active
        ticketObject.SetActive(true);

        SetupTicket();

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        //list to hold ingredients
        ingredientObjects = new List<GameObject>();
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Holdable"))
        {
            ingredientObjects.Add(g);
        }

        SetIngredientsGlow(true);
    }

    void GetStations(bool firstSetup)
    {
        //Only needs to find the objects on first time setup, not on reset
        if (firstSetup)
        {
            oven.stationObj = GameObject.FindGameObjectWithTag("Oven");
            chopping.stationObj = GameObject.FindGameObjectWithTag("Chopping");
            blender.stationObj = GameObject.FindGameObjectWithTag("Blender");
            windowS.stationObj = GameObject.FindGameObjectWithTag("WindowS");
        }

        //All stations are unused and not completed on startup and after reset
        oven.useCount = 0;
        chopping.useCount = 0;
        blender.useCount = 0;
        windowS.useCount = 0;

        //Doesnt initially lock any stations
        oven.locked = false;
        chopping.locked = false;
        blender.locked = false;
        windowS.locked = false;

        //No stations complete at the start
        oven.complete = false;
        chopping.complete = false;
        blender.complete = false;
        windowS.complete = false;

        //Gets which ingredients each station requires
        oven.targetIngredients = new List<MenuScript.IngredientType>();
        chopping.targetIngredients = new List<MenuScript.IngredientType>();
        blender.targetIngredients = new List<MenuScript.IngredientType>();
        windowS.targetIngredients = new List<MenuScript.IngredientType>();

        //Manually set the dish as the target ingredient for the window
        windowS.targetIngredients.Add(MenuScript.IngredientType.dish);

        //Open window does not show until ready to use
        //A closed window sprite is shown instead
        windowS.stationObj.SetActive(false);
    }
    

 
    //Disabled a specific station
    public void StationUsed(int station, MenuScript.IngredientType type)
    {
        //Sets the default scene to load as the kitchen
        string sceneToLoad = "Scene2";

        //Loads specified minigame scene and increments completed games
        switch (station)
        {
            case 0:
                oven.useCount++;
                oven.targetIngredients.Remove(type);
                if(oven.targetIngredients.Count <= 0)
                {
                    //Locks station if it does not require anymore ingredients
                    oven.locked = true;
                }
                //Gets the position of the current in use station to spawn mess in the right place
                currentStationPos = oven.stationObj.transform.position;
                messOffset = new Vector3(0.0f, -3.0f, 0.0f);
                minigamesComplete++;
                sceneToLoad = "MinigameScene";
                break;

            case 1:
                chopping.useCount++;
                chopping.targetIngredients.Remove(type);
                if (chopping.targetIngredients.Count <= 0)
                {
                    //Locks station if it does not require anymore ingredients
                    chopping.locked = true;
                }
                //Gets the position of the current in use station to spawn mess in the right place
                currentStationPos = chopping.stationObj.transform.position;
                messOffset = new Vector3(-3.5f, 1.0f, 0.0f);
                minigamesComplete++;
                sceneToLoad = "SlicingGame";
                break;

            case 2:
                blender.useCount++;
                blender.targetIngredients.Remove(type);
                if (blender.targetIngredients.Count <= 0)
                {
                    //Locks station if it does not require anymore ingredients
                    blender.locked = true;
                }
                //Gets the position of the current in use station to spawn mess in the right place
                currentStationPos = blender.stationObj.transform.position;
                messOffset = new Vector3(2.5f, 1.0f, 0.0f);
                minigamesComplete++;
                sceneToLoad = "WheelMinigame";
                break;

            case 3:
                windowS.useCount++;
                windowS.locked = true;

                //Gets the position of the current in use station to spawn mess in the right place
                currentStationPos = windowS.stationObj.transform.position;
                messOffset = new Vector3(0.0f, -6.0f, 0.0f);
                minigamesComplete++;
                sceneToLoad = "ServingMinigameScene";
                break;
        }

        //Locks all minigames and activates the dish if 3 games have been complete
        if (minigamesComplete >= 3)
        {
            oven.locked = true;
            chopping.locked = true;
            blender.locked = true;
            completeDish.SetActive(true);
        }

        //Pauses game
        state = stateEnum.paused;
        SceneSwitcher.LoadSceneAdd(sceneToLoad);

        //ingredients glow again after each game 
        SetIngredientsGlow(true);
    }

    //Returns the used status of a specified station
    public bool CheckStation(int station, MenuScript.IngredientType type)
    {
        //Temp station for checking status
        Station stationToCheck = new Station();
        stationToCheck.locked = false;

        //Sets temp station as one of the four stations
        switch (station)
        {
            case 0:
                stationToCheck = oven;
                break;

            case 1:
                stationToCheck = chopping;
                break;

            case 2:
                stationToCheck = blender;
                break;

            case 3:
                stationToCheck = windowS;
                break;
        }

        //Returns false if station is locked
        if(stationToCheck.locked)
        {
            return false;
        }

        //Otherwise checks if the provided ingredient type matches any of the temp stations target ingredients
        else
        {
            foreach (MenuScript.IngredientType i in stationToCheck.targetIngredients)
            {
                if (type == i)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //Pauses the scene while a minigame is playing
    public void PauseScene()
    {
        //Returns if it is already paused
        if (isScenePaused)
        {
            return;
        }

        //Hides the pause button during minigame
        pauseButton.SetActive(false);
        
        //Hides the ticket object and stops the player from interacting with it
        ticketObject.GetComponent<CanvasGroup>().alpha = 0;
        ticketObject.GetComponent<CanvasGroup>().interactable = false;
        ticketObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        sceneCam.gameObject.SetActive(false);
        isScenePaused = true;

        //sets kitchen state to paused
        state = stateEnum.paused;

        //activates background cover
        cover.SetActive(true);
    }

    //Unpauses the scene after a minigame
    public void UnPauseScene()
    {
        //Shows the pause button again when scene unpaused and makes the ticket interactable again
        pauseButton.SetActive(true);
        ticketObject.GetComponent<CanvasGroup>().alpha = 1;
        ticketObject.GetComponent<CanvasGroup>().interactable = true;
        ticketObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        isScenePaused = false;
        sceneCam.gameObject.SetActive(true);
        state = stateEnum.playing;

        //Hides the background cover
        cover.SetActive(false);

        //makes mess at the recently used station
        messSpawner.MakeMess(currentStationPos + messOffset, 1);

        //updates score
        scoreCurrent += AddScore();
    }
    
    //Pauses/unpauses gameplay while ticket is open/closed
    public void TicketPauseToggle()
    {
        ticketPause = !ticketPause;
        playerScript.MoveDelay = true;
    }

    //For pausing the kitchen scene when the pause button is clicked
    public void ButtonPauseOn()
    {
        buttonPause = true;
        playerScript.MoveDelay = true;
        pauseSound.Post(gameObject);
    }

    //Unpauses kitchen scene after pausing with pause button
    public void ButtonPauseOff()
    {
        buttonPause = false;
        playerScript.MoveDelay = true;
        pauseSound.Post(gameObject);
    }

    public bool GetTicketPause()
    {
        return ticketPause;
    }

    public bool GetButtonPause()
    {
        return buttonPause;
    }

    //Returns which state the kitchen scene is currently in
    public stateEnum GetState()
    {
        return state;
    }

    //Sets a station to glow when its target ingredient has been picked up
    public void SetStationGlow(int station, bool glowing)
    {
        switch (station)
        {
            case 0:
                oven.stationObj.GetComponentInChildren<GlowScript>().SetGlowing(glowing);
                break;

            case 1:
                chopping.stationObj.GetComponentInChildren<GlowScript>().SetGlowing(glowing);
                break;

            case 2:
                blender.stationObj.GetComponentInChildren<GlowScript>().SetGlowing(glowing);
                break;

            case 3:
                windowS.stationObj.GetComponentInChildren<GlowScript>().SetGlowing(glowing);
                break;

            default:
                break;
        }
    }

    //Sets ingredients to all glow
    public void SetIngredientsGlow(bool glow)
    {
        foreach (GameObject g in ingredientObjects)
        {
            if(g != null)
            {
                g.GetComponentInChildren<GlowScript>().SetGlowing(glow);
            }
        }
    }

    //Activates the open window animation and sets the window station to active
    public void OpenWindow()
    {
        windowS.stationObj.SetActive(true);
        windowS.stationObj.GetComponent<Animator>().Play("Window");

        windowOpenSound.Post(gameObject);
    }
}
