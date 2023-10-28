using AmeisenBotX.Common.Math;
using System.Runtime.InteropServices;

/// <summary>
/// Represents raw camera information.
/// </summary>
namespace AmeisenBotX.Wow.Objects.Raw
{
    /// <summary>
    /// Represents raw camera information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawCameraInfo
    {
        /// <summary>
        /// Gets or sets the virtual table pointer.
        /// </summary>
        public uint VTable { get; set; }

        /// <summary>
        /// Gets or sets the value of the Unk1 property.
        /// </summary>
        public uint Unk1 { get; set; }

        ///<summary>
        /// Gets or sets the position in a three-dimensional Cartesian coordinate system.
        ///</summary>
        public Vector3 Pos { get; set; }

        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        public Matrix3x3 ViewMatrix { get; set; }

        /// <summary>
        /// Gets or sets the field of view angle.
        /// </summary>
        public float Fov { get; set; }

        /// <summary>
        /// Gets or sets the value of Unk2.
        /// </summary>
        public float Unk2 { get; set; }

        /// <summary>
        /// Gets or sets the value of the Unk3 property.
        /// </summary>
        public int Unk3 { get; set; }

        /// <summary>
        /// Gets or sets the value of the near clipping plane for the Z-axis in a 3D space.
        /// </summary>
        public float ZNearPlane { get; set; }

        /// <summary>
        /// Gets or sets the distance to the far clipping plane of a 3D scene.
        /// </summary>
        public float ZFarPlane { get; set; }

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        public float Aspect { get; set; }
    }
}