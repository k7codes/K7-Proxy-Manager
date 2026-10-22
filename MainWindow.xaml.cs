using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using k7proxyyoneticisi.gorunummodelleri;
using k7proxyyoneticisi.modeller;

namespace k7proxyyoneticisi
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new anamodel();
        }

        private void OnMinimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void OnMaximize(object sender, RoutedEventArgs e) =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        private void OnClose(object sender, RoutedEventArgs e) => Close();

        private void Kopyala_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.Parent is ContextMenu cm && cm.PlacementTarget is DataGrid dg && dg.SelectedItem is modeller.proxy p)
            {
                Clipboard.SetText(p.adres);
            }
        }

        private void Sec_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.Parent is ContextMenu cm && cm.PlacementTarget is DataGrid dg && dg.SelectedItem is modeller.proxy p)
            {
                p.secili = true;
            }
        }

        private void CalisanlariKopyala_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as anamodel;
            if (vm == null) return;
            var calisanlar = vm.proxyler.Where(p => p.calisiyor).Select(p => p.adres);
            Clipboard.SetText(string.Join(Environment.NewLine, calisanlar));
        }

        private void AnaTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cp = AnaTabs.Template.FindName("PART_SelectedContentHost", AnaTabs) as ContentPresenter;
            if (cp == null) return;
            if (cp.RenderTransform is not TranslateTransform)
                cp.RenderTransform = new TranslateTransform(0, 10);
            cp.Opacity = 0;
            var sb = new Storyboard();
            var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(260))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(fade, cp);
            Storyboard.SetTargetProperty(fade, new PropertyPath(UIElement.OpacityProperty));
            sb.Children.Add(fade);

            var slide = new DoubleAnimation(10, 0, TimeSpan.FromMilliseconds(260))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(slide, cp);
            Storyboard.SetTargetProperty(slide, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(slide);

            sb.Begin();
        }
    }
}