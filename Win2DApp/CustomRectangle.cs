using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

namespace Win2DApp
{
    public sealed class CustomRectangle : CustomShapeBase
    {
        public CustomRectangle(float x, float y, float angle, float width, float height) : base(x, y, angle)
        {
            _left = 0;
            _right = width;
            _top = 0;
            _bottom = height;
        }

        private float _left;

        private float _right;

        private float _top;

        private float _bottom;

        protected override CanvasGeometry CreateSelfGeometry(ICanvasResourceCreator creator)
        {
            return CanvasGeometry.CreateRectangle(creator, _left, _top, _right - _left, _bottom - _top);
        }
    }
}
