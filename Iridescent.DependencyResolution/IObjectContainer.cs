using System;
using System.Collections.Generic;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Iridescent.DependencyResolution
{
    public interface IObjectContainer
    {
        T Resolve<T>();
        object Resolve(Type modelType);
        IList<object> ResolveAll(Type modelType);

        void Register(Type typeFor, Type typeUse);
        void Register(Type typeFor, object instance);

        /// <summary>
        /// 初始化依赖注入容器 需在调用其他方法之前执行
        /// </summary>
        /// <param name="scanAssembles">扫描的程序集</param>
        void Init(string[] scanAssembles = null);

        /// <summary>
        /// 初始化依赖注入容器 需在调用其他方法之前执行
        /// </summary>
        /// <param name="scanAssembles">扫描的程序集</param>
        /// <param name="registries">注入条目对象</param>
        /// <param name="conventions">条目转换对象</param>
        void Init(string[] scanAssembles , Registry[] registries, IRegistrationConvention[] conventions);

    }
}
