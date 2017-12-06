using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
   public static UserInterfaceManager Instance { get { return _instance = _instance ?? FindObjectOfType<UserInterfaceManager>(); } }
   private static UserInterfaceManager _instance;

   [SerializeField]
   private Text _numbersText;
   [SerializeField]
   private Text _targetText;
   [SerializeField]
   private Text _solutionText;
   [SerializeField]
   private TimerController _timerController;

   private int _largeCountChosen = -1;

   public void Clear()
   {
      _numbersText.text = string.Empty;
      _targetText.text = string.Empty;
      _solutionText.text = string.Empty;
      _largeCountChosen = -1;
   }

   public void GenerateButtonPressed()
   {
      //hard one
      //GameManager.Instance.NumbersController.PopulateNumbers(75,25,50,100,8,2,431);

      GameManager.Instance.NumbersController.PopulateNumbers(_largeCountChosen);

      GameManager.Instance.NumbersController.GenerateSolution();
      PopulateNumbers();
      PopulateTarget();
      PopulateSolution();
      HideSolution();
      _timerController.ShowTimer();
      _timerController.ResetTimer();


      //todo - remove this
      ShowSolution();
   }

   public void ShowSolutionButtonPressed()
   {
      //_timerController.ToggleTimer();
      ToggleSolution();
   }

   public void SetLargeNumbersCount(int largeCount)
   {
      _largeCountChosen = largeCount;
   }

   private void ToggleSolution()
   {
      _solutionText.gameObject.SetActive(!_solutionText.gameObject.activeSelf);
   }

   private void HideSolution()
   {
      _solutionText.gameObject.SetActive(false);
   }

   private void ShowSolution()
   {
      _solutionText.gameObject.SetActive(true);
   }

   private void PopulateNumbers()
   {
      var numbers = GameManager.Instance.NumbersController.GetNumbers();

      var numberStringBuilder = new StringBuilder();
      foreach (var number in numbers)
      {
         numberStringBuilder.Append(number);
         numberStringBuilder.Append(" ");
      }

      _numbersText.text = numberStringBuilder.ToString();
   }

   private void PopulateTarget()
   {
      _targetText.text = GameManager.Instance.NumbersController.GetTarget().ToString();
   }

   private void PopulateSolution()
   {
      _solutionText.text = GameManager.Instance.NumbersController.GetSolution().ToString();
   }
}
