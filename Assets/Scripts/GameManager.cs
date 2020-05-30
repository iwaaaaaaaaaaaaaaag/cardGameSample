/*
ソースがどのオブジェクト、または別ソースに使用されているか一目でわかりたい
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GamePlayerManager player;
    public GamePlayerManager enemy;
    [SerializeField] AI enemyAI;
    [SerializeField] UImanager uiManager;

    //手札にカードを生成
    [SerializeField] CardController cardPrefab;
    //手札を生成する場所を指定する必要があるため、親オブジェクトをtransform
    public Transform playerHandTransform,playerFieldTransform,enemyHandTransform,enemyFieldTransform;
    
    public bool isPlayerTurn;

    public Transform playerHero;
    public Transform enemyHero;
    
    int timeCount;


    // シングルトン化(どこからでもアクセスできるようにする)
    public static GameManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start(){
        StartGame();
    }
    void StartGame()
    {
        player.Init(new List<int>() {1,2,4,3});
        enemy.Init(new List<int>() {1,2,4,3,3,3});
        uiManager.HideResultPanel();
        uiManager.ShowHeroHP(player.heroHp,enemy.heroHp);
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
    }

    public void ReducdeManaCost(int cost, bool isPlayerCard)
    {
        if(isPlayerCard)
        {
            player.manaCost -= cost;
        }
        else
        {
            enemy.manaCost -= cost;
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
    }
    IEnumerator CountDown()
    {
        timeCount = 20;
        uiManager.UpdateTime(timeCount);
        while(timeCount > 0)
        {
                yield return new WaitForSeconds(1); //1秒待機
                timeCount--;
                uiManager.UpdateTime(timeCount);
        }
        ChangeTurn();
    }
    public void Restart()
    {
        // handとfieldのカードを削除
        foreach(Transform card in playerHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach(Transform card in playerFieldTransform)
        {
            Destroy(card.gameObject);
        }
        foreach(Transform card in enemyHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach(Transform card in enemyFieldTransform)
        {
            Destroy(card.gameObject);
        }



        // デッキを生成
        player.deck = new List<int>(){4,1,2,2,3};
        enemy.deck = new List<int>(){4,1,2,1,3};

        StartGame();
    }
    void SettingInitHand(){
        //カードをそれぞれに3枚配る
        for(int i=0;i<3;i++){
            GiveCardToHand(player.deck, playerHandTransform);
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
    }
    void GiveCardToHand(List<int> deck,Transform hand)
    { 
        if(deck.Count == 0)
        {
            return;
        }
        int cardID = deck[0];
        deck.RemoveAt(0);
        CreateCard(cardID, hand);
    }
     void CreateCard(int cardID, Transform hand){
        //インスタンスを生成
        //生成するオブジェクト、どこのオブジェクトの中に生成するか、相対位置
        CardController card = Instantiate(cardPrefab, hand, false);
        if(hand.name == "PlayerHand")
        {
            card.Init(cardID, true);
        } 
        else
        {
            card.Init(cardID, false);
        }

    }

    void TurnCalc(){
       StopAllCoroutines();
       StartCoroutine(CountDown());
       if(isPlayerTurn)
        {
            //プレイヤーの処理を記載
            PlayTurn();   
        }else{
            //敵の処理
            StartCoroutine(enemyAI.EnemyTurn());  
        }
    }

    public void SettingCanAttackView(CardController[] FieldCardList,bool canAttack)
    {
        foreach (CardController playerCard in FieldCardList)
        {
            //cardを攻撃可能にする
            playerCard.SetCanAttack(canAttack);
        }

    }

    void PlayTurn()
    {
        Debug.Log("Playerのターン");
        // フィールドのカードを攻撃可能にする
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList,true);
    }

    public void CardsBattle(CardController attacker,CardController defender){
        Debug.Log("CardBattle");
        Debug.Log("attacker HP:"+attacker.cardModel.hp);
        Debug.Log("defender HP:"+defender.cardModel.hp);
        attacker.Attack(defender);
        defender.Attack(attacker);
        Debug.Log("attacker HP:"+attacker.cardModel.hp);
        Debug.Log("defender HP:"+defender.cardModel.hp);
        attacker.CheckAlive();
        defender.CheckAlive();
        

    }
    public CardController[] GetEnemyFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();

        }
    } 
    public CardController[] GetFriendFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>(); 
        }
    } 



    public void OnClickEndButton()
    {
        if(isPlayerTurn)
        {
            ChangeTurn();
        }
    }

    public void ChangeTurn(){
        isPlayerTurn = !isPlayerTurn;

        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList,false);
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList,false);


        if(isPlayerTurn)
        {
            player.IncreaseManaCost();
            GiveCardToHand(player.deck, playerHandTransform);
        }
        else
        { 
            enemy.IncreaseManaCost();
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        TurnCalc();
    }

    public void AttackToHero(CardController attacker)
    {
        if(attacker.cardModel.isPlayerCard)
        {
            enemy.heroHp -= attacker.cardModel.at; 
        }
        else
        {
            player.heroHp -= attacker.cardModel.at;
        }
        attacker.SetCanAttack(false);
        uiManager.ShowHeroHP(player.heroHp,enemy.heroHp);

    }
    public void HealToHero(CardController healer)
    {
        if(healer.cardModel.isPlayerCard)
        {
            player.heroHp += healer.cardModel.at; 
        }
        else
        {
            enemy.heroHp += healer.cardModel.at;
        }
        uiManager.ShowHeroHP(player.heroHp,enemy.heroHp);

    }


    public void CheckHeroHP()
    {
        if(player.heroHp <= 0 || enemy.heroHp <= 0)
        {
            ShowResultPanel(player.heroHp);
        }
        
    }
    void ShowResultPanel(int heroHp)
    {
        StopAllCoroutines();
        uiManager.ShowResultPanel(heroHp);        
    }   
}
