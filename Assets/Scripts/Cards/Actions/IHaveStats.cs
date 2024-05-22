/// <summary>
/// Интерфейс для механик имеющих параметр, который могут изменять другие карты
/// </summary>
public interface IHaveStats
{
    /// <summary>
    /// Изменяемый параметр
    /// </summary>
    public int parameter { get; set; }
}
