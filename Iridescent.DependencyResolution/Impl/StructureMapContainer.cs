using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Iridescent.DependencyResolution.Impl
{
    internal class StructureMapContainer : IObjectContainer
    {

        public T Resolve<T>()
        {
            EnsureDependenciesRegistered();
            return ObjectFactory.GetInstance<T>();
        }

        public object Resolve(Type modelType)
        {
            EnsureDependenciesRegistered();
            return ObjectFactory.GetInstance(modelType);
        }

        public IList<object> ResolveAll(Type modelType)
        {
            EnsureDependenciesRegistered();
            return (IList<object>)ObjectFactory.GetAllInstances(modelType);
        }

        public void Register(Type typeFor, Type typeUse)
        {
            ObjectFactory.Configure(x => x.For(typeFor).Use(typeUse));
        }

        public void Register(Type typeFor, object instance)
        {
            ObjectFactory.Configure(x => x.For(typeFor).Use(instance));
        }

        public void Init(string[] scanAssembles = null)
        {
            Init(scanAssembles, null, null);
        }

        public void Init(string[] scanAssembles, Registry[] registries, IRegistrationConvention[] conventions)
        {
            EnsureDependenciesRegistered(scanAssembles, registries, conventions);
        }

        private static bool _dependenciesRegistered;
        private static readonly object sync = new object();

        /// <summary>
        /// 确保依赖已经注入
        /// </summary>
        public void EnsureDependenciesRegistered(string[] scanAssembles = null, Registry[] registries = null, IRegistrationConvention[] conventions = null)
        {
            if (!_dependenciesRegistered)
            {
                lock (sync)
                {
                    if (!_dependenciesRegistered)
                    {
                        RegisterDependencies(scanAssembles, registries, conventions);
                        _dependenciesRegistered = true;
                    }
                }
            }
        }

        public void RegisterDependencies(string[] scanAssembles, Registry[] registries, IRegistrationConvention[] conventions)
        {
            //要扫描依赖注入的程序集
            IList<string> listAssembles = GetScanAssembles(scanAssembles);

            //要注入的条目
            IList<Registry> listRegistries = GetRegistries(registries);

            //条目转换对象
            IList<IRegistrationConvention> listConvertions = GetRegistrationConvention(conventions);

            ObjectFactory.Initialize(expression =>
                                         {
                                             expression.Scan(
                                                 assemblyScanner =>
                                                 {
                                                     //扫描程序集
                                                     foreach (string assemble in listAssembles)
                                                     {
                                                         assemblyScanner.Assembly(assemble.Trim());
                                                     }

                                                     //设置转换对象
                                                     if (listConvertions == null || listConvertions.Count == 0)
                                                         assemblyScanner.WithDefaultConventions();
                                                     else
                                                     {
                                                         foreach (IRegistrationConvention convention in listConvertions)
                                                         {
                                                             assemblyScanner.With(convention);
                                                         }
                                                     }
                                                 });

                                             //增加注入条目
                                             foreach (Registry registry in listRegistries)
                                             {
                                                 expression.AddRegistry(registry);
                                             }

                                         });
        }

        private IList<string> GetScanAssembles(string[] scanAssembles)
        {
            IList<string> listAssembels = scanAssembles == null ? new List<string>() : new List<string>(scanAssembles);
            string setting = ConfigurationManager.AppSettings["DI_Assembles"];
            var configAssembles = string.IsNullOrWhiteSpace(setting)
                                         ? null
                                         : setting.Split(',');
            if (configAssembles != null)
            {
                Array.ForEach(configAssembles, listAssembels.Add);
            }

            return listAssembels.Distinct().ToList();
        }

        private IList<Registry> GetRegistries(Registry[] registries)
        {
            IList<Registry> listRegistries = registries == null ? new List<Registry>() : new List<Registry>(registries);
            string setting = ConfigurationManager.AppSettings["DI_Registries"];
            var configRegistries = string.IsNullOrWhiteSpace(setting) ? null : setting.Split(';');
            if (configRegistries != null)
            {
                Array.ForEach(configRegistries, s =>
                {
                    Registry registry =
                        (Registry)Activator.CreateInstance(Type.GetType(s));
                    listRegistries.Add(registry);
                });
            }

            return listRegistries;
        }

        private IList<IRegistrationConvention> GetRegistrationConvention(IRegistrationConvention[] conventions)
        {
            IList<IRegistrationConvention> listConvertions = conventions == null
                                                                 ? new List<IRegistrationConvention>()
                                                                 : new List<IRegistrationConvention>(conventions);
            string setting = ConfigurationManager.AppSettings["DI_Convertions"];
            var configConvertions = string.IsNullOrWhiteSpace(setting) ? null : setting.Split(';');
            if (configConvertions != null)
            {
                Array.ForEach(configConvertions, s =>
                {
                    IRegistrationConvention convention =
                        (IRegistrationConvention)Activator.CreateInstance(Type.GetType(s));
                    listConvertions.Add(convention);
                });
            }

            return listConvertions;
        }

    }

}
