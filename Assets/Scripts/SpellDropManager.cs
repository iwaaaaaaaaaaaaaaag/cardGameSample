using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellDropManager : MonoBehaviour, IDropHandler
{
        public void OnDrop(PointerEventData eventData)
    {

        CardController spellCard = eventData.pointerDrag.GetComponent<CardController>();
        CardController target = GetComponent<CardController>(); // nullの可能性もある

        if (spellCard == null)
        {
            return;
        }
        if (spellCard.CanUseSpell())
        {
            spellCard.UseSpellTo(target);
        }
         
    }
}


