using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardView cardView; //見かけに関することを操作
    public CardModel cardModel;//データ(model)に関することを操作
    public CardMovement cardMovement;//移動(move)に関することを操作

    GameManager gameManager;

    public bool IsSpell
    {
        get { return cardModel.spell != SPELL.NONE;}
    }
private void Awake(){
    cardView = GetComponent<CardView>();
    cardMovement = GetComponent<CardMovement>();
    gameManager = GameManager.instance;
}
public void Init(int cardID, bool isPlayer)
{
    cardModel = new CardModel(cardID, isPlayer);
    cardView.SetCard(cardModel);

}


public void CheckAlive ()
{
    if(cardModel.isAlive)
    {
        RefreshView();
    }else
    {
        Destroy(this.gameObject);
    }
}

public void Attack(CardController enemyCard)
{
    cardModel.Attack(enemyCard);
    SetCanAttack(false);
}

public void Heal(CardController friendCard)
{
    cardModel.Heal(friendCard);
    friendCard.RefreshView();
}

public void RefreshView()
{
    cardView.Refresh(cardModel);
}

public void SetCanAttack(bool canAttack)
{
    cardModel.canAttack = canAttack;
    cardView.SetActiveSelectablePanel(canAttack);    
}


public void OnFiled()
{
        gameManager.ReducdeManaCost(cardModel.cost,cardModel.isPlayerCard);
        cardModel.isFieldCard = true;
        if(cardModel.ability == ABILITY.INIT_ATTACKABLE)
        {
            SetCanAttack(true);
        }
}

/*
敵がいないのに攻撃しようとしている=> 敵AIのチェックと同様にすればよい
*/

public void UseSpellTo(CardController target)
{
    switch (cardModel.spell)
    {
        case SPELL.DAMAGE_ENEMY_CARD:
            // 特定の敵を攻撃する
            if (target == null)
            {
                return;
            }
            if (target.cardModel.isPlayerCard == cardModel.isPlayerCard)
            {
                return;
            }
            Attack(target); 
            target.CheckAlive();
            break;
        case SPELL.DAMAGE_ENEMY_CARDS:
            // 相手フィールドの全てのカードに攻撃する
            CardController[] enemyCards = gameManager.GetEnemyFieldCards(this.cardModel.isPlayerCard);
            foreach (CardController enemyCard in enemyCards)
            {
                Attack(enemyCard); 
            }
            foreach (CardController enemyCard in enemyCards)
            {
                enemyCard.CheckAlive();
            }
            break;
        case SPELL.DAMAGE_ENEMY_HERO:
            // 敵ヒーローを攻撃する
            gameManager.AttackToHero(this);
            break;
        case SPELL.HEAL_FRIEND_CARD:
            if (target == null)
            {
                return;
            }
            if (target.cardModel.isPlayerCard != cardModel.isPlayerCard)
            {
                return;
            }
            Heal(target);
            break;
        case SPELL.HEAL_FRIEND_CARDS:
            // 相手フィールドの全てのカードに攻撃する
            CardController[] friendCards = gameManager.GetFriendFieldCards(this.cardModel.isPlayerCard);
            foreach (CardController friendCard in friendCards)
            {
                Heal(friendCard); 
            }
            break;
        case SPELL.HEAL_FRIEND_HERO:
            gameManager.HealToHero(this);
            break;
        case SPELL.NONE:
            return;  
    }
    gameManager.ReducdeManaCost(cardModel.cost,cardModel.isPlayerCard);
    Destroy(this.gameObject); 
}

public bool CanUseSpell()
{
    switch (cardModel.spell)
    {
        case SPELL.DAMAGE_ENEMY_CARD:
        case SPELL.DAMAGE_ENEMY_CARDS:
            // 相手フィールドの全てのカードに攻撃する
            CardController[] enemyCards = gameManager.GetEnemyFieldCards(this.cardModel.isPlayerCard);
            if(enemyCards.Length > 0 && cardModel.cost <= GameManager.instance.player.manaCost)
            {
                return true;
            }
            return false;
        case SPELL.DAMAGE_ENEMY_HERO:
        case SPELL.HEAL_FRIEND_HERO:
            return true;
        case SPELL.HEAL_FRIEND_CARD:
        case SPELL.HEAL_FRIEND_CARDS:
            // 相手フィールドの全てのカードに攻撃する
            CardController[] friendCards = gameManager.GetFriendFieldCards(this.cardModel.isPlayerCard);
            if(friendCards.Length > 0 && cardModel.cost <= GameManager.instance.player.manaCost)
            {
                return true;
            }
            return false;
        case SPELL.NONE:
            return false;
    }
    return false;
}

public void Show()
{
    cardView.Show();
}

}