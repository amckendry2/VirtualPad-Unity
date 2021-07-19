using UnityEngine;

public class ControllerDisplay : MonoBehaviour
{
    public GameObject stickSprite;
    public GameObject bSprite;
    public GameObject aSprite;

    public void UpdateController(InputState input){
        aSprite.GetComponent<SpriteRenderer>().color = input.APressed ? Color.red : Color.white;
        bSprite.GetComponent<SpriteRenderer>().color = input.BPressed ? Color.blue : Color.white;
        stickSprite.GetComponent<SpriteRenderer>().color = input.StickPressed ? Color.yellow : Color.black;
            stickSprite.transform.eulerAngles = new Vector3(
                stickSprite.transform.eulerAngles.x,
                stickSprite.transform.eulerAngles.y,
                input.StickDir + 90
            );
    }
}
