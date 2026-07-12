using System.Text;
using System.Windows;
using System.Windows.Input;
using myCurrencyMagic.Client.ViewModels;

namespace myCurrencyMagic.Client;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private bool _isFormattingAmountText;

    public MainWindow(MainWindowViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        InitializeComponent();
        DataObject.AddPastingHandler(AmountTextBox, AmountTextBox_OnPaste);
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            return;
        }

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (_viewModel.ContainsOnlyAllowedAmountCharacters(e.Text))
        {
            return;
        }

        e.Handled = true;
        _viewModel.ShowInvalidCharacterMessage();
    }

    private void AmountTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (_isFormattingAmountText)
        {
            return;
        }

        _isFormattingAmountText = true;
        try
        {
            var result = _viewModel.FormatAmountText(AmountTextBox.Text, AmountTextBox.CaretIndex);
            if (AmountTextBox.Text != result.DisplayText)
            {
                AmountTextBox.Text = result.DisplayText;
            }

            AmountTextBox.CaretIndex = Math.Clamp(result.CaretIndex, 0, AmountTextBox.Text.Length);
            _viewModel.SetFormattedAmount(result);
        }
        finally
        {
            _isFormattingAmountText = false;
        }
    }

    private void AmountTextBox_OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (!e.DataObject.GetDataPresent(DataFormats.Text))
        {
            e.CancelCommand();
            return;
        }

        var text = e.DataObject.GetData(DataFormats.Text) as string;
        if (text is null)
        {
            e.CancelCommand();
            _viewModel.ShowInvalidCharacterMessage();
            return;
        }

        if (!_viewModel.ContainsOnlyAllowedAmountCharacters(text))
        {
            e.CancelCommand();
            _viewModel.ShowInvalidCharacterMessage();
            return;
        }

        if (!_viewModel.IsValidAmountPasteText(text))
        {
            e.CancelCommand();
            _viewModel.ShowInvalidFormatMessage();
        }
    }
}
