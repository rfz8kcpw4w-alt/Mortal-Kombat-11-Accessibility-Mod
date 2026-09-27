namespace MK11GameAccessibilityDLL;

/// <summary>
/// Menü yapısını ve navigasyonunu yönetir.
/// MK11Hook Lua scriptinden gelen menü olaylarını işler.
/// </summary>
public sealed class MenuAccessibilityModule
{
    private class MenuItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Index { get; set; }
    }

    private List<MenuItem> currentMenuItems = new();
    private int currentSelection = 0;
    private string currentMenuName = string.Empty;
    private UniversalAccessibilityAnnouncer announcer;

    public MenuAccessibilityModule(UniversalAccessibilityAnnouncer announcer) 
    {
        this.announcer = announcer;
    }

    public void OnMenuOpen(string menuName, int itemCount)
    {
        currentMenuName = menuName;
        currentSelection = 0;
        currentMenuItems.Clear();
        announcer.Announce($"Menü açıldı: {menuName}. {itemCount} seçenek.");
    }

    public void OnMenuClose()
    {
        announcer.Announce($"Menü kapatıldı: {currentMenuName}.");
        currentMenuName = string.Empty;
        currentMenuItems.Clear();
    }

    public void OnMenuItemSelected(string itemText, int index, int total)
    {
        currentSelection = index;
        announcer.Announce($"{itemText}. Seçenek {index} / {total}.", interrupt: true);
    }

    public void NavigateUp()
    {
        if (currentSelection > 1) currentSelection--;
        announcer.Announce("Yukarı.");
    }

    public void NavigateDown()
    {
        if (currentSelection < currentMenuItems.Count) currentSelection++;
        announcer.Announce("Aşağı.");
    }

    public void SelectCurrent()
    {
        if (currentSelection > 0 && currentSelection <= currentMenuItems.Count)
        {
            var item = currentMenuItems[currentSelection - 1];
            announcer.Announce($"{item.Name} seçildi.", interrupt: true);
        }
    }

    public void ReadCurrentMenu()
    {
        var menu = $"Menü: {currentMenuName}. ";
        if (currentMenuItems.Count > 0)
        {
            menu += $"Seçenekler: ";
            foreach (var item in currentMenuItems)
            {
                menu += $"{item.Name}. ";
            }
        }
        announcer.Announce(menu);
    }
}
