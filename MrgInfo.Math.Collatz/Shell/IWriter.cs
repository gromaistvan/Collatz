namespace MrgInfo.Math.Collatz.Shell
{
    interface IWriter
    {
        void Print(SequenceRecord record);

        void Print(CollatzSequence sequence);

        bool Active { get; }
    }
}
