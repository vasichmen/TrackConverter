using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackConverter.Lib.Classes.StackEdits
{
    /// <summary>
    /// информация об изменении на карте для отмены изменения
    /// </summary>
    public class StackItem
    {
        /// <summary>
        /// создает элемент стека с заданной информацией
        /// </summary>
        public StackItem(IUndoInfo Data)
        {
            this.Data = Data;
        }

        /// <summary>
        /// тип действия
        /// </summary>
        public EditInfoType Type
        {
            get
            {
                Type type = Data.GetType();
                switch (type.Name)
                {
                    case "MarkerMoveInfo": return EditInfoType.MarkerMove;
                    case "MarkerEditInfo": return EditInfoType.MarkerEdit;
                    case "MarkerAddInfo": return EditInfoType.MarkerAdd;
                    case "MarkerDeleteInfo": return EditInfoType.MarkerDelete;
                    default: throw new NotSupportedException("Classes.StackEdits.StackItem.Type: не поддерживаемый тип действия " + type.Name);
                }
            }
        }

        /// <summary>
        /// описание действия
        /// </summary>
        public string Text
        {
            get
            {
                switch (this.Type)
                {
                    case EditInfoType.MarkerAdd: return "добавление маркера";
                    case EditInfoType.MarkerEdit: return "изменение информации";
                    case EditInfoType.MarkerMove: return "перемещение маркера";
                    case EditInfoType.MarkerDelete: return "удаление маркера";
                    default: throw new NotSupportedException("Classes.StackEdits.StackItem.Text: не поддерживаемое описание действия " + this.Type);
                }
            }
        }

        /// <summary>
        /// информация о действии
        /// </summary>
        public IUndoInfo Data { get; set; }

    }
}
