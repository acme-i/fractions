namespace fractions
{
    /// <summary>
    /// A way of oscilating between A and Z
    /// </summary>
    public enum IncrementMethod : int
    {
        /// <summary>
        /// A-Z, A-Z etc
        /// </summary>
        MinMax = 0,
        /// <summary>
        /// Z-A, Z-A etc.
        /// </summary>
        MaxMin = 1,
        /// <summary>
        /// A-Z-A-Z-A etc.
        /// </summary>
        Cyclic = 2,
        /// <summary>
        /// A then Z then A then Z etc.
        /// </summary>
        Bit = 3
    }
}
