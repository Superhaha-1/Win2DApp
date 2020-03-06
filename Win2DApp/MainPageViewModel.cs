using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using Windows.UI;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Win2DApp
{
    public sealed class MainPageViewModel : ReactiveObject
    {
        public MainPageViewModel()
        {
            _root = new CustomRectangle(100f, 100f, 45f, 100f, 200f);
            _renderDictionary.Add(_root.RenderOption, _root);
            _focus = _root;
            MoveUpCommand = ReactiveCommand.Create(MoveUp);
            MoveLeftCommand = ReactiveCommand.Create(MoveLeft);
            RotateCommand = ReactiveCommand.Create(Rotate);
            MoveRightCommand = ReactiveCommand.Create(MoveRight);
            MoveDownCommand = ReactiveCommand.Create(MoveDown);
            AddChildCommand = ReactiveCommand.Create(AddChild);
            RemoveChildCommand = ReactiveCommand.Create(RemoveChild);
            BrowseUpCommand = ReactiveCommand.Create(BrowseUp);
        }

        private CustomShapeBase _root;

        private readonly Dictionary<RenderOption, CustomShapeBase> _renderDictionary = new Dictionary<RenderOption, CustomShapeBase>();

        public CustomShapeBase Root
        {
            get
            {
                return _root;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _root, value);
            }
        }

        private CustomShapeBase _focus;

        public ReactiveCommand<Unit, Unit> MoveUpCommand { get; }

        public ReactiveCommand<Unit, Unit> MoveLeftCommand { get; }

        public ReactiveCommand<Unit, Unit> RotateCommand { get; }

        public ReactiveCommand<Unit, Unit> MoveRightCommand { get; }

        public ReactiveCommand<Unit, Unit> MoveDownCommand { get; }

        public ReactiveCommand<Unit, Unit> AddChildCommand { get; }

        public ReactiveCommand<Unit, Unit> RemoveChildCommand { get; }

        public ReactiveCommand<Unit, Unit> BrowseUpCommand { get; }

        private void MoveUp()
        {
            _focus.Move(0f, -1f);
        }
        private void MoveLeft()
        {
            _focus.Move(-1f, 0f);
        }
        private void Rotate()
        {
            _focus.Rotate(1);
        }
        private void MoveRight()
        {
            _focus.Move(1f, 0f);
        }
        private void MoveDown()
        {
            _focus.Move(0f, 1f);
        }

        private void AddChild()
        {
            int max = 100;
            CustomShapeBase child = null;
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < max; j++)
                {
                   child = new CustomRectangle(i, j, 0, 10, 10);
                    _focus.AddChild(child);
                }
            }
            _focus = child;
        }

        private void RemoveChild()
        {
            var parent = _focus.Parent;
            if (parent == null)
            {
                _focus.RemoveAll();
                return;
            }
            parent.RemoveChild(_focus);
            _focus = parent;
        }

        private void BrowseUp()
        {
            var parent = _focus.Parent;
            if(parent != null)
            {
                _focus = parent;
            }
        }
    }

    public sealed class RenderOptionManager
    {
        public RenderOptionManager()
        {

        }

        private readonly Dictionary<(float, Color), int> _renderOptionDictionary = new Dictionary<(float, Color), int>();

        private readonly Dictionary<int, (float, Color)> _renderIndexDictionary = new Dictionary<int, (float, Color)>();

        public int GetRenderIndex(float strokeWidth, Color color)
        {
            var option = (strokeWidth, color);
            if (!_renderOptionDictionary.TryGetValue(option, out var index))
            {
                index = _renderOptionDictionary.Count;
                _renderOptionDictionary.Add(option, index);
                _renderIndexDictionary.Add(index, option);
            }
            return index;
        }

        public int GetRenderIndex(int renderIndex, float strokeWidth)
        {
            var option = _renderIndexDictionary[renderIndex];
            if(option.Item1 == strokeWidth)
            {
                return renderIndex;
            }
            return GetRenderIndex(strokeWidth, option.Item2);
        }
    }
}
