using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get { return _instance = _instance ?? FindObjectOfType<GameManager>(); } }
   private static GameManager _instance;

   public NumbersController NumbersController { get { return _numbersController = _numbersController ?? FindObjectOfType<NumbersController>(); } }
   private NumbersController _numbersController;

   private void Start()
   {
      UserInterfaceManager.Instance.Clear();
      Screen.orientation = ScreenOrientation.Portrait;
   }
}
