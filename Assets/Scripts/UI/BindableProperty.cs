using System;
using System.Collections.Generic;
using Unity.Properties;

public class BindableProperty<T>
{
    T _value;
    public event Action<T> OnValueChanged;

    public BindableProperty()
    {
        _value = default;
    }
    public BindableProperty(T value)
    {
        _value = value;
    }

    [CreateProperty]
    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }
}