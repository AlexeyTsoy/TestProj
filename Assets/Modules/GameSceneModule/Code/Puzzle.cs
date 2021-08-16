using UnityEngine;

public enum PuzzleType
{
    Union,
    Star,
    Rectangle,
    Ellipse,
    Polygon,
    Quad
}

public class Puzzle : MonoBehaviour
{
#pragma warning disable 0649 // is never assigned to, and will always have its default value null.
    public PuzzleType type;

#pragma warning restore 0649

}
