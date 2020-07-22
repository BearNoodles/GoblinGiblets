using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour {
    
    //Attached menu script and a variable for the current dish
    MenuScript menu;
    MenuScript.Dish currentDish;

    public MenuScript.Dish CurrentDish { get { return currentDish; } }

    //Spawn positions for the ingredients
    Vector2[] spawnPositions;

    //Current spawn position to iterate through position list
    int currentPos;

    //List of ingredients
    List<GameObject> ingredients;

    //Public variables for ingredient prefabs
    public GameObject ingredient0;
    public GameObject ingredient1;
    public GameObject ingredient2;
    public GameObject ingredient3;
    public GameObject ingredient4;
    public GameObject ingredient5;
    public GameObject ingredient6;
    public GameObject ingredient7;
    public GameObject ingredient8;
    public GameObject ingredient9;
    public GameObject ingredient10;
    public GameObject ingredient11;

    // Use this for initialization
    void Awake ()
    {
        //Get the attached menuscript
        menu = GetComponent<MenuScript>();

        currentPos = 0;

        //Hard coded spawn position
        //TODO add a lot more possible positions for ingredients to spawn
        spawnPositions = new Vector2[3];
        spawnPositions[0] = new Vector3(Random.Range(-3.0f, 3.0f), -4.0f, 0.0f);
        spawnPositions[1] = new Vector3(-7.0f, 0.0f, 0.0f);
        spawnPositions[2] = new Vector3(7.0f, 0.0f, 0.0f);
    }

    //Gets a random dish from the menuscript and returns it
    public MenuScript.Dish SelectDish()
    {
        currentDish = GetDishRandom();
        SpawnIngredients();

        return currentDish;
    }

    //Gets a specified dish from the menu script
    public MenuScript.Dish SelectDish(string name)
    {
        currentDish = GetDish(name);
        SpawnIngredients();

        return currentDish;
    }

    //Spawns ingredients
    void SpawnIngredients()
    {
        //Creates a temporary object to instantiate
        GameObject tempObj;
        tempObj = new GameObject();
        MenuScript.IngredientType tempType = MenuScript.IngredientType.bat;

        //Loops through all ingredients from the current dish and sets tempobj to it and instantiats it
        foreach (MenuScript.IngredientType i in currentDish.ingredients)
        {
            switch (i)
            {
                case MenuScript.IngredientType.bat:
                    tempObj = ingredient0;
                    tempType = MenuScript.IngredientType.bat;
                    break;

                case MenuScript.IngredientType.blood:
                    tempObj = ingredient1;
                    tempType = MenuScript.IngredientType.blood;
                    break;

                case MenuScript.IngredientType.brain:
                    tempObj = ingredient2;
                    tempType = MenuScript.IngredientType.brain;
                    break;

                case MenuScript.IngredientType.carrot:
                    tempObj = ingredient3;
                    tempType = MenuScript.IngredientType.carrot;
                    break;

                case MenuScript.IngredientType.eye:
                    tempObj = ingredient4;
                    tempType = MenuScript.IngredientType.eye;
                    break;

                case MenuScript.IngredientType.goblin:
                    tempObj = ingredient5;
                    tempType = MenuScript.IngredientType.goblin;
                    break;

                case MenuScript.IngredientType.lice:
                    tempObj = ingredient6;
                    tempType = MenuScript.IngredientType.lice;
                    break;

                case MenuScript.IngredientType.ingredient:
                    tempObj = ingredient7;
                    tempType = MenuScript.IngredientType.ingredient;
                    break;

                case MenuScript.IngredientType.key:
                    tempObj = ingredient8;
                    tempType = MenuScript.IngredientType.key;
                    break;

                case MenuScript.IngredientType.mystery:
                    tempObj = ingredient9;
                    tempType = MenuScript.IngredientType.mystery;
                    break;

                case MenuScript.IngredientType.rat:
                    tempObj = ingredient10;
                    tempType = MenuScript.IngredientType.rat;
                    break;

                case MenuScript.IngredientType.skull:
                    tempObj = ingredient11;
                    tempType = MenuScript.IngredientType.skull;
                    break;
            }
            GameObject tempInstance = Instantiate<GameObject>(tempObj, spawnPositions[currentPos], Quaternion.identity);
            tempInstance.GetComponent<Item>().Type = tempType;
            currentPos++;
        }
        currentPos = 0;
        
    }

    //Gets specific dish from menu
    MenuScript.Dish GetDish(string id)
    {
        return menu.GetDish(id);
    }

    //Gets random dish from menu
    MenuScript.Dish GetDishRandom()
    {
        return menu.GetDishRandom();
    }
}
