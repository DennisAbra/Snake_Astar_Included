using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglyLinkedList : IEnumerable, IEnumerator
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
        while (temp.next != null)
        {
            temp = temp.next;
        }
        return temp;
    }

    public Tile RemoveLast()
    {
        return RemoveLast(head, Count);
    }

    Tile RemoveLast(Tile node, int nodesLeft)
    {
        if (nodesLeft == 1)
        {
            Tile temp = last;
            last = node;
            node.next = null;
            Count--;
            return temp;
        }
        else return RemoveLast(node.next, nodesLeft - 1);
    }
    //Todo: Get all nodes positions and make them impassable


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

    public GameObject this[int index] // Overloading array
    {
        get => GetValue(index); // Expression body. Used instead of {} && return
        set => SetValue(index, value);
    }

    public GameObject GetValue(int index)
    {
        if (index < 0 || index >= Count || head == null)
        {  Debug.Log("Index out of range"); } 
        Tile currenNode = head;
        for (int i = 0; i <= index && currenNode != null; i++)
        {
            if (index == i)
            {
                return currenNode.gameObjectdata;
            }
            currenNode = currenNode.next;
        }
        return null;
    }
    public void SetValue(int index, GameObject value)
    {
        if (index < 0 || index >= Count || head == null)
        { Debug.Log("Index out of range"); } 
        Tile currenNode = head;
        for (int i = 0; i <= index && currenNode != null; i++)
        {
            if (index == i)
            {
                currenNode.gameObjectdata = value;
                return;
            }
            currenNode = currenNode.next;
        }
    }

    public object Current => GetValue(iteratorPos);

    public bool MoveNext()
    {
        iteratorPos++;
        return iteratorPos < Count;
    }

    public void Reset()
    {
        iteratorPos = 0;
    }

    public IEnumerator GetEnumerator()
    {
        return this;
    }
}
