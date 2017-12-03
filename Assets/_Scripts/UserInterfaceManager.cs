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

   public void Clear()
   {
      _numbersText.text = string.Empty;
      _targetText.text = string.Empty;
      _solutionText.text = string.Empty;
   }

   public void GenerateButtonPressed()
   {
      //todo - add interface to read in number of large numbers instead of random selection
      GameManager.Instance.NumbersController.PopulateNumbers();

      GameManager.Instance.NumbersController.GenerateSolution();
      PopulateNumbers();
      PopulateTarget();
      PopulateSolution();
      HideSolution();
      _timerController.ShowTimer();
      _timerController.ResetTimer();
   }

   public void ShowSolutionButtonPressed()
   {
      _timerController.ToggleTimer();
      ToggleSolution();
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
