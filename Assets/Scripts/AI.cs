using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public IEnumerator EnemyTurn()
    {
        Debug.Log("enemyのターン");
        //フィールドのカードを攻撃可能にする
        CardController[] enemyFieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        gameManager.SettingCanAttackView(enemyFieldCardList,true);

        yield return new WaitForSeconds(1);

        /* 場にカードを出す */
        // 手札のカードリストを取得
        CardController[] handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();

        // コスト以下のカードがあれば、カードをフィールドに出し続ける
        // 条件：モンスターカードならコストのみ
        // 条件：スペルならコストと、使用可能かどうか(CanUseSpell)
        while (Array.Exists(handCardList,card => 
        card.cardModel.cost <= gameManager.enemy.manaCost
        && (( !card.IsSpell ) || (card.IsSpell && card.CanUseSpell())) ))
        {
            // コスト以下のカードリストを取得
            CardController[] selectableHandCardList = Array.FindAll(handCardList,
                card => card.cardModel.cost <= gameManager.enemy.manaCost
                && (( !card.IsSpell ) || (card.IsSpell && card.CanUseSpell())) );
            // 場に出すカードを選択
            CardController selectCard = selectableHandCardList[0];
            // カードを表にする
            selectCard.Show();
            // スペルカードなら使用する
            if (selectCard.IsSpell)
            {
                StartCoroutine(CastSpellOf(selectCard));
            }
            else
            {
                // カードを移動
                StartCoroutine(selectCard.cardMovement.MoveToField(gameManager.enemyFieldTransform));
                selectCard.OnFiled();
            }
            yield return new WaitForSeconds(1);
            handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();

        }

        yield return new WaitForSeconds(1);




        /* 攻撃 */
        // フィールドのカードリストを取得
        CardController[] fieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();        
        //攻撃可能カードがあれば攻撃を繰り返す
        while(Array.Exists(fieldCardList,card =>card.cardModel.canAttack))
        {
            Debug.Log(Array.Exists(fieldCardList,card =>card.cardModel.canAttack));
         // 攻撃可能カードを取得
        CardController[] enemyCanAttackCardList = Array.FindAll(fieldCardList,card =>card.cardModel.canAttack);
        CardController[] playerFieldCardList = gameManager.playerFieldTransform.GetComponentsInChildren<CardController>();

            

            // attackerカードを取得
            CardController attacker = enemyCanAttackCardList[0];
            
            if(playerFieldCardList.Length > 0)
            {
            // difenderカードを取得（Playerフィールドから選択）
            // シールドカードのみ攻撃対象にする
            if(Array.Exists(playerFieldCardList, card => card.cardModel.ability == ABILITY.SHIELD))
            {
                playerFieldCardList = Array.FindAll(playerFieldCardList, card => card.cardModel.ability == ABILITY.SHIELD);
            }
            CardController defender = playerFieldCardList[0];
            // attackerとdiffenderを戦わせる
            StartCoroutine(attacker.cardMovement.MoveToTarget(defender.transform));
            yield return new WaitForSeconds(1);
            gameManager.CardsBattle(attacker,defender);
            }
            else
            {
            StartCoroutine(attacker.cardMovement.MoveToTarget(gameManager.playerHero));
            yield return new WaitForSeconds(0.25f);
            gameManager.AttackToHero(attacker);
            yield return new WaitForSeconds(0.25f);
            gameManager.CheckHeroHP();
            }

            fieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();        
            yield return new WaitForSeconds(1);            
        }


        gameManager.ChangeTurn();
    }

    IEnumerator CastSpellOf(CardController card)
    {
        CardController target = null;
        Transform movePosition = null;
        switch(card.cardModel.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
                target = gameManager.GetEnemyFieldCards(card.cardModel.isPlayerCard)[0];
                movePosition = target.transform;
                break;
            case SPELL.HEAL_FRIEND_CARD:
                target = gameManager.GetFriendFieldCards(card.cardModel.isPlayerCard)[0];
                movePosition = target.transform;
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                movePosition = gameManager.playerFieldTransform;
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                movePosition = gameManager.enemyFieldTransform;
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                movePosition = gameManager.playerHero;
                break;
            case SPELL.HEAL_FRIEND_HERO:
                movePosition = gameManager.enemyHero;
                break;

        }
        // 移動先としてターゲット/それぞれのフィールド/それぞれのheroのtransformが必要
        StartCoroutine(card.cardMovement.MoveToField(movePosition));
        yield return new WaitForSeconds(0.25f);
        card.UseSpellTo(target);// カードを使用したら破壊する
    }


}
