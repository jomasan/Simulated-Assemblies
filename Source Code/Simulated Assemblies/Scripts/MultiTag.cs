using UnityEngine;
using System.Collections.Generic;

public class MultiTag : MonoBehaviour
{
    [SerializeField]
    private List<TagType> tags = new List<TagType>();

    // Public property to access tags
    public List<TagType> Tags => tags;

    // Method to add a tag
    public void AddTag(TagType tag)
    {
        if (!tags.Contains(tag))
        {
            tags.Add(tag);
        }
    }

    // Method to remove a tag
    public void RemoveTag(TagType tag)
    {
        if (tags.Contains(tag))
        {
            tags.Remove(tag);
        }
    }

    // Method to check if a tag exists
    public bool HasTag(TagType tag)
    {
        return tags.Contains(tag);
    }
}
