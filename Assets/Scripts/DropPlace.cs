using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlace : MonoBehaviour, IDropHandler
{
    public enum TYPE
    {
        HAND,
        FIELD,
    }
    public TYPE type;
    public void OnDrop(PointerEventData eventData)
    {
        if(type == TYPE.HAND)
        {
            return;
        }
        //ドラッグしているオブジェクトを入手
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if (card != null)
        {
            

            if(!card.cardMovement.isDraggable)
            {
                return;
            }

            if(card.IsSpell)
            {
                return;
            }
            //fieldの位置情報をカードに設定
            card.cardMovement.defaultParent = this.transform;

            if(card.cardModel.isFieldCard)
            {
                return;
            }
        card.OnFiled();
        }
    }

}
