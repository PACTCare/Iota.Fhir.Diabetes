namespace Pact.App.Core.Services
{
  using System;
  using System.Collections.Generic;

  using Autofac;
  using Autofac.Core;

  public static class DependencyResolver
  {
    static DependencyResolver()
    {
      Modules = new List<IModule>();
    }

    public static string LocalStoragePath { get; set; }

    public static List<IModule> Modules { get; set; }

    private static IContainer Container { get; set; }

    public static void Init()
    {
      var builder = new ContainerBuilder();

      foreach (var module in Modules)
      {
        builder.RegisterModule(module);
      }

      Container = builder.Build();
    }

    public static void Reload()
    {
      Init();
    }

    public static T Resolve<T>()
    {
      if (Container == null)
      {
        Init();
      }

      return Container.Resolve<T>();
    }

    public static object Resolve(Type type)
    {
      if (Container == null)
      {
        Init();
      }

      return Container.Resolve(type);
    }
  }
}