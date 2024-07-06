using CT_MKWII_WPF.Views.Components;

namespace CT_MKWII_WPF;

    
public class ModItem 
{
    public string ModName { get; set; } = "";
    public bool IsEnabled { get; set; }

    public override string ToString() => "ModItem: " + ModName + " - " + IsEnabled;
}
