using System.Windows.Input;

namespace k7proxyyoneticisi.gorunummodelleri
{
    public class komut : ICommand
    {
        private readonly Action<object?> _calistir;
        private readonly Func<object?, bool>? _yapilabilir;

        public komut(Action<object?> calistir, Func<object?, bool>? yapilabilir = null)
        {
            _calistir = calistir;
            _yapilabilir = yapilabilir;
        }

        public komut(Action calistir, Func<bool>? yapilabilir = null)
            : this(_ => calistir(), _ => yapilabilir?.Invoke() ?? true) { }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _yapilabilir?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _calistir(parameter);
    }
}