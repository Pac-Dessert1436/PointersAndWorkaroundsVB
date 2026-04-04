namespace PointersAndWorkaroundsVB;

/// <summary>
/// Pointer utilities for VB.NET - provides safe pointer operations since VB.NET
/// lacks native support for unsafe code and pointer arithmetic.
/// </summary>
/// <remarks>
/// This module enables VB.NET developers to work with pointers safely, providing
/// alternatives to C#'s unsafe code blocks and pointer operations that VB.NET doesn't support.
/// </remarks>
public static class Pointer
{
    /// <summary>
    /// Creates a typed pointer from an existing array by internally pinning it to prevent
    /// garbage collector movement. Provides a fluent alternative to the constructor.
    /// VB.NET lacks direct pointer creation capabilities.
    /// </summary>
    /// <param name="array">The array to create a pointer from. Must not be null.</param>
    /// <returns>A new <see cref="Pointer{T}"/> instance pointing to the pinned array.</returns>
    public static Pointer<T> Create<T>(T[] array) where T : unmanaged => new(array);

    /// <summary>
    /// Gets the upper bound index of the pointer, which is one less than the total number of elements.
    /// Provides VB.NET-style UBound functionality for pointer-based arrays.
    /// </summary>
    /// <param name="ptr">The pointer to get the upper bound for.</param>
    /// <returns>The upper bound index of the pointer.</returns>
    public static int UBound<T>(Pointer<T> ptr) where T : unmanaged => ptr.Length - 1;
}

/// <summary>
/// Pointer class to manage unmanaged memory pointers, providing safe access to unmanaged memory.
/// VB.NET lacks native support for unsafe code and pointer operations.
/// </summary>
/// <typeparam name="T">The unmanaged type of elements in the pointer.</typeparam>
public unsafe class Pointer<T> : IDisposable where T : unmanaged
{
    private readonly T* _ptr; // Pointer to the first element of type T
    private int _length; // Number of elements
    private bool _isDisposed = false; // To detect redundant calls

    /// <summary>
    /// Creates a pointer from an existing array (internally pins the array to prevent GC movement).
    /// VB.NET cannot perform this operation natively.
    /// </summary>
    public Pointer(T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        _length = array.Length;
        fixed (T* p = array) // Pin the array to prevent GC movement
        {
            _ptr = p;
        }
    }

    /// <summary>
    /// Creates a pointer from a raw pointer and length, ensuring the pointer points to valid 
    /// memory. Note that VB.NET does not support raw pointers.
    /// </summary>
    internal Pointer(T* ptr, int length)
    {
        ArgumentNullException.ThrowIfNull(ptr);
        _ptr = ptr;
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        _length = length;
    }

    /// <summary>
    /// Gets the number of elements in the pointer.
    /// </summary>
    public int Length => _length;

    /// <summary>
    /// Gets a value indicating whether the pointer is null.
    /// </summary>
    public bool IsNull => _ptr == null;

    /// <summary>
    /// Gets the size of each element in bytes.
    /// </summary>
    public int ElementSize => sizeof(T);

    /// <summary>
    /// Gets the type of elements in the pointer.
    /// </summary>
    public Type ElementType => typeof(T);

    /// <summary>
    /// Accesses elements via index (similar to pointer offset).
    /// Provides safe array-like access to pointer-based memory.
    /// </summary>
    public T this[int index]
    {
        get
        {
            ObjectDisposedException.ThrowIf(_isDisposed, nameof(Pointer<>));
            if (index < 0 || index >= _length) throw new IndexOutOfRangeException();
            return *(_ptr + index); // Pointer offset access
        }
        set
        {
            ObjectDisposedException.ThrowIf(_isDisposed, nameof(Pointer<>));
            if (index < 0 || index >= _length) throw new IndexOutOfRangeException();
            *(_ptr + index) = value; // Pointer offset assignment
        }
    }

    /// <summary>
    /// Gets the address of the first element (as IntPtr for cross-language interop).
    /// </summary>
    public IntPtr Address => (IntPtr)_ptr;

    /// <summary>
    /// Creates a new pointer at the specified offset from the current pointer.
    /// </summary>
    /// <param name="offset">The number of elements to offset.</param>
    /// <returns>A new pointer at the specified offset.</returns>
    public Pointer<T> Offset(int offset)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(Pointer<>));
        if (offset < 0 || offset >= _length) throw new ArgumentOutOfRangeException(nameof(offset));
        return new Pointer<T>(_ptr + offset, _length - offset);
    }

    /// <summary>
    /// Copies elements from this pointer to another pointer.
    /// </summary>
    /// <param name="destination">The destination pointer.</param>
    /// <param name="count">The number of elements to copy. If 0, copies all elements.</param>
    public void CopyTo(Pointer<T> destination, int count = 0)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(Pointer<>));
        ArgumentNullException.ThrowIfNull(destination);

        int elementsToCopy = count == 0 ? _length : count;
        if (elementsToCopy > _length)
            throw new ArgumentException("Source pointer does not have enough space.");
        if (elementsToCopy > destination.Length)
            throw new ArgumentException("Destination pointer does not have enough space.");

        Buffer.MemoryCopy(_ptr, destination._ptr, destination.Length * sizeof(T), elementsToCopy * sizeof(T));
    }

    /// <summary>
    /// Copies elements from this pointer to an array.
    /// </summary>
    /// <param name="destination">The destination array.</param>
    /// <param name="count">The number of elements to copy. If 0, copies all elements.</param>
    public void CopyTo(T[] destination, int count = 0)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(Pointer<>));
        ArgumentNullException.ThrowIfNull(destination);

        int elementsToCopy = count == 0 ? _length : count;
        if (elementsToCopy > _length)
            throw new ArgumentException("Source pointer does not have enough space.");
        if (elementsToCopy > destination.Length)
            throw new ArgumentException("Destination array does not have enough space.");

        fixed (T* destPtr = destination)
        {
            Buffer.MemoryCopy(_ptr, destPtr, destination.Length * sizeof(T), elementsToCopy * sizeof(T));
        }
    }

    /// <summary>
    /// Copies elements from an array to this pointer.
    /// </summary>
    /// <param name="source">The source array.</param>
    /// <param name="count">The number of elements to copy. If 0, copies all elements.</param>
    public void CopyFrom(T[] source, int count = 0)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(Pointer<>));
        ArgumentNullException.ThrowIfNull(source);

        int elementsToCopy = count == 0 ? source.Length : count;
        if (elementsToCopy > source.Length)
            throw new ArgumentException("Source array does not have enough space.");
        if (elementsToCopy > _length)
            throw new ArgumentException("Pointer does not have enough space.");

        fixed (T* srcPtr = source)
        {
            Buffer.MemoryCopy(srcPtr, _ptr, _length * sizeof(T), elementsToCopy * sizeof(T));
        }
    }

    /// <summary>
    /// Fills the pointer with a specific value.
    /// </summary>
    /// <param name="value">The value to fill with.</param>
    public void Fill(T value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(Pointer<>));

        for (int i = 0; i < _length; i++)
        {
            _ptr[i] = value;
        }
    }

    /// <summary>
    /// Converts the pointer to an array of elements.
    /// </summary>
    /// <returns>An array containing all elements in the pointer.</returns>
    public T[] ToArray()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(Pointer<>));

        T[] array = new T[_length];
        CopyTo(array);
        return array;
    }

    /// <summary>
    /// Gets a string representation of the pointer.
    /// </summary>
    /// <returns>A string containing pointer information.</returns>
    public override string ToString()
    {
        return _isDisposed
            ? $"Pointer<{typeof(T).Name}> [Disposed]"
            : $"Pointer<{typeof(T).Name}> [Address: {Address}, Length: {_length}, ElementSize: {ElementSize} bytes]";
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current pointer.
    /// </summary>
    /// <param name="obj">The object to compare with the current pointer.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is Pointer<T> other)
        {
            return _ptr == other._ptr && _length == other._length;
        }
        return false;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current pointer.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine((nint)_ptr, _length);
    }

    /// <summary>
    /// Disposes of the pointer, setting it to null and length to zero.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected virtual method to release resources.
    /// </summary>
    /// <param name="disposing">True if called from Dispose method; false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            // No managed resources to dispose
            if (disposing) { }

            // Dispose of unmanaged resources, by clearing memory of each element
            for (int i = 0; i < _length; i++)
                _ptr[i] = default;

            _length = 0;
            _isDisposed = true;
        }
    }
}