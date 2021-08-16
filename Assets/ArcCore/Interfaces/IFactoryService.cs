using System;

public interface IFactoryService<out T>: IDisposable
{
    T GetService(Type type);
}
