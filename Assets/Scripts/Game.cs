using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameSettings;

public class Game : MonoBehaviour
{
    Camera mainCam;
    [SerializeField] Transform ringParent;

    const float ringHeight = 0.25f;
    const int numStacks = 3;

    List<Stack<GameObject>> stacks = new List<Stack<GameObject>>(numStacks);
    [SerializeField] BoxCollider[] stackInteractionAreas = new BoxCollider[numStacks];
    GameObject currentSelectedRing;
    int currentStackIndex, newStackIndex;
    Vector3 currentPosition, newPosition;

    void Awake()
    {
        mainCam = Camera.main;
        InitGame();
    }

    //set initial game state
    void InitGame()
    {
        for (int i = 0; i < stacks.Capacity; i++)
        {
            stacks.Add(new Stack<GameObject>(ringCount));
        }

        //loops through all children of ringParent in reverse order (bottom -> top in the hierarchy)
        for (int i = ringParent.childCount - 1; i >= 0; i--)
        {
            //adds <amount of rings equal to player's choice in main menu> to first stack
            if (i < ringCount)
            {
                stacks[0].Push(ringParent.GetChild(i).gameObject);
            }
            //disables the rest
            else
            {
                ringParent.GetChild(i).gameObject.SetActive(false);
            }
        }

        //moves rings down to the base of the game board
        foreach (GameObject go in stacks[0])
        {
            go.transform.Translate(Vector3.down * ringParent.localScale.y * ringHeight * (maxRings - ringCount), Space.World);
        }

        SetRingColour();
    }

    //changes ring colour based on ring count
    //i know Stack.ElementAt() defeats the purpose of a stack, ok D:<
    void SetRingColour()
    {
        for (int i = 0; i < ringCount; i++)
        {
            MeshRenderer mr = stacks[0].ElementAt(i).GetComponent<MeshRenderer>();
            mr.material = new Material(mr.material);
            mr.material.color = Color.LerpUnclamped(Color.white, Color.black, (float)i / ringCount);
        }
    }

    void Update()
    {
        //on mouse down
        if (Input.GetMouseButtonDown(0) && !gameOver)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            (bool, int) mouseDownData = GetStackIndex(ray);

            if (mouseDownData.Item1 && stacks[mouseDownData.Item2].Count > 0)
            {
                currentStackIndex = mouseDownData.Item2;
                currentSelectedRing = stacks[mouseDownData.Item2].Pop();

                //set original position
                currentPosition = currentSelectedRing.transform.position;
            }
        }

        //on mouse drag (sort of)
        if (currentSelectedRing != null)
        {
            //snap currentSelectedRing's position to cursor position
            currentSelectedRing.transform.position = mainCam.ScreenToWorldPoint(Input.mousePosition);
        }

        //on mouse up
        if (Input.GetMouseButtonUp(0) && !gameOver && currentSelectedRing != null)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            //update newStackIndex only if mouse button has been released on one of the stackInteractableAreas
            (bool, int) mouseUpData = GetStackIndex(ray);
            newStackIndex = mouseUpData.Item2;

            //if the move is valid
            if (mouseUpData.Item1 && IsValidMove(ray))
            {
                //set and update currentSelectedRing's new position
                newPosition.x = stackInteractionAreas[newStackIndex].transform.position.x;
                newPosition.y = stacks[newStackIndex].Count * ringHeight;
                currentSelectedRing.transform.localPosition = newPosition;

                //increment moveCount only if currentStackIndex and newStackIndex are different
                if (newStackIndex != currentStackIndex)
                {
                    moveCount++;
                }
            }
            else
            {
                //revert newStackIndex back to currentStackIndex
                newStackIndex = currentStackIndex;

                //reset position of currentSelectedRing
                currentSelectedRing.transform.position = currentPosition;
            }

            //push currentSelectedRing into stack with index that matches updated newStackIndex
            stacks[newStackIndex].Push(currentSelectedRing);

            //check if game is over (but only if a ring was placed in the last stack)
            if (newStackIndex == stacks.Capacity - 1)
            {
                gameOver = stacks[stacks.Capacity - 1].Count == ringCount;
            }

            currentSelectedRing = null;
        }
    }

    //return index of collider that was clicked (if any)
    (bool, int) GetStackIndex(Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray);

        for (int i = 0; i < stackInteractionAreas.Length; i++)
        {
            if (hits.Any(h => h.collider == stackInteractionAreas[i]))
            {
                return (true, i);
            }
        }

        return (false, -1);
    }

    //returns true if the selected ring can be placed on a chosen stack, according to Tower of Hanoi rules
    bool IsValidMove(Ray ray)
    {
        //if stack is empty, move is automatically valid
        if (stacks[newStackIndex].Count == 0)
        {
            return true;
        }
        //otherwise check if there is a bigger ring immediately below it
        else
        {
            GameObject topOfStack = stacks[newStackIndex].Peek();
            return currentSelectedRing.transform.GetSiblingIndex() < topOfStack.transform.GetSiblingIndex();
        }
    }
}