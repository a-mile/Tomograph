using StructureMap;

namespace Tomograph.Library.Infrastructure
{
    public static class IoC
    {
        public static Container Container => _container ?? (_container = GetContainer());

        private static Container _container;

        private static Container GetContainer()
        {
            var container = new Container();
            container.Configure(c => c.IncludeRegistry<TomographRegistry>());

            return container;
        }
    }
}
