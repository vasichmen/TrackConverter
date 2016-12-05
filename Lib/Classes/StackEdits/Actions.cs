using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public class MarkerMoveInfo : IUndoInfo
        {
            Action<TrackPoint, TrackPoint> undoAction;
            TrackPoint oldPoint;
            TrackPoint newPoint;

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
        public class MarkerEditInfo : IUndoInfo
        {
            Action<TrackPoint, TrackPoint> undoAction;
            TrackPoint oldPoint;
            TrackPoint newPoint;

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
        public class MarkerDeleteInfo : IUndoInfo
        {
            Action<TrackPoint> undoAction;
            TrackPoint oldPoint;

            /// <summary>
            /// создает новый экземпляр отмены действия с заданными данными для отмены
            /// </summary>
            /// <param name="oldPoint">новая информация от точке</param>
            /// <param name="undoAction">дейсвие для отмены изменения информации</param>
            public MarkerDeleteInfo(TrackPoint oldPoint, Action<TrackPoint> undoAction)
            {
                if (undoAction == null)
                    throw new ArgumentNullException("StackEdits.MarkerDeleteInfo: при создании пустые объекты недопустимы");
                this.undoAction = undoAction;
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
        public class MarkerAddInfo : IUndoInfo
        {
            Action<TrackPoint> undoAction;
            TrackPoint newPoint;


            /// <summary>
            /// создает новый экземпляр отмены действия с заданными данными для отмены
            /// </summary>
            /// <param name="newPoint">новая информация от точке</param>
            /// <param name="undoAction">дейсвие для отмены изменения информации</param>
            public MarkerAddInfo(TrackPoint newPoint, Action<TrackPoint> undoAction)
            {
                if (undoAction == null)
                    throw new ArgumentNullException("StackEdits.MarkerAddInfo: при создании пустые объекты недопустимы");
                this.undoAction = undoAction;
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
