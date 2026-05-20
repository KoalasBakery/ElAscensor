using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class FusePuzzle : PuzzleBehaviour
{
    
    List<Fuse> fuses= new List<Fuse>();
    Fuse currentFuse;
    Vector2 inputWorldPosition;
    int currentIndex;

    public override void Init(PuzzleData _newPuzzleData, PuzzleManager _manager)
    {
        base.Init(_newPuzzleData, _manager);

        currentFuse = null;
        EraseFuses();

        FusePuzzleData fuseData = (FusePuzzleData)_newPuzzleData;

        Fuse[] allFuses= _manager.fusePuzzleHolder.GetComponentsInChildren<Fuse>();

        for (int i = 0; i < fuseData.fuses.Length; i++)
        {
            if (i >= allFuses.Length)
                return;

            Fuse _tempFuse = allFuses[i];

            _tempFuse.ActiveFuse();
            
            Color spriColor = new Color(
            fuseData.fuses[i].color.Evaluate(0).r,
            fuseData.fuses[i].color.Evaluate(0).g,
            fuseData.fuses[i].color.Evaluate(0).b,
            1);

            _tempFuse.fuseStartSprite.sprite = fuseData.fuses[i].sprite;
            _tempFuse.fuseStartSprite.color = spriColor;

            _tempFuse.fuseEndSprite.sprite = fuseData.fuses[i].sprite;
            _tempFuse.fuseEndSprite.color = spriColor;

            _tempFuse.lineRend.colorGradient = fuseData.fuses[i].color;
            _tempFuse.lineRend.SetPosition(0, Vector3.zero);
            _tempFuse.lineRend.SetPosition(1, Vector3.zero);

            _tempFuse.fuseStart.position = new Vector3(
                fuseData.fuses[i].fuseStart.x * fuseData.spacing.x + fuseData.offset.x,
                fuseData.fuses[i].fuseStart.y * fuseData.spacing.y + fuseData.offset.y, 0);
            _tempFuse.fuseEnd.position = new Vector3(
                fuseData.fuses[i].fuseEnd.x * fuseData.spacing.x + fuseData.offset.x,
                fuseData.fuses[i].fuseEnd.y * fuseData.spacing.y + fuseData.offset.y, 0);

            fuses.Add(_tempFuse);

        }
    }
  
    public void EraseFuses()
    {
        foreach (var item in fuses)
        {
            if (item == null) continue;

            item.DisableFuse();
        }
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
                currentFuse = fuse;

                currentIndex = i;
                return;
            }

            if (Vector2.Distance(inputWorldPosition, fuse.fuseEnd.position) < 0.5f)
            {
                Transform _tempTransform;
                currentFuse = fuse;
                _tempTransform = currentFuse.fuseStart;
                currentFuse.fuseStart = currentFuse.fuseEnd;
                currentFuse.fuseEnd = _tempTransform;
              
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

