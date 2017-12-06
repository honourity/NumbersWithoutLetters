using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NumbersController : MonoBehaviour
{
   [SerializeField]
   private int _maxLargeNumbers = 4;
   [SerializeField]
   private int _attemptsChunkSize = 100000;
   [SerializeField]
   private int _numberCount = 6;
   [SerializeField]
   private int _maxTargetBase = 999;

   private System.Random _random;
   private double _maxTarget;
   private double _minTarget;
   private IList<double> _numbers;
   private double _target;
   private IOperable _solution;

   public IList<double> PopulateNumbers()
   {
      //remember random end of range is exclusive not inclusive
      return PopulateNumbers(_random.Next(0, _maxLargeNumbers));
   }

   public IList<double> PopulateNumbers(int largeCount)
   {
      if (largeCount > _maxLargeNumbers || largeCount < 0)
      {
         largeCount = _random.Next(0, _maxLargeNumbers+1);
      }

      _maxTarget = _maxTargetBase;
      _minTarget = Math.Min(100 * (largeCount + 1), _maxTarget);

      _numbers.Clear();

      //adding large numbers
      for (var i = 0; i < largeCount; i++)
      {
         var index = _random.Next(0, 4);
         switch (index)
         {
            case 0:
               _numbers.Add(25);
               break;
            case 1:
               _numbers.Add(50);
               break;
            case 2:
               _numbers.Add(75);
               break;
            case 3:
               _numbers.Add(100);
               break;
         }
      }

      //adding small numbers
      var small = _numberCount - largeCount;
      for (var i = 0; i < small; i++)
      {
         var number = _random.Next(1, 11);
         _numbers.Add(number);
      }

      return _numbers;
   }

   public IList<double> PopulateNumbers(double number1, double number2, double number3, double number4, double number5, double number6, double target)
   {
      _minTarget = _maxTarget = target;
      _numbers.Clear();
      _numbers.Add(number1);
      _numbers.Add(number2);
      _numbers.Add(number3);
      _numbers.Add(number4);
      _numbers.Add(number5);
      _numbers.Add(number6);

      return _numbers;
   }

   public void GenerateSolution()
   {
      var attempts = 1;
      var attemptChunks = 0;
      var maxTarget = _maxTarget;
      var minTarget = _minTarget;

      var operables = GetFreshOperables(_numbers);

      while (operables.Count > 1 || (operables.FirstOrDefault().Value < minTarget || operables.FirstOrDefault().Value > maxTarget))
      {
         var successfulOperation = false;

         var firstIndex = 0;
         var secondIndex = 0;
         IOperable operable = null;
         IOperable firstOperable = null;
         IOperable secondOperable = null;

         while (!successfulOperation)
         {
            //get 2 different elements out of the existing list
            firstIndex = _random.Next(0, operables.Count);
            secondIndex = _random.Next(0, operables.Count);
            while (secondIndex == firstIndex) { secondIndex = _random.Next(0, operables.Count); }
            firstOperable = operables.ElementAt(firstIndex);
            secondOperable = operables.ElementAt(secondIndex);

            //choose a an operation and make sure its legal
            var operation = (Enums.OperationType)_random.Next(1, 5);
            operable = new Operation(firstOperable, operation, secondOperable);

            //check against ruleset for allowed operations
            var operableValue = operable.Value;
            if (
               (operableValue != 0.0)
               && (operableValue % 1 == 0.0)
               && !(operableValue < 0.0)
               && !(operable.Type == Enums.OperationType.Multiply && operable.FirstNumber.Value == 1)
               && !(operable.Type == Enums.OperationType.Multiply && operable.SecondNumber.Value == 1)
               && !(operable.Type == Enums.OperationType.Divide && operable.SecondNumber.Value == 1)
               && !(operable.Type == Enums.OperationType.Divide && operable.FirstNumber.Value == 1)
               )
            {
               successfulOperation = true;
            }
         }

         //remove elements from list
         if (firstOperable != null) operables.Remove(firstOperable);
         if (secondOperable != null) operables.Remove(secondOperable);

         //inject our new operation back into the operables list
         operables.Add(operable);

         //if there's only 1 operable in the list, we have used up the numbers and failed, so try again
         //OR if any operables have hit the max size result, try again
         if ((operables.Count < 2) && (operables.FirstOrDefault().Value < minTarget || operables.FirstOrDefault().Value > maxTarget))
         {
            operables = GetFreshOperables(_numbers);
            attempts++;
         }

         //we probably have a number list, and target range which has no solution
         // so increase target range upper limit a little bit and keep trying
         if (attempts > _attemptsChunkSize)
         {
            maxTarget += _maxTarget;
            minTarget -= Math.Max(_minTarget / 2, 1);

            attempts = 0;
            attemptChunks++;
         }
      }

      _solution = operables.First(o => o.Value >= minTarget);

      //Console.WriteLine("(" + (attempts + (attemptChunks * attemptsChunkSize)) + " attempts)");

      _target = _solution.Value;
   }

   public IList<double> GetNumbers()
   {
      return _numbers;
   }

   public double GetTarget()
   {
      return _target;
   }

   public StringBuilder GetSolution(IOperable operable = null, StringBuilder stringBuilder = null)
   {
      if (operable == null) operable = _solution;
      if (stringBuilder == null) stringBuilder = new StringBuilder();

      if (operable.Type != Enums.OperationType.None)
      {
         //do recursive on children
         GetSolution(operable.FirstNumber, stringBuilder);
         GetSolution(operable.SecondNumber, stringBuilder);

         //handle current operable
         stringBuilder.Append(operable.FirstNumber.Value);

         switch (operable.Type)
         {
            case Enums.OperationType.Add:
               stringBuilder.Append(" + ");
               break;
            case Enums.OperationType.Subtract:
               stringBuilder.Append(" - ");
               break;
            case Enums.OperationType.Multiply:
               stringBuilder.Append(" * ");
               break;
            case Enums.OperationType.Divide:
               stringBuilder.Append(" / ");
               break;
         }

         stringBuilder.AppendFormat("{0} = {1}\n", operable.SecondNumber.Value, operable.Value);
      }

      return stringBuilder;
   }

   private void Awake()
   {
      _numbers = new List<double>();
      _random = new System.Random();
   }

   private IList<IOperable> GetFreshOperables(IList<double> numbers)
   {
      _numbers = numbers;

      var operables = new List<IOperable>();
      foreach (var number in numbers)
      {
         operables.Add(new Number(number));
      }

      return operables;
   }
}
