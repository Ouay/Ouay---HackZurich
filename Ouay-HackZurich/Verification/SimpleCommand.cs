using System;
using System.Windows.Input;

namespace Ouay_HackZurich.Verification
{
	class SimpleCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public SimpleCommand(Action action, bool canExecute = true)
		{
			this._action = action;
			this._canExecute = canExecute;
		}
		public void Enable(bool enable = true)
		{
			this._canExecute = enable;
			this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
		public bool CanExecute(object parameter)
		{
			return (this._canExecute);
		}
		public void Execute(object parameter)
		{
			if (this._action != null)
			{
				this._action();
			}
		}
		bool _canExecute;
		Action _action;
	}
}
