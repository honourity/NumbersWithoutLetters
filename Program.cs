using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Program
{
   public static void Main(string[] args)
   {
      var controller = new NumbersController();

      while (true)
      {
         Run(controller);
      }
   }

   public static void Run(NumbersController controller)
   {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("[generate]");
      Console.ForegroundColor = ConsoleColor.White;

      Console.ReadKey(true);
      
      Console.SetCursorPosition(0, Console.CursorTop - 1);

      Console.Write("Numbers: ");

      //create large and small numbers
      //todo - player chooses how many large
      foreach (var number in controller.GetNumbersCheat(50,25,7,5,6,1,345))
      {
         Console.Write(number.ToString() + " ");
      }
      Console.Write("\b");
      Console.WriteLine();

      Console.WriteLine("Target:  " + controller.GetTarget());

      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("[show working]");
      Console.ForegroundColor = ConsoleColor.DarkGray;

      Console.ReadKey(true);
      Console.SetCursorPosition(0, Console.CursorTop - 1);
      Console.WriteLine(controller.GetSolution(null, null).ToString().TrimEnd('\n') + "\n");
   }
}

public class NumbersController
{
   private Random _random;

   private const int _attemptsChunkSize = 100000;
   private const int _numberCount = 6;
   private const int _maxTargetBase = 999;
   private const int _minTargetBase = 100;
   private int _maxTarget;
   private int _minTarget;
   private IList<double> _numbers;
   private IOperable _solution;

   public NumbersController()
   {
      _random = new Random();
      _numbers = new List<double>();
   }

   public IList<double> GetNumbers()
   {
      return GetNumbers(_random.Next(0, 7));
   }

   public IList<double> GetNumbers(int largeCount)
   {
      if (largeCount < 0) largeCount = _random.Next(0,7);

      _maxTarget = _maxTargetBase;
      _minTarget = 100 * (largeCount + 1);

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

   public IList<double> GetNumbersCheat(int a, int b, int c, int d, int e, int f, int target)
   {
      _minTarget = _maxTarget = target;
      _numbers.Clear();
      _numbers.Add(a);
      _numbers.Add(b);
      _numbers.Add(c);
      _numbers.Add(d);
      _numbers.Add(e);
      _numbers.Add(f);

      return _numbers;
   }

   public double GetTarget()
   {
      var attempts = 1;
      var attemptChunks = 0;
      var maxTarget = _maxTarget;
      var minTarget = _minTarget;

      var operables = GetFreshOperables();

      while (!operables.Any(o => o.Value >= minTarget))
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
            if (
               (operableValue != 0.0)
               && (operableValue % 1 == 0.0)
               && !(operable.Type == Enums.OperationType.Multiply && operable.FirstNumber.Value == 1)
               && !(operable.Type == Enums.OperationType.Multiply && operable.SecondNumber.Value == 1)
               && !(operable.Type == Enums.OperationType.Divide && operable.SecondNumber.Value == 1)
               && !(operable.Type == Enums.OperationType.Divide && operable.FirstNumber.Value == 1))
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
         if ((operables.Count < 2) || (operables.Any(o => o.Value > maxTarget)))
         {
            operables = GetFreshOperables();
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

         stringBuilder.AppendFormat("{0} = {1}     \n", operable.SecondNumber.Value, operable.Value);
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
