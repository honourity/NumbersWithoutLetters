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
