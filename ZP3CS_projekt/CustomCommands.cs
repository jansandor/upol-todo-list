using System.Windows.Input;

namespace ZP3CS_projekt.Commands
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand RemoveTaskCommand = new RoutedUICommand
            (
                "Removes Selected Task",
                "RemoveTaskCommand",
                typeof(CustomCommands)
            );
        public static readonly RoutedUICommand MarkTaskAsFinishedCommand = new RoutedUICommand
            (
                "Marks Selected Todo Task as Finished",
                "MarkTaskAsFinishedCommand",
                typeof(CustomCommands)
            );
        public static readonly RoutedUICommand EditTaskCommand = new RoutedUICommand
            (
                "Opens Task Editation Dialog",
                "EditTaskCommand",
                typeof(CustomCommands)
            );
        public static readonly RoutedUICommand TaskProgress_DecrCommand = new RoutedUICommand
            (
                "Decrements Task Progress Value",
                "TaskProgress_DecrCommand",
                typeof(CustomCommands)
            );
        public static readonly RoutedUICommand TaskProgress_IncrCommand = new RoutedUICommand
            (
                "Increments Task Progress Value",
                "TaskProgress_IncrCommand",
                typeof(CustomCommands)
            );
    }
}
