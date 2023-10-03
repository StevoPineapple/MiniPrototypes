using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSplash : MonoBehaviour
{
    bool hasWon;
    DartsManager dartsMan;
    // Start is called before the first frame update
    void Start()
    {
        if (dartsMan == null)
        {
            dartsMan = GameObject.Find(GameFamilyNames.DartsObject).GetComponent<DartsManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DartsSplash"))
        {
            if (!hasWon)
            {
                hasWon = true;
                dartsMan.SetWin();
            }
            other.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DartsSplash"))
        {
            other.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}

