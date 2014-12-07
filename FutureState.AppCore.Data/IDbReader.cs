namespace FutureState.AppCore.Data
{
    public interface IDbReader
    {
        object this[int index] { get; }
        object this[string name] { get; }
        int Depth { get; }
        bool IsClosed { get; }
        bool IsDbNull(int ordinal);
        bool IsDbNull(string name);
        int RecordsAffected { get; }
        void Close();
        bool NextResult();
        bool Read();
    }
}