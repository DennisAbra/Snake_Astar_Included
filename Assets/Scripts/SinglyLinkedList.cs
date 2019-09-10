using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglyLinkedList
{
    public Tile head;
    Tile last;
    private int iteratorPos;


    public int Count
    { get; private set; }

    public void InsertFrontOfLine(GameObject new_gameObjectData)
    {
        Tile new_Node = new Tile(new_gameObjectData); // skapar en ny nod till listan
        new_Node.next = head; // den nya noden tränger sig längst fram genom att skapa en koppling till huvudet av listan
        head = new_Node; // visar att den nya noden numera är längst fram
        Count++;
    }


    public void InsertLast( GameObject new_gameObjectData)
    {
        Tile new_Node = new Tile(new_gameObjectData); // skapar en ny nod till listan
        if(head != null) // har vi ett huvud?
        {
            Tile lastNode = GetLastNode();  // Hitta den sista noden i listan
            lastNode.next = new_Node; // sätt dess högra nod till den nya noden
        }
        else
        {
            head = new_Node; // finns inte ett huvud är nya noden huvudet
        }
        Count++;
    }

    public Tile GetLastNode()
    {
        Tile temp = head; // hämta en tillfällig ref till huvudet för att räkna till sista platsen i listan
        while (temp.next != null) // när temp.next är null är temp den sista noden i listan
        {
            temp = temp.next;
        }
        return temp;
    }


    public Tile RemoveLast() // Kallas från Snake scriptet
    {
        return RemoveLast(head, Count); // Kallar vår interna RemoveLast
    }


    Tile RemoveLast(Tile node, int nodesLeft)
    {
        if (nodesLeft == 1) // Kollar hur många noder vi har kvar, om detta stämmar har vi hittat den näst sista noden och gör dess nästa nod till null. På så sätt kapas den sista noden ur vår lista
        {
            Tile temp = last;
            last = node;
            node.next = null;
            Count--;
            return temp;
        }
        else return RemoveLast(node.next, nodesLeft - 1); // Minksa antalet noder kvar, och kalla sig själv igen. Detta upprepas tills vi uppfyllt vårt IF statement
    }


    public void InsertAfter(Tile prevNode, GameObject new_gameObjectData)
    {
        if(prevNode == null) //NullRefEx protection
        {
            Debug.Log("The given previous node CANNOT be null"); 
        }
        Tile new_Node = new Tile(new_gameObjectData); // skapar ny nod till listan
        new_Node.next = prevNode.next; // nya noden glider in bakom den förra noden
        prevNode.next = new_Node; // den noden som kommer efter förra noden är numer vår nya nod
        Count++;
    }

}
