using System.Collections;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public static FadePanelController instance;
    [SerializeField] private Animator panelAnim;
    [SerializeField] private Animator gameInfoAnim;

    private void Start()
    {
        instance = this;
    }

    public void OK()
    {
        panelAnim.SetBool("Out", true);
        gameInfoAnim.SetBool("Out", true);
        StartCoroutine(GameStartCo());
    }

    public void GameOver()
    {
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("gameover", true);
    }

    public IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1f);
        Board.instance.CurrentState = GameState.move;
    }
}