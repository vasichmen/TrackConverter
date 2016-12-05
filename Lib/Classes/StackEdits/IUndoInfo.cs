namespace TrackConverter.Lib.Classes.StackEdits
{
    /// <summary>
    /// интерфейс отмены действия
    /// </summary>
    public interface IUndoInfo
    {
        /// <summary>
        /// отменяет действие
        /// </summary>
        void Undo();
    }
}