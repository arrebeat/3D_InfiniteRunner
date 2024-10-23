using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

public class Typer : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float delayBetweenTypes = .1f;

    public string textToType;

    private Coroutine _coroutine;

    void Start()
    {
        textMesh.text = "";
    }

    [Button]
    public void StartTyping()
    {
        textMesh.text = "";
        
        if (_coroutine != null)
        {
            StopAllCoroutines();
            _coroutine = null;
            _coroutine = StartCoroutine(TypeHandler(textToType));
        }
        else
        {
            _coroutine = StartCoroutine(TypeHandler(textToType));
        }
    }

    private IEnumerator TypeHandler(string text)
    {
        textMesh.text = "";
        foreach (var l in text.ToCharArray())
        {
            textMesh.text += l;
            yield return new WaitForSeconds(delayBetweenTypes);
        }

        yield return null;
    }
}
