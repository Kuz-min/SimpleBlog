namespace SimpleBlog.ViewModels;

public class ListViewModel<T>
{
    public int offset { get; set; }
    public int length { get; set; }
    public T[] items { get; set; } = default!;
}
