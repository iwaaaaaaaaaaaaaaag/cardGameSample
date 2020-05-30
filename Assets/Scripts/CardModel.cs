using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カードデータとその処理
public class CardModel
{
    public string name;
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;
    public ABILITY ability;
    public SPELL spell;
    public bool isAlive;
    public bool canAttack;
    public bool isFieldCard;
    public bool isPlayerCard;


    //コンストラクタを明示的に指定
    public CardModel(int cardID, bool isPlayer){
        CardEntity cardEntity = Resources.Load<CardEntity>("CardEntityList/Card" + cardID);
        this.name = cardEntity.name;
        this.hp   = cardEntity.hp;
        this.at   = cardEntity.at;
        this.cost = cardEntity.cost;
        this.icon = cardEntity.icon;
        this.ability = cardEntity.ability;
        this.spell = cardEntity.spell;
        this.isAlive = true;
        this.isPlayerCard = isPlayer;
        this.canAttack = false;
    }

    void Damage(int dmg){
        this.hp -= dmg;
        if(this.hp <= 0){
            this.hp = 0;
            isAlive = false;
        }
    }

    // 自分を回復する
    void RecoveryHP(int point)
    {
        hp += point;
    }
    
    public void Attack(CardController card)
    {
        card.cardModel.Damage(this.at);
    }

    // cardを回復させる
    public void Heal(CardController card)
    {
        card.cardModel.RecoveryHP(at);
    }
}
