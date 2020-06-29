#region README
//this hearth health display works with any HealthSystem.

//It's pretty much just a class that connects the DotCounter
//with the HealthSystem.
//I'm doing it this way because even UnityEvents don't make
//it possible to subscribe methods to their lists, where the
//method arguments are provided dynamically.
#endregion README

public class HearthHealthDisplay : DotCounter
{
    public HealthSystem hs;

    private void Start() {
        UpdateCounter();
    }

    private void OnEnable() => Subscribe();

    private void OnDisable() => Unsubscribe();
    
    ~HearthHealthDisplay() => Unsubscribe();

    private void Subscribe() {
        hs.OnSetupHealth += SetupCounter;
        hs.OnHealed += IncreaseBy;
        hs.OnTakenDamage += DecreaseBy;
    }

    private void Unsubscribe() {
        hs.OnSetupHealth -= SetupCounter;
        hs.OnHealed -= IncreaseBy;
        hs.OnTakenDamage -= DecreaseBy;
    }
}
