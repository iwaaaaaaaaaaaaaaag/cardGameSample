using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform defaultParent;
    public bool isDraggable;

    public void OnDrag(PointerEventData eventData)
    {
        if(!isDraggable)
        {
            return;
        }
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // カードのコストとplayerのManaコストを比較
        CardController card = GetComponent<CardController>();
        if (card.cardModel.isPlayerCard && GameManager.instance.isPlayerTurn && !card.cardModel.isFieldCard && card.cardModel.cost <= GameManager.instance.player.manaCost)
        {
            Debug.Log("hoge1");
            isDraggable = true;
        }
        else if (card.cardModel.isPlayerCard && GameManager.instance.isPlayerTurn && card.cardModel.isFieldCard && card.cardModel.canAttack)
        {
            Debug.Log("hoge2");
            isDraggable = true;
        }
        else
        {
            Debug.Log("hoge3");
            isDraggable = false;
        }

        if(!isDraggable)
        {
            return;
        }
        defaultParent = transform.parent;
        transform.SetParent(defaultParent.parent,false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!isDraggable)
        {
            return;
        }
        transform.SetParent(defaultParent,false);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }

    public IEnumerator MoveToField(Transform field)
    {
        // 一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        // Dotweenでカードをフィールドに移動
        transform.DOMove(field.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        defaultParent = field;
        transform.SetParent(defaultParent);

    }
    public IEnumerator MoveToTarget(Transform target)
    {
        // 現在の位置と並びを取得
        Vector3 currentPosition = transform.position;
        int siblingIndex = transform.GetSiblingIndex();

        // 一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        // Dotweenでカードをフィールドに移動
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);

        // 元の位置に戻る
        transform.DOMove(currentPosition, 0.25f);
        yield return new WaitForSeconds(0.25f);
        if (this != null)
        {
            transform.SetParent(defaultParent);
            transform.SetSiblingIndex(siblingIndex);
        }
    }
    
    void Start()
    {
        defaultParent = transform.parent;
    }
}

