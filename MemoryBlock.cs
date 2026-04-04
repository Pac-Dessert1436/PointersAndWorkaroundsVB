namespace PointersAndWorkaroundsVB;

using System.Runtime.InteropServices;

/// <summary>
/// MemoryBlock class for allocating and managing unmanaged memory blocks safely.
/// Provides VB.NET developers with memory management capabilities that VB.NET lacks natively.
/// </summary>
/// <remarks>
/// VB.NET does not support unsafe code or direct memory management. This class provides
/// safe alternatives for working with unmanaged memory, similar to C#'s memory management
/// capabilities but with VB.NET safety constraints.
/// </remarks>
public unsafe class MemoryBlock : IDisposable
{
    private void* _memory; // Pointer to the allocated memory block
    private int _sizeInBytes; // Size of the memory block in bytes
    private bool _isDisposed; // Flag indicating whether the memory block is disposed

    /// <summary>
    /// Allocates a new memory block of the specified size.
    /// VB.NET lacks native support for unmanaged memory allocation.
    /// </summary>
    /// <param name="sizeInBytes">The size of the memory block in bytes.</param>
    /// <param name="initMemoryToZero">Whether to initialize the memory to zero.</param>
    public MemoryBlock(int sizeInBytes, bool initMemoryToZero = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeInBytes);

        _sizeInBytes = sizeInBytes;
        _memory = Marshal.AllocHGlobal(sizeInBytes).ToPointer();

        if (initMemoryToZero) Fill(0);
    }

    /// <summary>
    /// Creates a memory block from an existing pointer.
    /// Use with caution - the caller is responsible for the lifetime of the pointer.
    /// </summary>
    /// <param name="pointer">The existing pointer.</param>
    /// <param name="sizeInBytes">The size of the memory block in bytes.</param>
    /// <returns>A new MemoryBlock instance that references the existing memory.</returns>
    public static MemoryBlock FromPointer<T>(Pointer<T> pointer, int sizeInBytes) where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(pointer);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeInBytes);

        return new MemoryBlock(pointer.RawPointer, sizeInBytes);
    }

    /// <summary>
    /// Private constructor for creating MemoryBlock from existing pointer.
    /// </summary>
    private MemoryBlock(void* ptr, int sizeInBytes)
    {
        _memory = ptr;
        _sizeInBytes = sizeInBytes;
    }

    /// <summary>
    /// Allocates a new memory block for the specified number of elements of type T.
    /// Provides type-safe memory allocation for VB.NET developers.
    /// </summary>
    /// <typeparam name="T">The type of elements to allocate memory for.</typeparam>
    /// <param name="elementCount">The number of elements.</param>
    /// <returns>A new <see cref="MemoryBlock"/> instance.</returns>
    public static MemoryBlock Allocate<T>(int elementCount) where T : unmanaged
    {
        return new MemoryBlock(elementCount * sizeof(T));
    }

    /// <summary>
    /// Gets the size of the memory block in bytes.
    /// </summary>
    public int SizeInBytes => _sizeInBytes;

    /// <summary>
    /// Gets a value indicating whether the memory block is disposed.
    /// </summary>
    public bool IsDisposed => _isDisposed;

    /// <summary>
    /// Gets the address of the memory block as IntPtr.
    /// Useful for interop scenarios where VB.NET needs to pass memory addresses.
    /// </summary>
    public IntPtr Address => (IntPtr)_memory;

    /// <summary>
    /// Creates a typed pointer from this memory block.
    /// Enables VB.NET developers to work with typed pointers safely.
    /// </summary>
    /// <typeparam name="T">The type of pointer to create.</typeparam>
    /// <param name="elementCount">The number of elements.</param>
    /// <returns>A new <see cref="Pointer{T}"/> instance.</returns>
    public Pointer<T> AsPointer<T>(int elementCount) where T : unmanaged
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(MemoryBlock));

        int requiredSize = elementCount * sizeof(T);
        if (requiredSize > _sizeInBytes)
            throw new ArgumentException("Memory block is too small for the specified number of elements.");

        return new Pointer<T>((T*)_memory, elementCount);
    }

    /// <summary>
    /// Fills the memory block with a specific byte value.
    /// Provides memory initialization capabilities that VB.NET lacks.
    /// </summary>
    /// <param name="value">The byte value to fill with.</param>
    public void Fill(byte value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(MemoryBlock));

        byte* ptr = (byte*)_memory;
        for (int i = 0; i < _sizeInBytes; i++)
        {
            ptr[i] = value;
        }
    }

    /// <summary>
    /// Fills the memory block with a typed value.
    /// </summary>
    /// <typeparam name="T">The type of value to fill with.</typeparam>
    /// <param name="value">The value to fill with.</param>
    public void Fill<T>(T value) where T : unmanaged
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(MemoryBlock));

        T* ptr = (T*)_memory;
        int elementCount = _sizeInBytes / sizeof(T);

        for (int i = 0; i < elementCount; i++)
        {
            ptr[i] = value;
        }
    }

    /// <summary>
    /// Copies data from a managed array to this memory block.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The source array.</param>
    /// <param name="startIndex">The starting index in the array.</param>
    /// <param name="count">The number of elements to copy.</param>
    public void CopyFrom<T>(T[] array, int startIndex = 0, int count = 0) where T : unmanaged
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(MemoryBlock));
        ArgumentNullException.ThrowIfNull(array);

        if (count == 0) count = array.Length - startIndex;
        if (startIndex < 0 || startIndex >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (count < 0 || startIndex + count > array.Length)
            throw new ArgumentOutOfRangeException(nameof(count));

        int requiredSize = count * sizeof(T);
        if (requiredSize > _sizeInBytes)
            throw new ArgumentException("Memory block is too small for the specified number of elements.");

        fixed (T* source = &array[startIndex])
        {
            Buffer.MemoryCopy(source, _memory, _sizeInBytes, requiredSize);
        }
    }

    /// <summary>
    /// Copies data from this memory block to a managed array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The destination array.</param>
    /// <param name="startIndex">The starting index in the array.</param>
    /// <param name="count">The number of elements to copy.</param>
    public void CopyTo<T>(T[] array, int startIndex = 0, int count = 0) where T : unmanaged
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(MemoryBlock));
        ArgumentNullException.ThrowIfNull(array);

        if (count == 0) count = array.Length - startIndex;
        if (startIndex < 0 || startIndex >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (count < 0 || startIndex + count > array.Length)
            throw new ArgumentOutOfRangeException(nameof(count));

        int requiredSize = count * sizeof(T);
        if (requiredSize > _sizeInBytes)
            throw new ArgumentException("Memory block does not contain enough data for the specified number of elements.");

        fixed (T* destination = &array[startIndex])
        {
            Buffer.MemoryCopy(_memory, destination, count * sizeof(T), requiredSize);
        }
    }

    /// <summary>
    /// Creates a span over the memory block for safe access.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="elementCount">The number of elements.</param>
    /// <returns>A span over the memory block.</returns>
    public Span<T> AsSpan<T>(int elementCount) where T : unmanaged
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(MemoryBlock));

        int requiredSize = elementCount * sizeof(T);
        if (requiredSize > _sizeInBytes)
            throw new ArgumentException("Memory block is too small for the specified number of elements.");

        return new Span<T>(_memory, elementCount);
    }

    /// <summary>
    /// Creates a read-only span over the memory block for safe access.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="elementCount">The number of elements.</param>
    /// <returns>A read-only span over the memory block.</returns>
    public ReadOnlySpan<T> AsReadOnlySpan<T>(int elementCount) where T : unmanaged
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(MemoryBlock));

        int requiredSize = elementCount * sizeof(T);
        if (requiredSize > _sizeInBytes)
            throw new ArgumentException("Memory block is too small for the specified number of elements.");

        return new ReadOnlySpan<T>(_memory, elementCount);
    }

    /// <summary>
    /// Copies data from this memory block to another memory block.
    /// Provides safe memory copying operations for VB.NET.
    /// </summary>
    /// <param name="destination">The destination memory block.</param>
    /// <param name="count">The number of bytes to copy. If 0, copies the entire memory block.</param>
    public void CopyTo(MemoryBlock destination, int count = 0)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(MemoryBlock));
        ArgumentNullException.ThrowIfNull(destination);

        int bytesToCopy = count == 0 ? _sizeInBytes : count;
        if (bytesToCopy > _sizeInBytes)
            throw new ArgumentException("Source memory block does not have enough space.");
        if (bytesToCopy > destination._sizeInBytes)
            throw new ArgumentException("Destination memory block does not have enough space.");

        Buffer.MemoryCopy(_memory, destination._memory, destination._sizeInBytes, bytesToCopy);
    }

    /// <summary>
    /// Disposes of the memory block, freeing the allocated memory.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected virtual method to release resources.
    /// </summary>
    /// <param name="disposing">True if called from Dispose; false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (_memory is not null)
            {
                Marshal.FreeHGlobal((IntPtr)_memory);
                _memory = null;
            }

            _sizeInBytes = 0;
            _isDisposed = true;
        }
    }

    /// <summary>
    /// Finalizes the memory block.
    /// </summary>
    ~MemoryBlock()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets a string representation of the memory block.
    /// </summary>
    /// <returns>A string containing memory block information.</returns>
    public override string ToString()
    {
        return _isDisposed
            ? "MemoryBlock [Disposed]"
            : $"MemoryBlock [Address: {Address}, Size: {_sizeInBytes} bytes]";
    }
}