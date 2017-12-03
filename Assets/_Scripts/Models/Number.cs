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