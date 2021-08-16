
using System;

public interface IServiceProvider: IDisposable
{
    T GetService<T>();
}
