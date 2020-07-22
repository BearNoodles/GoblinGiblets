using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicketManager : MonoBehaviour {

    // Public game objects for sprites, ingredients and stations
    public Image Dish;
    public GameObject ticket;
    public Animator ticketAni;
    public GameObject[] completes;
    public Image[] stations, ingredients;
    public Sprite[] stationSprites, ingredientSprites, dishSprites;

    // Audio events
    public AK.Wwise.Event ticketSound = new AK.Wwise.Event();

    // Ticket visible status
    bool ticketVisible = false;

    // Set the complete panels to false
    public void Start()
    {
        for (int i = 0; i < completes.Length; i++)
        {
            completes[i].SetActive(false);
        }
    }

    // Set ticket status
    public void ToggleTicket()
    {
        // Set bool to inverse of itself
        ticketVisible = !ticketVisible;

        // Play sound effect
        ticketSound.Post(gameObject);

        // Play animation
        ticketAni.SetBool("Visible", ticketVisible);
    }

    // Set sprites for every station
    public void SetStationSprites(int[] stationInts)
    {
        for (int i = 0; i < stations.Length; i++)
        {
            stations[i].sprite = stationSprites[stationInts[i]];
        }
    }

    // Set ingredients based on dish
    public void SetStationIngredients(MenuScript.Dish dish)
    {
        for(int i = 0; i < ingredients.Length; i++)
        {
            //Key
            // 0 = Brain
            // 1 = Blood
            // 2 = Skull
            // 3 = Rat
            // 4 = Lice
            switch (dish.ingredients[i])
            {
                case MenuScript.IngredientType.brain:
                    ingredients[i].sprite = ingredientSprites[0];
                    break;

                case MenuScript.IngredientType.blood:
                    ingredients[i].sprite = ingredientSprites[1];
                    break;

                case MenuScript.IngredientType.skull:
                    ingredients[i].sprite = ingredientSprites[2];
                    break;

                case MenuScript.IngredientType.rat:
                    ingredients[i].sprite = ingredientSprites[3];
                    break;

                case MenuScript.IngredientType.lice:
                    ingredients[i].sprite = ingredientSprites[4];
                    break;
            }
        }
    }

    // Set random dish
    public void SetDish()
    {
        Dish.sprite = dishSprites[Random.Range(0, dishSprites.Length)];
    }
}