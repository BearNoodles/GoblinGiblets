using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IngredientTracker
{
    private static MenuScript.IngredientType currentIngredient;
    private static Sprite currentIngredientSprite;


    public static MenuScript.IngredientType CurrentIngredient { get { return currentIngredient; } set { currentIngredient = value; } }
    public static Sprite CurrentIngredientSprite { get { return currentIngredientSprite; } set { currentIngredientSprite = value; } }
   
	
}
