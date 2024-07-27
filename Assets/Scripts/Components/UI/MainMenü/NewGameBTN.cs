using Components.UI.UIButton_Sliders;
using Events;

namespace Components.UI.MainMenü
{
    public class NewGameBTN  : UIButton
    {
        protected override void OnClick()
        {
            MainMenüEvents.NewGameBTN?.Invoke();
        }
    }
}