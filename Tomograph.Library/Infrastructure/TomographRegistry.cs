using StructureMap;
using Tomograph.Library.Abstract;
using Tomograph.Library.SuperTomograph;

namespace Tomograph.Library.Infrastructure
{
    public class TomographRegistry : Registry
    {
        public TomographRegistry()
        {
            this.For<IImageLoader>().Use<EmguCVImageLoader>();
            this.For<IEmitterDetectorSystem>().Use<ConicalDetectorEmitterSystem>();            
            this.For<IDicomCreator>().Use<FoDicomCreator>();
        }
    }
}
