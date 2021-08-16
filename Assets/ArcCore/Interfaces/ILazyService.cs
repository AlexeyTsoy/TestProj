using System;

public interface ILazyService<out T> : IDisposable
{
    T GetService();
}
