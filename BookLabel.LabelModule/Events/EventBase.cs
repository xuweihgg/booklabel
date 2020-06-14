using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookLabel.LabelModule
{
    public class EventBase<BehaviorT> : Attachment<FrameworkElement, BehaviorT, EventBase<BehaviorT>>
        where BehaviorT : CommandBehaviorBase<FrameworkElement>
    {
        private static readonly DependencyProperty BehaviorProperty = Behavior();
        public static readonly DependencyProperty CommandProperty = Command(BehaviorProperty);
        public static readonly DependencyProperty CommandParameterProperty = Parameter(BehaviorProperty);

        public static void SetCommand(FrameworkElement control, ICommand command) { control.SetValue(CommandProperty, command); }
        public static ICommand GetCommand(FrameworkElement control) { return control.GetValue(CommandProperty) as ICommand; }
        public static void SetCommandParameter(FrameworkElement control, object parameter) { control.SetValue(CommandParameterProperty, parameter); }
        public static object GetCommandParameter(FrameworkElement buttonBase) { return buttonBase.GetValue(CommandParameterProperty); }
    }
}
