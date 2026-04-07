using UnityEngine;

namespace bnj.id_assigner.Runtime
{
    /// <summary>
    /// Abstract base class for ScriptableObjects managed by <see cref="Editor.IdAssignerWindow{T}"/>.
    /// Inherit from this instead of <see cref="ScriptableObject"/> to get ID management with no
    /// boilerplate. IDs are assigned at edit time only — do not modify <see cref="Id"/> or
    /// <see cref="IdSet"/> at runtime.
    /// </summary>
    public abstract class SO_IdContainer : ScriptableObject
    {
        // Fields are serialized so IDs persist with the asset.
        [SerializeField, Tooltip("Set to 0 to reset the ID for reassignment and resolve collisions (duplicate IDs).\nDo NOT change manually under any other circumstances! Use the ID Assigner window instead.")]
        private int _id;
        /// <summary>
        /// Unique integer ID for this asset. Setting it also marks <see cref="IdSet"/> as <c>true</c>.
        /// </summary>
        public int Id
        {
            get => _id;
            set { _id = value; _idSet = true; }
        }

        [SerializeField, HideInInspector]
        private bool _idSet;
        /// <summary>
        /// <c>true</c> once <see cref="Id"/> has been explicitly assigned by the assigner; <c>false</c> on uninitialized assets.
        /// </summary>
        public bool IdSet => _idSet;
    }
}
