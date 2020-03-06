using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Win2DApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page, IViewFor<MainPageViewModel>
    {
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainPageViewModel();
            this.WhenActivated(d =>
            {
                ViewModel.WhenAnyValue(vm => vm.Root).Subscribe(root => _root = root).DisposeWith(d);
                Observable.Timer(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(100), RxApp.TaskpoolScheduler).Where(l => _root != null && _root.HasNewGeometry()).Subscribe(l => CanvasControl_Main.Invalidate()).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.MoveUpCommand, v => v.Button_Up).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.MoveDownCommand, v => v.Button_Down).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.MoveLeftCommand, v => v.Button_Left).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.MoveRightCommand, v => v.Button_Right).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RotateCommand, v => v.Button_Rotate).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddChildCommand, v => v.Button_Add).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RemoveChildCommand, v => v.Button_Remove).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.BrowseUpCommand, v => v.Button_BrowseUp).DisposeWith(d);
            });
        }

        #region IViewFor<MainPageViewModel>

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
        .Register(nameof(ViewModel), typeof(MainPageViewModel), typeof(MainPage), new PropertyMetadata(null));

        public MainPageViewModel ViewModel
        {
            get => (MainPageViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainPageViewModel)value;
        }

        #endregion

        private CustomShapeBase _root;

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CanvasControl_Main.RemoveFromVisualTree();
            CanvasControl_Main = null;
        }

        private void CanvasControl_Main_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var geometry = _root?.GetGeometry(sender);
            if(geometry == null)
            {
                return;
            }
            args.DrawingSession.DrawGeometry(geometry, Colors.Black, 1f);
        }
    }
}
