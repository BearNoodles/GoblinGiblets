using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this static script is used to store the scores of the various minigames and related information between unity scenes
// original script by Ben Rogers, updated with more variables etc by Daryl Grant
public static class MinigameScores
{
    private static int cookingScore, cookingScoreTotal, servingScore, servingScoreTotal,
        choppingScore, choppingScoreTotal, blendingScore, blendingScoreTotal, dishTarget, scoreTarget, totalScore, difficultyId;

    // getters and setters for the variables
    public static int CookingScore
    {
        get
        {
            return cookingScore;
        }
        set
        {
            cookingScore = value;
            cookingScoreTotal += value;
            totalScore += value;
        }
    }

    public static int CookingScoreTotal
    {
        get
        {
            return cookingScoreTotal;
        }
        set
        {
            cookingScoreTotal = value;
        }
    }

    public static int DifficultyId
    {
        get
        {
            return difficultyId;
        }
        set
        {
            difficultyId = value;
        }
    }

    public static int ServingScore
    {
        get
        {
            return servingScore;
        }
        set
        {
            servingScore = value;
            servingScoreTotal += value;
            totalScore += value;
        }
    }

    public static int ServingScoreTotal
    {
        get
        {
            return servingScoreTotal;
        }
        set
        {
            servingScoreTotal = value;
        }
    }

    public static int ChoppingScore
    {
        get
        {
            return choppingScore;
        }
        set
        {
            choppingScore = value;
            choppingScoreTotal += value;
            totalScore += value;
        }
    }

    public static int ChoppingScoreTotal
    {
        get
        {
            return choppingScoreTotal;
        }
        set
        {
            choppingScoreTotal = value;
        }
    }

    public static int BlendingScore
    {
        get
        {
            return blendingScore;
        }
        set
        {
            blendingScore = value;
            BlendingScoreTotal += value;
            totalScore += value;
        }
    }

    public static int BlendingScoreTotal
    {
        get
        {
            return blendingScoreTotal;
        }
        set
        {
            blendingScoreTotal = value;
        }
    }

    public static int TotalScore
    {
        get
        {
            return totalScore;
        }
        set
        {
            totalScore = value;
        }
    }

    public static int ScoreTarget
    {
        get
        {
            return scoreTarget;
        }
        set
        {
            scoreTarget = value;
        }
    }

    public static int DishTarget
    {
        get
        {
            return dishTarget;
        }
        set
        {
            dishTarget = value;
        }
    }

    // reset all the scores
    public static void ResetScores()
    {
        cookingScore = 0;
        servingScore = 0;
        choppingScore = 0;
        blendingScore = 0;

        cookingScoreTotal = 0;
        servingScoreTotal = 0;
        choppingScoreTotal = 0;
        blendingScoreTotal = 0;
        totalScore = 0;
    }
}
