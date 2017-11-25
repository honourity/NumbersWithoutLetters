using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Program
{
   public static void Main(string[] args)
   {
      var controller = new NumbersController();

      Console.WriteLine("Press a key to generate a combination...");
      Console.ReadKey(true);

      //create large and small numbers
      foreach (var number in controller.GetNumbers())
      {
         Console.Write(number.ToString() + " ");
      }
      Console.Write("\b");
      Console.WriteLine();

      Console.WriteLine("Target to reach: " + controller.GetTarget());

      Console.WriteLine("Press a key to see the answer...");
      Console.ReadKey(true);

      Console.WriteLine(controller.GetSolution(null, null).ToString());
      Console.WriteLine("Press a key to exit.");
      Console.ReadKey(true);
   }
}

public class NumbersController
{
   private Random _random;

   //todo - add user choosing large and small numbers
   //todo - create some sort of algorithm for deciding the _minTarget based on how many large and small
   // maybe a timeout for calculating _minTarget, which automatically reduces it
   private int _numberCount = 6;
   private int _minTarget = 100;
   private int _maxTarget = 999;
   private IList<double> _numbers;
   private IOperable _solution;

   public NumbersController()
   {
      _random = new Random();
      _numbers = new List<double>();
   }

   public IList<double> GetNumbers()
   {
      //sample code adding all small numbers
      //todo - replace this with input from user getting how many large and how many small
      for (var i = 0; i < _numberCount; i++)
      {
         var number = _random.Next(1, 10);
         _numbers.Add(number);
      }

      return _numbers;
   }

   public double GetTarget()
   {
      var attempts = 0;

      var operables = GetFreshOperables();

      while (!operables.Any(o => o.Value > _minTarget) && operables.All(o => o.Value <= _maxTarget))
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

            var operableValue = operable.Value;
            if ((operableValue != 0.0) && (operableValue % 1 == 0.0))
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
         if (operables.Count < 2)
         {
            operables = GetFreshOperables();
            attempts++;
         }
      }

      _solution = operables.First(o => o.Value > _minTarget);

      return _solution.Value;
   }

   private IList<IOperable> GetFreshOperables()
   {
      var operables = new List<IOperable>();
      foreach (var number in _numbers)
      {
         operables.Add(new Number(number));
      }

      return operables;
   }

   public StringBuilder GetSolution(IOperable operable, StringBuilder stringBuilder)
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
}

public interface IOperable
{
   double Value { get; }

   IOperable FirstNumber { get; }
   Enums.OperationType Type { get; }
   IOperable SecondNumber { get; }
}

public class Enums
{
   public enum OperationType
   {
      None = 0,
      Add = 1,
      Subtract = 2,
      Multiply = 3,
      Divide = 4
   }
}

public class Operation : IOperable
{
   public double Value
   {
      get
      {
         switch (Type)
         {
            case Enums.OperationType.Add: return FirstNumber.Value + SecondNumber.Value;
            case Enums.OperationType.Subtract: return FirstNumber.Value - SecondNumber.Value;
            case Enums.OperationType.Multiply: return FirstNumber.Value * SecondNumber.Value;
            case Enums.OperationType.Divide: return FirstNumber.Value / SecondNumber.Value;
            default: return -1;
         }
      }
   }

   public IOperable FirstNumber { get; private set; }
   public Enums.OperationType Type { get; private set; }
   public IOperable SecondNumber { get; private set; }

   public Operation(IOperable firstNumber, Enums.OperationType type, IOperable secondNumber)
   {
      Type = type;
      FirstNumber = firstNumber;
      SecondNumber = secondNumber;
   }
}

public class Number : IOperable
{
   public double Value { get; private set; }

   public IOperable FirstNumber { get; private set; }
   public Enums.OperationType Type { get; private set; }
   public IOperable SecondNumber { get; private set; }

   public Number(double value)
   {
      Value = value;
      Type = Enums.OperationType.None;
   }
}
