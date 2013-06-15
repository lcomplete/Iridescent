using Iridescent.DependencyResolution.Impl;

namespace Iridescent.DependencyResolution
{
    public class ContainerFactory
    {
        private static IObjectContainer _objectContainer;

        public static IObjectContainer Singleton
        {
            get
            {
                if(_objectContainer==null)
                    _objectContainer=new StructureMapContainer();

                return _objectContainer;
            }
        }
    }
}
