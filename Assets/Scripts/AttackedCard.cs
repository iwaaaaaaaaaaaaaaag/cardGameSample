using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//攻撃される側の処理
public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /* 攻撃 */
        // attackerカードを取得
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        // difenderカードを取得（Playerフィールドから選択）
        CardController defender = GetComponent<CardController>();

        if (attacker == null || defender == null)
        {
            return;
        }
        if(attacker.cardModel.isPlayerCard == defender.cardModel.isPlayerCard)
        {
            return;
        } 

        // シールドカードがあれば、シールドカード以外は攻撃できない
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards(attacker.cardModel.isPlayerCard);
        if(Array.Exists(enemyFieldCards,card => card.cardModel.ability == ABILITY.SHIELD ) && defender.cardModel.ability != ABILITY.SHIELD)
        {
            return;
        }

        if(attacker.cardModel.canAttack)
        {
            // attackerとdiffenderを戦わせる
            GameManager.instance.CardsBattle(attacker,defender );
        }

    }
}
