public interface IOperable
{
   double Value { get; }
   IOperable FirstNumber { get; }
   Enums.OperationType Type { get; }
   IOperable SecondNumber { get; }
}
