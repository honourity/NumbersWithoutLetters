using System;
using UnityEngine;

public class TimerController : MonoBehaviour
{
   [SerializeField]
   private RectTransform _hand;
   [SerializeField]
   private float _timerLength = 30f;

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
            _hand.localRotation = Quaternion.Euler(_hand.localRotation.eulerAngles.x, _hand.localRotation.eulerAngles.y, -360f * (_timerSeconds / 60f));
         }
      }
   }
}
