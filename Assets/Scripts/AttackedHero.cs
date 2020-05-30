using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//攻撃される側の処理
public class AttackedHero : MonoBehaviour, IDropHandler
{
        public void OnDrop(PointerEventData eventData)
    {

        /* 攻撃 */
        // attackerカードを取得
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();

        if (attacker == null )
        {
            return;
        }
        // 敵フィールドにシールドカードがあれば攻撃できない
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards(attacker.cardModel.isPlayerCard);
        if(Array.Exists(enemyFieldCards,card => card.cardModel.ability == ABILITY.SHIELD))
        {
            return;
        }

        if(attacker.cardModel.canAttack)
        {
            // attackerがHeroに攻撃する
            GameManager.instance.AttackToHero(attacker);
            GameManager.instance.CheckHeroHP();
        }

    }
}
