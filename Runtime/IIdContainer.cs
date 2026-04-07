namespace bnj.id_assigner.Runtime
{
    /// <summary>
    /// Implement on a <see cref="UnityEngine.ScriptableObject"/> to make its assets manageable by
    /// <see cref="Editor.IdAssigner{T}"/>. IDs are assigned at edit time only — do not modify
    /// <see cref="Id"/> or <see cref="IdSet"/> at runtime.
    /// </summary>
    public interface IIdContainer
    {
        /// <summary>
        /// Unique integer ID for this asset. The setter must also set <see cref="IdSet"/> to <c>true</c>.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// <c>true</c> once <see cref="Id"/> has been explicitly assigned by the assigner; <c>false</c> on uninitialized assets.
        /// </summary>
        bool IdSet { get; }
    }
}
