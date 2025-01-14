using UnityEngine;

public static class TagUtilities
{
    public static bool HasTag(GameObject obj, TagType tag)
    {
        MultiTag multiTag = obj.GetComponent<MultiTag>();
        return multiTag != null && multiTag.HasTag(tag);
    }
}
