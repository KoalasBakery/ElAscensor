using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class Fuse
{
    public Transform fuseStart, fuseEnd;
    public LineRenderer lineRend;
}

public class FusePuzzle : PuzzleBehaviour
{
    
    [SerializeField] List<Fuse> fuses= new List<Fuse>();
    Fuse currentFuse;
    [SerializeField]List<GameObject> puzzleItems= new List<GameObject>();
    Vector2 inputWorldPosition;
    int currentIndex;

    public override void Init(PuzzleData _newPuzzleData)
    {
        base.Init(_newPuzzleData);
        CreateFuses();
    }

    [ContextMenu("Create Fuses")]
    public void CreateFuses()
    {
        
        EraseFuses();
        
        //transform.position = Vector3.zero;
        
        FusePuzzleData fuseData= (FusePuzzleData)data;

        for (int i = 0; i < fuseData.fuses.Length; i++)
        {
            GameObject startPos = new GameObject();
            GameObject endPos = new GameObject();
            GameObject lineRend = Instantiate(fuseData.lineRendPrefab);

            Transform parentTransform = PuzzleManager.Instance.transform;

            startPos.transform.parent = parentTransform;
            endPos.transform.parent = parentTransform;
            lineRend.transform.parent = parentTransform;  


            startPos.name = $"StartPos_{i}";
            endPos.name = $"EndPos_{i}";
            lineRend.name = $"LineRend_{i}";

            puzzleItems.Add(startPos);
            puzzleItems.Add(endPos);
            puzzleItems.Add(lineRend);

            Color spriColor =new Color(
                fuseData.fuses[i].color.Evaluate(0).r, 
                fuseData.fuses[i].color.Evaluate(0).g, 
                fuseData.fuses[i].color.Evaluate(0).b, 
                1);

            LineRenderer lR = lineRend.GetComponent<LineRenderer>();

            lR.colorGradient = fuseData.fuses[i].color;
            lR.SetPosition(0, Vector3.zero);
            lR.SetPosition(1, Vector3.zero);


            SpriteRenderer sR = startPos.AddComponent<SpriteRenderer>();
            sR.sprite = fuseData.fuseSprite;
            sR.color = spriColor;

            sR = endPos.AddComponent<SpriteRenderer>();
            sR.sprite = fuseData.fuseSprite;
            fuseData.fuses[i].color.Evaluate(0);

            sR.color = spriColor;

            startPos.transform.position = new Vector3(
                fuseData.fuses[i].fuseStart.x * fuseData.spacing.x+ fuseData.offset.x, 
                fuseData.fuses[i].fuseStart.y * fuseData.spacing.y+ fuseData.offset.y, 0);
            endPos.transform.position = new Vector3(
                fuseData.fuses[i].fuseEnd.x * fuseData.spacing.x+ fuseData.offset.x,
                fuseData.fuses[i].fuseEnd.y * fuseData.spacing.y + fuseData.offset.y, 0);

            Fuse newFuse = new Fuse()
            {
                fuseStart = startPos.transform,
                fuseEnd = endPos.transform,
                lineRend = lR
            };
            fuses.Add(newFuse);
        }
    }
    [ContextMenu("Erase Fuses")]
    public void EraseFuses()
    {
        foreach (var item in puzzleItems)
        {
            DestroyImmediate(item);
        }
        puzzleItems.Clear();
        fuses.Clear();
    }
    
    public override void OnInteract(InputAction.CallbackContext context)
    {
        base.OnInteract(context);
        if (context.valueType !=typeof( Vector2)) return;


        Vector2 inputScreenPos = context.ReadValue<Vector2>();
        inputWorldPosition = Camera.main.ScreenToWorldPoint(inputScreenPos);

        if (currentFuse!=null)
        {
            currentFuse.lineRend.SetPosition(0, currentFuse.fuseStart.position);
            currentFuse.lineRend.SetPosition(1, inputWorldPosition);
            return;
        }
        for (int i = 0; i < fuses.Count; i++)
        {
            Fuse fuse = fuses[i];

            if (Vector2.Distance(inputWorldPosition, fuse.fuseStart.position) < 0.5f)
            {
                currentFuse = new Fuse()
                {
                    fuseStart = fuse.fuseStart,
                    fuseEnd = fuse.fuseEnd,
                    lineRend = fuse.lineRend,
                };
                currentIndex = i;
                return;
            }

            if (Vector2.Distance(inputWorldPosition, fuse.fuseEnd.position) < 0.5f)
            {
                currentFuse = new Fuse()
                {
                    fuseStart = fuse.fuseEnd,
                    fuseEnd = fuse.fuseStart,
                    lineRend = fuse.lineRend,
                };
                currentIndex = i;
                return;
            }
        }
    }


    public override void OnRelease()
    {
        base.OnRelease();
        if (currentFuse == null) return;

        if (Vector2.Distance(inputWorldPosition, currentFuse.fuseEnd.position) < 0.5f)
        {
            fuses.RemoveAt(currentIndex );
            currentFuse.lineRend.SetPosition(0, currentFuse.fuseStart.position);
            currentFuse.lineRend.SetPosition(1, currentFuse.fuseEnd.position);
        }
        else
        {
            currentFuse.lineRend.SetPosition(0, Vector2.zero);
            currentFuse.lineRend.SetPosition(1, Vector2.zero);
        }
        if (fuses.Count < 1)
            PuzzleComplete();
        
        currentFuse = null;
    }
}

