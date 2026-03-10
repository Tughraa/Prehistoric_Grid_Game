using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string id = "undef";
    public string itemName = "Undefined Item";
    public Sprite sprite;
    public List<string> tags;
}
