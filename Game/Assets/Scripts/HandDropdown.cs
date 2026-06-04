using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Script que informa los valores del combo de mano dominante
/// </summary>
public class HandDropdown : MonoBehaviour
{
    public TMP_Dropdown handDropdown;
    
    // Start is called before the first frame update
    void Start()
    {
        handDropdown.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Diestro"),
            new TMP_Dropdown.OptionData("Zurdo"),
        };

        handDropdown.AddOptions(options);
        handDropdown.value = 0;
    }
}