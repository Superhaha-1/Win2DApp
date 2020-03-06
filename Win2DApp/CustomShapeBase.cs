using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace Win2DApp
{
    public abstract class CustomShapeBase/* : IDisposable*/
    {
        protected CustomShapeBase(float x, float y, float angle)
        {
            _x = x;
            _y = y;
            _angle = angle;
            Volatile.Write(ref _children, Array.Empty<CustomRectangle>());
        }

        public RenderOption RenderOption { get; set; } = RenderOption.Default;

        private CustomShapeBase _parent;

        public CustomShapeBase Parent => Volatile.Read(ref _parent);

        private CustomShapeBase[] _children;

        private float _x;

        private float _y;

        private float _angle;

        private CanvasGeometry _geometry;

        private CanvasGeometry _selfGeometry;

        private CanvasGeometry _groupGeometry;

        private Matrix3x2 _transform;

        private int _transformChanged = 1;

        private int _selfChanged = 1;

        private int _groupChanged = 1;

        private int _geometryChanged = 1;

        private void SetParentChange()
        {
            if(_parent == null)
            {
                return;
            }
            if (Interlocked.CompareExchange(ref _parent._groupChanged, 1, 0) == 0)
            {
                _parent.SetParentChange();
            }
        }

        private void SetGroupChange()
        {
            if (Interlocked.CompareExchange(ref _groupChanged, 1, 0) == 0)
            {
                _parent?.SetGroupChange();
            }
        }

        public void Move(float shiftX, float shiftY)
        {
            _x += shiftX;
            _y += shiftY;
            Interlocked.CompareExchange(ref _transformChanged, 1, 0);
            SetParentChange();
        }

        public void Rotate(float shift)
        {
            _angle = (_angle + shift).ToAngle();
            Interlocked.CompareExchange(ref _transformChanged, 1, 0);
            SetParentChange();
        }

        public void AddChild(CustomShapeBase child)
        {
            var children = Volatile.Read(ref _children);
            var length = children.Length;
            var newChildren = new CustomShapeBase[length + 1];
            children.CopyTo(newChildren, 0);
            newChildren[length] = child;
            Volatile.Write(ref child._parent, this);
            Volatile.Write(ref _children, newChildren);
            SetGroupChange();
        }

        public void RemoveAll()
        {
            //var children = Volatile.Read(ref _children);
            //foreach(var child in children)
            //{
            //    Volatile.Write(ref child._parent, null);
            //    child.Dispose();
            //}
            Volatile.Write(ref _children, Array.Empty<CustomShapeBase>());
            SetGroupChange();
        }

        public void RemoveChild(CustomShapeBase child)
        {
            if(child._parent != this)
            {
                return;
            }
            var children = Volatile.Read(ref _children);
            var length = children.Length;
            var newChildren = children.Where(c => c != child).ToArray();
            Volatile.Write(ref _children, newChildren);
            Volatile.Write(ref child._parent, null);
            //child.Dispose();
            SetGroupChange();
        }

        public CanvasGeometry GetGeometry(ICanvasResourceCreator creator)
        {
            if (Interlocked.CompareExchange(ref _selfChanged, 0, 1) == 1)
            {
                //_selfGeometry?.Dispose();
                _selfGeometry = CreateSelfGeometry(creator);
                Interlocked.CompareExchange(ref _groupChanged, 1, 0);
            }
            if (Interlocked.CompareExchange(ref _groupChanged, 0, 1) == 1)
            {
                var children = Volatile.Read(ref _children);
                var length = children.Length;
                //var old = _groupGeometry;
                if (length == 0)
                {
                    _groupGeometry = _selfGeometry;
                }
                else
                {
                    var geometrys = new CanvasGeometry[length + 1];
                    geometrys[0] = _selfGeometry;
                    for (int i = 0; i < length; i++)
                    {
                        geometrys[i + 1] = children[i].GetGeometry(creator);
                    }
                    _groupGeometry = CanvasGeometry.CreateGroup(creator, geometrys);
                }
                //if (old != null && old != _selfGeometry)
                //{
                //    old.Dispose();
                //}
                Interlocked.CompareExchange(ref _geometryChanged, 1, 0);
            }
            if (Interlocked.Exchange(ref _transformChanged, 0) == 1)
            {
                _transform = Matrix3x2.Multiply(Matrix3x2.CreateRotation(-_angle.AngleToRadians()), Matrix3x2.CreateTranslation(_x, _y));
                Interlocked.CompareExchange(ref _geometryChanged, 1, 0);
            }
            if (Interlocked.Exchange(ref _geometryChanged, 0) == 1)
            {
                //var old = _geometry;
                if (_transform.IsIdentity)
                {
                    _geometry = _groupGeometry;
                }
                else
                {
                    _geometry = _groupGeometry.Transform(_transform);
                }
                //if (old != null && old != _groupGeometry)
                //{
                //    old.Dispose();
                //}
            }
            return _geometry;
        }

        public bool HasNewGeometry()
        {
            return Volatile.Read(ref _selfChanged) == 1
                || Volatile.Read(ref _groupChanged) == 1
                || Volatile.Read(ref _transformChanged) == 1
                || Volatile.Read(ref _geometryChanged) == 1;
        }

        protected abstract CanvasGeometry CreateSelfGeometry(ICanvasResourceCreator creator);

        //public void Dispose()
        //{
        //    foreach(var child in _children)
        //    {
        //        child.Dispose();
        //    }
        //    _selfGeometry?.Dispose();
        //    _groupGeometry?.Dispose();
        //    _geometry?.Dispose();
        //}
    }
}
