using System;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Classes.StackEdits
{
    /// <summary>
    /// действия, которые можно отменить
    /// </summary>
    public class Actions
    {
        /// <summary>
        /// информация о последнем перемещении маркера на карте
        /// </summary>
        public class MarkerMoveInfo: IUndoInfo
        {
            private Action<TrackPoint, TrackPoint> undoAction;
            private readonly TrackPoint oldPoint;
            private readonly TrackPoint newPoint;

            /// <summary>
            /// создает новый экземпляр отмены действия с заданными данными для отмены
            /// </summary>
            /// <param name="oldPoint">старое значение точки</param>
            /// <param name="newPoint">новое значение точки</param>
            /// <param name="undoAction">действие для отмены изменения</param>
            public MarkerMoveInfo(TrackPoint newPoint, TrackPoint oldPoint, Action<TrackPoint, TrackPoint> undoAction)
            {
                if (undoAction == null || oldPoint == null)
                    throw new ArgumentNullException("StackEdits.MarkerEditInfo: при создании пустые объекты недопустимы");
                this.undoAction = undoAction;
                this.oldPoint = oldPoint;
                this.newPoint = newPoint;
            }

            /// <summary>
            /// отменяет перемещение маркера
            /// </summary>
            public void Undo()
            {
                undoAction.Invoke(newPoint, oldPoint);
            }
        }

        /// <summary>
        /// информация о б изменении информации точки
        /// </summary>
        public class MarkerEditInfo: IUndoInfo
        {
            private Action<TrackPoint, TrackPoint> undoAction;
            private readonly TrackPoint oldPoint;
            private readonly TrackPoint newPoint;

            /// <summary>
            /// создает новый экземпляр отмены действия с заданными данными для отмены
            /// </summary>
            /// <param name="oldPoint">старая информация от точке</param>
            /// <param name="newPoint">новая информация от точке</param>
            /// <param name="undoAction">дейсвие для отмены изменения информации</param>
            public MarkerEditInfo(TrackPoint newPoint, TrackPoint oldPoint, Action<TrackPoint, TrackPoint> undoAction)
            {
                if (undoAction == null || oldPoint == null)
                    throw new ArgumentNullException("StackEdits.MarkerEditInfo: при создании пустые объекты недопустимы");
                this.undoAction = undoAction;
                this.oldPoint = oldPoint;
                this.newPoint = newPoint;
            }

            /// <summary>
            /// выполнение отмены изменения информации
            /// </summary>
            public void Undo()
            {
                undoAction.Invoke(newPoint, oldPoint);
            }
        }


        /// <summary>
        /// отмена удаления маркера
        /// </summary>
        public class MarkerDeleteInfo: IUndoInfo
        {
            private Action<TrackPoint> undoAction;
            private readonly TrackPoint oldPoint;

            /// <summary>
            /// создает новый экземпляр отмены действия с заданными данными для отмены
            /// </summary>
            /// <param name="oldPoint">новая информация от точке</param>
            /// <param name="undoAction">дейсвие для отмены изменения информации</param>
            public MarkerDeleteInfo(TrackPoint oldPoint, Action<TrackPoint> undoAction)
            {
                this.undoAction = undoAction ?? throw new ArgumentNullException("StackEdits.MarkerDeleteInfo: при создании пустые объекты недопустимы");
                this.oldPoint = oldPoint;
            }

            /// <summary>
            /// выполненение отмены удаления
            /// </summary>
            public void Undo()
            {
                undoAction.Invoke(oldPoint);
            }
        }


        /// <summary>
        /// отмена добавления маркера
        /// </summary>
        public class MarkerAddInfo: IUndoInfo
        {
            private Action<TrackPoint> undoAction;
            private readonly TrackPoint newPoint;


            /// <summary>
            /// создает новый экземпляр отмены действия с заданными данными для отмены
            /// </summary>
            /// <param name="newPoint">новая информация от точке</param>
            /// <param name="undoAction">дейсвие для отмены изменения информации</param>
            public MarkerAddInfo(TrackPoint newPoint, Action<TrackPoint> undoAction)
            {
                this.undoAction = undoAction ?? throw new ArgumentNullException("StackEdits.MarkerAddInfo: при создании пустые объекты недопустимы");
                this.newPoint = newPoint;
            }


            /// <summary>
            /// выполнение отмены добавления
            /// </summary>
            public void Undo()
            {
                undoAction.Invoke(newPoint);
            }
        }

    }
}
