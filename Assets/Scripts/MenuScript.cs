using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour {

    //Enumeration to hold each type of ingredient
    public enum IngredientType
    {
        bat, blood, brain, carrot, eye, goblin,
        ingredient, key, lice, mystery, rat, skull, dish
    }
                                      
    //Dishes have an string to ID them and a likst of ingredients type
    //id and ingredients set when new dish is created
    public struct Dish
    {
        public string id;
        public List<IngredientType> ingredients;

        public Dish(string name, List<IngredientType> ings)
        {
            id = name;
            ingredients = ings;
        }
    }

    //list of all dishes
    List<Dish> menu;

    // Use this for initialization
    void Awake()
    {
        //Initialise and set some dishes in the menu
        menu = new List<Dish>();
        menu.Add(AddMenuItem("LiceSkullRat", IngredientType.lice, IngredientType.skull, IngredientType.rat));
        //menu.Add(AddMenuItem("MysteryBloodCarrot", IngredientType.mystery, IngredientType.blood, IngredientType.carrot));
        menu.Add(AddMenuItem("BloodBrainRat", IngredientType.blood, IngredientType.brain, IngredientType.rat));
        //menu.Add(AddMenuItem("EyeEyeEye", IngredientType.eye, IngredientType.eye, IngredientType.eye));

        //menu = FillMenu();
    }

    //adds new dish to menu list
    Dish AddMenuItem(string name, IngredientType ing0, IngredientType ing1, IngredientType ing2)
    {
        //Temporary list of ingredients to create new dish with
        List<IngredientType> ings = new List<IngredientType>();
        
        ings.Add(ing0);
        ings.Add(ing1);
        ings.Add(ing2);
        Dish newDish = new Dish(name, ings);

        return newDish;
    }

    List<Dish> FillMenu()
    {
        List<Dish> tempMenu = new List<Dish>();
        //Temporary list of ingredients to create new dish with
        List<IngredientType> ings = new List<IngredientType>();
        //MysteryBloodCarrot
        ings.Add(IngredientType.mystery);
        ings.Add(IngredientType.blood);
        ings.Add(IngredientType.carrot);
        tempMenu.Add(new Dish("MysteryBloodCarrot", ings));
        ings.Clear();

        //HandSpongeRat
        ings.Add(IngredientType.lice);
        ings.Add(IngredientType.skull);
        ings.Add(IngredientType.rat);
        tempMenu.Add(new Dish("LiceSpongeRat", ings));
        ings.Clear();

       

        return tempMenu;
    }

    //Searches the menu and returns the dish with the given ID
    public Dish GetDish(string searchID)
    {
        Dish dish;
        foreach(Dish d in menu)
        {
            if (d.id == searchID)
            {
                dish = d;
                return dish;
            }
        }
        dish = new Dish("", null);
        return dish;
    }

    //Picks a random dish
    public Dish GetDishRandom()
    {
        int menuSize = menu.Count;
        int randish = (int)(Random.Range(0.1f, menuSize) - 0.1f);
        return menu[randish];
    }
}
