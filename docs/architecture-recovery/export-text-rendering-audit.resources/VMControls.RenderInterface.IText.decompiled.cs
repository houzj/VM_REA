using System.Windows;

namespace VMControls.RenderInterface;

public interface IText : IShapeBase
{
	string Content { get; }

	Point Position { get; }

	int FontSize { get; }
}
