using System.Numerics;
using Raylib_cs;

namespace Application.Utils;

public class Frustum
{
    private readonly Plane[] _planes;

    public Frustum()
    {
        _planes = new Plane[6];
    }

    /// <summary>
    /// Update frustum planes using the current RLGL MVP matrices.
    /// </summary>
    /// <remarks>
    /// You should make sure to call this function from inside the Raylib camera mode that you want to do frustum checks against.
    /// </remarks>
    public void UpdatePlanes()
    {
        var mat = Rlgl.rlGetMatrixProjection() * Rlgl.rlGetMatrixModelview();
        var right = new Vector4(mat.M41 - mat.M11, mat.M42 - mat.M12, mat.M43 - mat.M13, mat.M44 - mat.M14);
        var left = new Vector4(mat.M41 + mat.M11, mat.M42 + mat.M12, mat.M43 + mat.M13, mat.M44 + mat.M14);
        var top = new Vector4(mat.M41 - mat.M21, mat.M42 - mat.M22, mat.M43 - mat.M23, mat.M44 - mat.M24);
        var bottom = new Vector4(mat.M41 + mat.M21, mat.M42 + mat.M22, mat.M43 + mat.M23, mat.M44 + mat.M24);
        var front = new Vector4(mat.M41 - mat.M31, mat.M42 - mat.M32, mat.M43 - mat.M33, mat.M44 - mat.M34);
        var back = new Vector4(mat.M41 + mat.M31, mat.M42 + mat.M32, mat.M43 + mat.M33, mat.M44 + mat.M34);

        _planes[0] = Plane.Normalize(new Plane(back));
        _planes[1] = Plane.Normalize(new Plane(front));
        _planes[2] = Plane.Normalize(new Plane(bottom));
        _planes[3] = Plane.Normalize(new Plane(top));
        _planes[4] = Plane.Normalize(new Plane(right));
        _planes[5] = Plane.Normalize(new Plane(left));
    }

    /// <summary>
    /// Checks whether a given axis-aligned bounding-box is inside the current frustum planes.
    /// </summary>
    /// <param name="aabb">Raylib bounding-box to check</param>
    /// <returns>True if the AABB is inside the frustum planes, false otherwise.</returns>
    public bool AabbInside(BoundingBox aabb)
    {
        foreach (var plane in _planes)
        {
            var point = Vector3.Max(aabb.min * plane.Normal, aabb.max * plane.Normal);
            var distance = point.X + point.Y + point.Z + plane.D;
            if (distance <= 0)
                return false;
        }

        return true;
    }
    
    /// <summary>
    /// Check whether a given sphere is inside the current frustum planes.
    /// </summary>
    /// <remarks>
    /// This doesn't exactly tell you whether a sphere is inside the frustum. In some cases on the frustum corners a
    /// sphere may be outside the frustum but inside all of the plane half-spaces. This means that there are occasionally
    /// false positives, but never false negatives.
    /// </remarks>
    /// <param name="center">Center point of the sphere in world space</param>
    /// <param name="radius">Radius of the sphere</param>
    /// <returns>True if the sphere is inside the frustum planes, false otherwise.</returns>
    public bool SphereInside(Vector3 center, float radius)
    {
        return _planes.All(plane => !(Vector3.Dot(plane.Normal, center) + plane.D < -radius));
    }
}