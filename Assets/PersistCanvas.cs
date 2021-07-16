using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistCanvas : SingletonMonoBehavior<PersistCanvas>
{
    public override string HierarchyPath => "PersistCanvas";
    public CanvasGroup blackScreen;

    new private void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        blackScreen = transform.Find("BlackScreen").GetComponent<CanvasGroup>();
    }
}
