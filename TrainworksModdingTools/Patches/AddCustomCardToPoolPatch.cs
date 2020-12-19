﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using HarmonyLib;
using Trainworks.Managers;
using System.Diagnostics;
using System.Linq;

namespace Trainworks.Patches
{
    /// <summary>
    /// Adds custom cards to their appropriate pools.
    /// This one is used particularly when choosing for a card reward.
    /// </summary>
    [HarmonyPatch(typeof(CardPoolHelper), "GetCardsForClass")]
    [HarmonyPatch(new Type[] { typeof(CardPool), typeof(ClassData), typeof(CollectableRarity), typeof(CardPoolHelper.RarityCondition), typeof(bool) })]
    class AddCustomCardToPoolPatch
    {
        static void Postfix(ref List<CardData> __result, ref CardPool cardPool, ClassData classData, CollectableRarity paramRarity, CardPoolHelper.RarityCondition rarityCondition, bool testRarityCondition)
        {
            List<CardData> customCardsToAddToPool = CustomCardPoolManager.GetCardsForPoolSatisfyingConstraints(cardPool.name, classData, paramRarity, rarityCondition, testRarityCondition);
            __result.AddRange(customCardsToAddToPool);
        }
    }

    /// <summary>
    /// Adds custom cards to their appropriate pools.
    /// This one is used particularly when pulling cards from card effect mid-battle.
    /// </summary>
    [HarmonyPatch]
    class AddCustomCardToPoolPatch2
    {
        static MethodBase TargetMethod()
        {
            var methods = typeof(CardEffectState)
                .GetMethods()
                .Where(method => method.Name == "GetFilteredCardListFromPool")
                .Where(method => !method.IsStatic)
                .Cast<MethodBase>();
            return methods.First();
        }

        static void Postfix(ref bool __result, ref CardPool ___paramCardPool, CardUpgradeMaskData ___paramCardFilter, RelicManager relicManager, ref List<CardData> toProcessCards)
        {
            List<CardData> customCardsToAddToPool = CustomCardPoolManager.GetCardsForPoolSatisfyingConstraints(___paramCardPool.name, ___paramCardFilter, relicManager);
            toProcessCards.AddRange(customCardsToAddToPool);
            __result = toProcessCards.Count > 0;
        }
    }

    /// <summary>
    /// Adds custom cards to their appropriate pools.
    /// This one is used particularly when loading assets during loading screens.
    /// </summary>
    [HarmonyPatch]
    class AddCustomCardToPoolPatch3
    {
        static MethodBase TargetMethod()
        {
            var methods = typeof(CardEffectState)
                .GetMethods()
                .Where(method => method.Name == "GetFilteredCardListFromPool")
                .Where(method => method.IsStatic)
                .Cast<MethodBase>();
            return methods.First();
        }

        static void Postfix(ref bool __result, CardPool paramCardPool, CardUpgradeMaskData paramCardFilter, RelicManager relicManager, ref List<CardData> toProcessCards)
        {
            if (paramCardPool != null)
            {
                List<CardData> customCardsToAddToPool = CustomCardPoolManager.GetCardsForPoolSatisfyingConstraints(paramCardPool.name, paramCardFilter, relicManager);
                toProcessCards.AddRange(customCardsToAddToPool);
                __result = toProcessCards.Count > 0;
            }
        }
    }
}
