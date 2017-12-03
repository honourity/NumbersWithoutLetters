using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
   [SerializeField]
   private RectTransform _hand;
   [SerializeField]
   private float _timerLength = 30f;
   [SerializeField]
   private float _fullRevolutionTime = 60f;
   [SerializeField]
   float timeLeftStartColourChange = 10f;
   [SerializeField]
   float timeLeftSolidColour = 1f;

   private Image _faceSprite;
   private float _timerSeconds;
   private bool _runTimer;

   public void ShowTimer()
   {
      gameObject.SetActive(true);
   }

   public void HideTimer()
   {
      gameObject.SetActive(false);
   }

   public void ResetTimer()
   {
      _timerSeconds = 0f;
      _runTimer = true;
      StartCoroutine(TimerColourShift());
   }

   private void Update()
   {
      if (_runTimer)
      {
         _timerSeconds += Time.deltaTime;
         if (_timerSeconds > _timerLength)
         {
            _runTimer = false;
         }
         else
         {
            _hand.localRotation = Quaternion.Euler(_hand.localRotation.eulerAngles.x, _hand.localRotation.eulerAngles.y, -360f * (_timerSeconds / _fullRevolutionTime));
         }
      }
   }

   private void Awake()
   {
      _faceSprite = GetComponent<Image>();
   }

   private IEnumerator TimerColourShift()
   {
      float colourIntensity = 1f;
      var timeLeft = _timerLength - _timerSeconds;

      _faceSprite.color = new Color(_faceSprite.color.r, colourIntensity, colourIntensity);

      while (_runTimer
         && timeLeft > timeLeftSolidColour
         && _timerSeconds < _timerLength)
      {
         timeLeft = _timerLength - _timerSeconds;

         if (timeLeft < timeLeftStartColourChange)
         {
            //move the colour back and forth based on time past and some tweaked scaling
            var modifiedTime = (_timerSeconds * (1 / timeLeft)) + 1f;
            colourIntensity = Mathf.PingPong(modifiedTime, 1f);

            //apply colour
            _faceSprite.color = new Color(_faceSprite.color.r, colourIntensity, colourIntensity);
         }

         yield return new WaitForEndOfFrame();
      }

      _faceSprite.color = new Color(_faceSprite.color.r, 0f, 0f);
      yield return null;
   }
}
