using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglyLinkedList
{
    public Tile head;
    Tile last;


    public int Count
    { get; private set; }

    public void InsertFrontOfLine(GameObject new_gameObjectData)
    {
        Tile new_Node = new Tile(new_gameObjectData); 
        new_Node.next = head; 
        head = new_Node; 
        Count++;
    }


    public void InsertLast( GameObject new_gameObjectData)
    {
        Tile new_Node = new Tile(new_gameObjectData); 
        if(head != null) 
        {
            Tile lastNode = GetLastNode();  
            lastNode.next = new_Node; 
        }
        else
        {
            head = new_Node; 
        }
        Count++;
    }

    public Tile GetLastNode()
    {
        Tile temp = head; 
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


    public void InsertAfter(Tile prevNode, GameObject new_gameObjectData)
    {
        if(prevNode == null) 
        {
            Debug.Log("The given previous node CANNOT be null"); 
        }
        Tile new_Node = new Tile(new_gameObjectData); 
        new_Node.next = prevNode.next; 
        prevNode.next = new_Node; 
        Count++;
    }

}
