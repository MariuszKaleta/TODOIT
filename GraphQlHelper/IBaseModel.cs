namespace GraphQlHelper
{
    public interface IBaseElement : IBaseElement<int>
    {
        string Name { get; }
    }
    public interface IBaseElement<out TId>
    {
        string Name { get; }

        TId Id { get; }
    }
}
