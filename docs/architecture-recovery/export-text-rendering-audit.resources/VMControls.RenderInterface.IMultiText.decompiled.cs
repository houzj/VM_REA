using System.Collections.Generic;

namespace VMControls.RenderInterface;

public interface IMultiText : IShape, IShapeBase
{
	List<IText> Texts { get; }
}
