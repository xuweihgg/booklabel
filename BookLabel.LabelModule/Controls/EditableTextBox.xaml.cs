using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookLabel.LabelModule.Controls
{
    /// <summary>
    /// EditableTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class EditableTextBox : UserControl
    {
        public EditableTextBox()
        {
            InitializeComponent();
            base.Focusable = true;
            base.FocusVisualStyle = null;
        }
        #region Members Varables
        private string oldText;

        #region Properties
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBox), new PropertyMetadata(""));

        public string TextFormat
        {
            get { return (string)GetValue(TextFormatProperty); }
            set { SetValue(TextFormatProperty, value); }
        }
        public static DependencyProperty TextFormatProperty = DependencyProperty.Register("TextFormat", typeof(string), typeof(EditableTextBox), new PropertyMetadata("{0}"));
        #endregion
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(
            "IsEditable",
            typeof(bool),
            typeof(EditableTextBox),
            new PropertyMetadata(false));
        public bool IsInEditMode
        {
            get
            {
                if (IsEditable)
                    return (bool)GetValue(IsInEditModeProperty);
                else
                    return false;
            }
            set
            {
                if (IsEditable)
                {
                    if (value) oldText = Text;
                    SetValue(IsInEditModeProperty, value);
                }
            }
        }
        public static readonly DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(EditableTextBox),
            new PropertyMetadata(false));

        public static readonly RoutedEvent OnTextModifyCompleteEvent = EventManager.RegisterRoutedEvent("OnTextModifyComplete", RoutingStrategy.Bubble, typeof(ValueChanged),
            typeof(EditableTextBox));
        public event ValueChanged OnTextModifyComplete
        {
            add
            {
                AddHandler(OnTextModifyCompleteEvent, value);
            }
            remove
            {
                RemoveHandler(OnTextModifyCompleteEvent, value);
            }
        }

        private bool RaiseOnOnTextModifyCompleteEvent(object o, bool isTrue)
        {
            LostFocusEventArgs m = new LostFocusEventArgs(EditableTextBox.OnTextModifyCompleteEvent, this);
            m.IsTrue = isTrue;
            m.NewValue = o;
            RaiseEvent(m);
            if (m != null)
                return m.IsTrue;
            else
                return false;
        }
        #endregion

        public delegate void ValueChanged(object sender, LostFocusEventArgs e);

        public ICommand CompletedCommand
        {
            get { return (ICommand)GetValue(CompletedCommandProperty); }
            set { SetValue(CompletedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompletedCommandProperty =
            DependencyProperty.Register("CompletedCommand", typeof(ICommand), typeof(EditableTextBox), new PropertyMetadata(default(ICommand)));

        public object CompletedCommandParameter
        {
            get { return (object)GetValue(CompletedCommandParameterProperty); }
            set { SetValue(CompletedCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompletedCommandParameterProperty =
            DependencyProperty.Register("CompletedCommandParameter", typeof(object), typeof(EditableTextBox), new PropertyMetadata(default(object)));

        public IInputElement CommandTarget { get; set; }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                if (tb != null)
                {
                    tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    TextModifyComplete(tb);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                TextBox tb = sender as TextBox;
                if (tb != null)
                {
                    tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    TextModifyComplete(tb);
                }
                Text = oldText;
                e.Handled = true;
            }
        }

        private bool TextModifyComplete(TextBox tb)
        {
            bool result = true;
            if (this.CompletedCommand != null)
            {
                this.CompletedCommand.Execute(Text);

                if (RaiseOnOnTextModifyCompleteEvent(Text, true) == false)
                {
                    result = false;
                    IsInEditMode = true;
                    if (tb != null)
                        tb.Focus();
                }
                else
                {
                    IsInEditMode = false;
                }
            }
            return result;
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;

            // Give the TextBox input focus
            txt.Focus();

            txt.SelectAll();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
                TextModifyComplete(tb);
        }
    }

    public class LostFocusEventArgs:RoutedEventArgs
    {
        private object newValue;
        public object NewValue
        {
            get
            {
                return newValue;
            }
            set
            {
                newValue = value;
            }
        }
        private bool isTrue;
        /// <summary>
        /// 是否满足条件，可以继续执行失去焦点的操作。
        /// </summary>
        public bool IsTrue
        {
            get
            {
                return isTrue;
            }
            set
            {
                isTrue = value;
            }
        }

        public LostFocusEventArgs() : base() { }

        public LostFocusEventArgs(RoutedEvent e) : base(e) { }

        public LostFocusEventArgs(RoutedEvent e, object o): base(e, o) { }
    }
}
