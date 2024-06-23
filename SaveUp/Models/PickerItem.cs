namespace SaveUp.Models
{
    /// <summary>
    /// Class to use for more complex picker scenarios where you want to display a different text than the value you want to use
    /// </summary>
    /// <typeparam name="T">The type of the background value</typeparam>
    public class PickerItem<T>
    {
        public string DisplayText { get; set; }
        public T BackgroundValue { get; set; }
    }
}
