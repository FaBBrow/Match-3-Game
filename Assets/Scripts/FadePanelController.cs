using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    [SerializeField] private Animator panelAnim;
    [SerializeField] private Animator gameInfoAnim;

    public void OK()
    {
        panelAnim.SetBool("Out", true);
        gameInfoAnim.SetBool("Out", true);
    }
}