using System.Collections.Generic;

namespace AmeisenBotX.Memory.Structs
{
    public readonly struct AllocationPool(nint address, int size)
    {
        public nint Address { get; } = address;

        public SortedList<int, int> Allocations { get; } = [];

        public int Size { get; } = size;

        /// <summary>
        /// Free a memory block in the pool
        /// </summary>
        /// <param name="address">Allocation address</param>
        /// <returns>True, when the block has been found and freed, false if not</returns>
        public bool Free(nint address, out int size)
        {
            int addressInt = address.ToInt32();
            int baseAddressInt = Address.ToInt32();

            if (addressInt >= baseAddressInt
                && addressInt < baseAddressInt + Size)
            {
                int relAddress = addressInt - baseAddressInt;

                size = Allocations[relAddress];
                Allocations.Remove(relAddress);

                return true;
            }

            size = 0;
            return false;
        }

        /// <summary>
        /// Try to reserve a memory block in the pool
        /// </summary>
        /// <param name="size">Size of the wanted block</param>
        /// <param name="address">Address of the memory allocation</param>
        /// <returns>True when a block could be reserved, false if not</returns>
        public bool Reserve(int size, out nint address)
        {
            if (GetNextFreeBlock(size, out int offset))
            {
                Allocations.Add(offset, size);
                address = nint.Add(Address, offset);
                return true;
            }

            address = nint.Zero;
            return false;
        }

        private bool GetNextFreeBlock(int size, out int offset)
        {
            if (size <= Size)
            {
                if (Allocations.Count == 0)
                {
                    offset = 0;
                    return true;
                }
                else
                {
                    IList<int> keys = Allocations.Keys;
                    IList<int> values = Allocations.Values;

                    for (int i = 0; i < Allocations.Count; ++i)
                    {
                        int allocationEnd = keys[i] + values[i];

                        // when there is a next element, use it as the limiter, if not use the
                        // whole remaining space
                        int memoryLeft = i + 1 < Allocations.Count
                            ? keys[i + 1] - allocationEnd
                            : Size - allocationEnd;

                        if (memoryLeft >= size)
                        {
                            offset = allocationEnd;
                            return true;
                        }
                    }
                }
            }

            offset = 0;
            return false;
        }
    }
}