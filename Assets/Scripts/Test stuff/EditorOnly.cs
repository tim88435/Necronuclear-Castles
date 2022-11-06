using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For objects that should only appear in the editor
public class EditorOnly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        //if in editor, do nothing
#else
        gameObject.SetActive(false);//else, turn off this object
#endif
    }
}
