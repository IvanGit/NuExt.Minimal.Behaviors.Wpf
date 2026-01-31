using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Minimal.Behaviors.Wpf
{
    /// <summary>
    /// Provides an abstract base class for behaviors that convert events to command executions.
    /// Implements the event-to-command pattern with support for parameter resolution and execution validation.
    /// </summary>
    /// <typeparam name="T">The type of the associated object, which must inherit from <see cref="DependencyObject"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This behavior resolves command parameters in the following order:
    /// 1. If <see cref="CommandParameter"/> is set, use it.
    /// 2. If <see cref="EventArgsConverter"/> is specified, use the converted value.
    /// 3. If <see cref="PassEventArgsToCommand"/> is <see langword="true"/>, use the event arguments.
    /// 4. Otherwise, pass <see langword="null"/>.
    /// </para>
    /// </remarks>
    public abstract class EventToCommandBehavior<T> : EventBehavior<T> where T : DependencyObject
    {
        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior<T>));

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            nameof(CommandParameter), typeof(object), typeof(EventToCommandBehavior<T>));

        /// <summary>
        /// Identifies the <see cref="EventArgsConverter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EventArgsConverterProperty = DependencyProperty.Register(
            nameof(EventArgsConverter), typeof(IValueConverter), typeof(EventToCommandBehavior<T>));

        /// <summary>
        /// Identifies the <see cref="PassEventArgsToCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PassEventArgsToCommandProperty = DependencyProperty.Register(
            nameof(PassEventArgsToCommand), typeof(bool), typeof(EventToCommandBehavior<T>));

        /// <summary>
        /// Identifies the <see cref="ProcessHandledEvent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProcessHandledEventProperty = DependencyProperty.Register(
            nameof(ProcessHandledEvent), typeof(bool), typeof(EventToCommandBehavior<T>));

        /// <summary>
        /// Identifies the <see cref="MarkEventHandled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetHandledProperty = DependencyProperty.Register(
            nameof(MarkEventHandled), typeof(bool), typeof(EventToCommandBehavior<T>));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the command to execute when the event is raised.
        /// </summary>
        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the command when it is executed.
        /// </summary>
        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets or sets the converter used to transform event arguments before passing them to the command.
        /// </summary>
        /// <value>
        /// An <see cref="IValueConverter"/> that transforms event arguments; otherwise, <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// This converter is invoked only when <see cref="CommandParameter"/> is not set.
        /// If the converter returns <see langword="null"/>, the command will receive <see langword="null"/> as parameter.
        /// </remarks>
        public IValueConverter? EventArgsConverter
        {
            get => (IValueConverter?)GetValue(EventArgsConverterProperty);
            set => SetValue(EventArgsConverterProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to pass the event arguments directly to the command
        /// when <see cref="CommandParameter"/> is not set.
        /// </summary>
        /// <value>
        /// <see langword="true"/> to pass event arguments directly; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// Default value is <see langword="false"/> - event arguments are not passed to the command
        /// unless explicitly configured.
        /// </remarks>
        public bool PassEventArgsToCommand
        {
            get => (bool)GetValue(PassEventArgsToCommandProperty);
            set => SetValue(PassEventArgsToCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this behavior should process routed events that have already been marked as handled.
        /// </summary>
        /// <value>
        /// <c>true</c> to process handled routed events; otherwise, <c>false</c>.
        /// </value>
        public bool ProcessHandledEvent
        {
            get => (bool)GetValue(ProcessHandledEventProperty);
            set => SetValue(ProcessHandledEventProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this behavior should mark the routed event as handled after command execution.
        /// </summary>
        /// <value>
        /// <c>true</c> to mark the routed event as handled; otherwise, <c>false</c>.
        /// </value>
        public bool MarkEventHandled
        {
            get => (bool)GetValue(SetHandledProperty);
            set => SetValue(SetHandledProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the command can be executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The event data.</param>
        /// <returns>true if the command can be executed; otherwise, false.</returns>
        protected virtual bool CanExecuteCommand(object? sender, object? eventArgs)
        {
            return IsEnabled && Command?.CanExecute(ResolveCommandParameter(sender, eventArgs)) == true;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The event data.</param>
        protected virtual void ExecuteCommand(object? sender, object? eventArgs)
        {
            if (eventArgs is RoutedEventArgs { Handled: true } && !ProcessHandledEvent)
            {
                return;
            }

            var command = Command;
            if (command == null || !CanExecuteCommand(sender, eventArgs))
            {
                return;
            }
            var commandParameter = ResolveCommandParameter(sender, eventArgs);
            command.Execute(commandParameter);

            if (eventArgs is RoutedEventArgs routedEventArgsAfterSync && MarkEventHandled)
            {
                routedEventArgsAfterSync.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when the associated event is raised. Ensures that the command is executed
        /// even if the binding has not yet been evaluated by using the dispatcher to delay execution.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The event data.</param>
        protected override void OnEvent(object? sender, object? eventArgs)
        {
            if (Command == null && BindingOperations.GetBindingExpression(this, CommandProperty) != null)
            {
                Dispatcher.InvokeAsync(() => {
                    ExecuteCommand(sender, eventArgs);
                }, DispatcherPriority.DataBind);
                return;
            }
            ExecuteCommand(sender, eventArgs);
        }

        /// <summary>
        /// Resolves the command parameter to use when executing the command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The event data.</param>
        /// <returns>The command parameter.</returns>
        protected virtual object? ResolveCommandParameter(object? sender, object? eventArgs)
        {
            var commandParameter = CommandParameter;
            if (commandParameter != null)
            {
                return commandParameter;
            }

            var converter = EventArgsConverter;
            if (converter != null)
            {
                return converter.Convert(eventArgs, typeof(object), sender, CultureInfo.CurrentCulture);
            }

            return PassEventArgsToCommand ? eventArgs : null;
        }

        #endregion
    }
}
