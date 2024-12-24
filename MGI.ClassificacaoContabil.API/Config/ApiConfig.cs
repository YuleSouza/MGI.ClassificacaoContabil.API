﻿using Autofac;
using Infra.Data;
using Infra.Interface;
using Service.Helper;
using System.Reflection;

namespace API.Config
{
    public static class ApiConfig
    {
        public static void RegisterServices(this ContainerBuilder builder, string assemblyName)
        {
            var assemblies = Assembly.Load(assemblyName);
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
            builder.RegisterType<DbSession>().InstancePerLifetimeScope();
            builder.RegisterType<TransactionHelper>().As<ITransactionHelper>().InstancePerDependency();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        public static void RegisterRepositories(this ContainerBuilder repoBuilder, string assemblyName)
        {
            var assemblies = Assembly.Load(assemblyName);

            repoBuilder.RegisterAssemblyTypes(assemblies)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
        }
    }
}
