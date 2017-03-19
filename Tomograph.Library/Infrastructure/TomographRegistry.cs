using Moq;
using StructureMap;
using Tomograph.Library.Abstract;
using Tomograph.Library.SuperTomograph;

namespace Tomograph.Library.Infrastructure
{
    public class TomographRegistry : Registry
    {
        public TomographRegistry()
        {
            this.For<IBitmapLoader>().Use<SimpleBitmapLoader>();
            this.For<IEmitterDetectorSystem>().Use<ConicalDetectorEmitterSystem>();
            this.For<IOutputBitmapFilter>().Use(new Mock<IOutputBitmapFilter>().Object);
        }
    }
}
