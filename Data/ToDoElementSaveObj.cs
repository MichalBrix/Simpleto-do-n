using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;


namespace TodoLists.Data
{
    public class ToDoElementSaveObj
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsFinished { get; set; }
        public int OpenElementsNo { get; set; }
        public int InProgressElementsNo { get; set; }
        public int FinishedElementsNo { get; set; }
        public IEnumerable<ToDoElementSaveObj> ToDoElements { get; set; }
        public DateTime? DateStarted { get; set; }
        public DateTime? DateFinished { get; set; }

        public ToDoElementSaveObj()
        {
            ToDoElements = new List<ToDoElementSaveObj>();
        }

        public ToDoElementSaveObj(ToDoElement toDoElement)
        {
            ID = toDoElement.ID.ToString();
            Description = toDoElement.Description;
            IsInProgress = toDoElement.IsInProgress;
            IsFinished = toDoElement.IsFinished;
            OpenElementsNo = toDoElement.OpenElementsNo;
            InProgressElementsNo = toDoElement.InProgressElementsNo;
            FinishedElementsNo = toDoElement.FinishedElementsNo;
            DateFinished = toDoElement.DateFinished;
            DateStarted = toDoElement.DateStarted;
            ToDoElements = toDoElement.Children.Select(x=> new ToDoElementSaveObj(x));
            
        }
    }
}
