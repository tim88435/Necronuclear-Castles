using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For objects that should only appear in the editor
public class EditorOnly : MonoBehaviour
{
    private bool inEditor = false;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        inEditor = true;
#endif
        gameObject.SetActive(inEditor);
    }
}
