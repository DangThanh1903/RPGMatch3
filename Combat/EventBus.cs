using System;

public static class EventBus {
    public static event Action<int> OnWeaponDestroyed;
    public static event Action OnBlockSwapped;

    public static void TriggerWeaponDestroyed(int damage) {
        OnWeaponDestroyed?.Invoke(damage);
    }

    public static void TriggerBlockSwap() {
        OnBlockSwapped?.Invoke();
    }
}
